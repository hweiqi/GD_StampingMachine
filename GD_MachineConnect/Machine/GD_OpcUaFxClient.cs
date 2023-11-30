﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GD_CommonLibrary.Extensions;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.UaFx;
using Opc.UaFx.Client;
using Org.BouncyCastle.Security;

namespace GD_MachineConnect.Machine
{

    //https://docs.traeger.de/en/software/sdk/opc-ua/net/client.development.guide#reading-values

    public class GD_OpcUaFxClient: IAsyncDisposable
    {
        public enum ClientState
        {
            Created = 0,
            Connecting = 1,
            Connected = 2,
            Disconnecting = 3,
            Disconnected = 4,
            Reconnecting = 5,
            Reconnected = 6
        }



        private bool disposedValue;
        public async ValueTask DisposeAsync()
        {
            if (!disposedValue)
            {
                Disconnect();
                disposedValue = true;
            }

        }

        OpcClient m_OpcUaClient = new OpcClient();


        ~GD_OpcUaFxClient()
        {
            DisposeAsync();
        }
        public GD_OpcUaFxClient()
        {

        }

        private string HostPath;
        public GD_OpcUaFxClient(string hostPath, int port = 0, string dataPath = null, string user = null, string password=null) 
        {
            Opc.UaFx.Client.OpcClientIdentity userIdentity = new(user, password);

            HostPath = hostPath;
            var baseUrl = CombineUrl(hostPath, port, dataPath);
            m_OpcUaClient = new OpcClient(baseUrl.ToString());

            m_OpcUaClient.Security.UserIdentity = userIdentity;
            m_OpcUaClient.SessionTimeout = OpcClient.DefaultSessionTimeout;

            m_OpcUaClient.StateChanged += (sender, e) =>
            {
                IsConnected = e.NewState == OpcClientState.Connected;
                State = (ClientState)e.NewState;
            };
        }


        public ClientState State { get; private set; }


        //public int ConntectMillisecondsTimeout = 3000;

        private Uri CombineUrl(string hostPath , int? port ,string dataPath)
        {
            if (!hostPath.Contains("opc.tcp://"))
                hostPath = "opc.tcp://" + hostPath;
            if (port.HasValue)
                hostPath += $":{port}";
            var BaseUrl = new Uri(hostPath);
            return new Uri(BaseUrl, dataPath);
        }

        private const int retryCounter = 5;

        //private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public bool IsConnected { get; private set; }
        public async Task<bool> AsyncConnect()
        {
            if (disposedValue)
                return false;
            if (TcpPing.RetrieveIpAddress(HostPath, out var _ip))
            {
                if (!await TcpPing.IsPingableAsync(_ip))
                {
                    ConnectException = new PingException($"Ping Host: {_ip} is Failed");
                    return false;
                }
            }
            await Task.Run(() =>
            {
                try
                {
                    if (m_OpcUaClient.State == OpcClientState.Created || m_OpcUaClient.State == OpcClientState.Disconnected)
                    {
                        m_OpcUaClient.Connect();
                        m_OpcUaClient.DisconnectTimeout = int.MaxValue;
                        m_OpcUaClient.SessionTimeout = int.MaxValue;
                        ConnectException = null;
                    }
                }
                catch (Exception ex)
                {
                    ConnectException = ex;
                }
                finally
                {

                }
            });
            await WaitForCondition.WaitNotAsync(() => m_OpcUaClient.State, OpcClientState.Connecting, 5000);
            var ret = m_OpcUaClient.State == OpcClientState.Connected;
            return ret;
        }

        public Exception ConnectException { get;private set; }

        public void Disconnect()
        {
            try
            {
                m_OpcUaClient.DisconnectTimeout = 100;
                m_OpcUaClient.SessionTimeout = 100;
                m_OpcUaClient.Disconnect();
            }
            catch
            {

            }
        }

        public async Task DisconnectAsync()
        {
            try
            {
                m_OpcUaClient.DisconnectTimeout = 100;
                m_OpcUaClient.SessionTimeout = 100;
                m_OpcUaClient.Disconnect();
                await WaitForCondition.WaitAsync(() => m_OpcUaClient.State, OpcClientState.Connected, false);
            }
            catch
            {

            }
        }



        /// <summary>
        /// 寫入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeTreeString"></param>
        /// <param name="WriteValue"></param>
        /// <returns></returns>
        public async Task<bool> AsyncWriteNode<T>(string NodeTreeString, T WriteValue)
        {
            var ret = false;

            for (int i = 0; i < retryCounter; i++)
            {
                try
                {
                    if (m_OpcUaClient.State == OpcClientState.Connected)
                    {
                        var result = await Task.Run(() =>
                        {
                            OpcStatus opcresult = m_OpcUaClient.WriteNode(NodeTreeString, WriteValue);
                            return opcresult;
                        });

                        if (result.IsGood)
                            return true;
                    }

                }

                catch (Exception ex)
                {
                    await Task.Delay(10);
                    //Disconnect();
                }
            }

            return ret;
        }


        /// <summary>
        /// 寫入數列組
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeTreeString"></param>
        /// <param name="WriteValue"></param>
        /// <returns></returns>

        public async Task<IEnumerable<bool>> AsyncWriteNodes(Dictionary<string,object> NodeTrees)
        {
            var ret = new List<bool>();
            if (NodeTrees.Count == 0)
                return ret;

            List<OpcWriteNode> commands = new(NodeTrees.Select(node => new OpcWriteNode(node.Key, node.Value)));

            for (int i = 0; i < retryCounter; i++)
            {
                try
                {
                    if (m_OpcUaClient.State== OpcClientState.Connected)
                    {
                        // var tags = NodeTrees.Keys.ToArray();
                        //var values = NodeTrees.Values.ToArray();
                        // ret = m_OpcUaClient.WriteNodes(tags, values);

                        var result = await Task.Run(() =>
                        {
                            return m_OpcUaClient.WriteNodes(commands);
                        });
                        ret = result.Select(x => x.IsGood).ToList();
                    }

                }
                catch (Opc.Ua.ServiceResultException sex)
                {
                    //Disconnect();
                }
                catch (Exception ex)
                {
                    await Task.Delay(10);
                    //Disconnect();
                }
            }
            return ret;
        }







        /// <summary>
        /// 讀取點
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeID"></param>
        /// <returns></returns>
        public async Task<(bool , T)> AsyncReadNode<T>(string NodeID)
        {
            //T NodeValue = default(T);

            for (int i = 0; i < retryCounter; i++)
            {
                try
                {
                    if (m_OpcUaClient.State == OpcClientState.Connected)
                    {
                        //T NodeValue;
                        // NodeValue = m_OpcUaClient.ReadNode<T>(NodeID);
                        OpcValue rNode = await Task.Run(() =>
                        {
                            return m_OpcUaClient.ReadNode(NodeID);
                        });
                           
                        if(rNode.Value  is T NodeValue) 
                        {
                            return (true, NodeValue);
                        }
                        return (false, default(T));
                    }
                }
                catch(Opc.Ua.ServiceResultException sex)
                {
                   // Disconnect();
                }
                catch (Exception ex)
                {
                    await Task.Delay(50);
                    //Disconnect();
                }
                await Task.Delay(10);
            }

            return (false, default(T));
        }



        /// <summary>
        /// 讀取點
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeID"></param>
        /// <returns></returns>
        public async Task<(bool result, IEnumerable<T>values)> AsyncReadNodes<T>(IEnumerable<string> NodeTrees)
        {
            var commands = new List<OpcReadNode>();
            foreach (var node in NodeTrees)
            {
                commands.Add(new OpcReadNode(node));
            }


            //T NodeValue = default(T);
            for (int i = 0; i < retryCounter; i++)
            {
                try
                {
                    if (m_OpcUaClient.State == OpcClientState.Connected)
                    {
                        //T NodeValue;
                        // NodeValue = m_OpcUaClient.ReadNode<T>(NodeID);


                        var job = await Task.Run(() =>
                        {
                            return m_OpcUaClient.ReadNodes(commands).ToList();
                        });

                        var values = job.Select(x => x.Value).ToList();
                        if (values.Exists(x => x is not T))
                            return (false, null);
                        else
                        {
                            var Tvalues = values.OfType<T>();
                            return (true, Tvalues);
                        }
                    }
                }
                catch (Opc.Ua.ServiceResultException sex)
                {
                    // Disconnect();
                }
                catch (Exception ex)
                {
                    await Task.Delay(50);
                    //Disconnect();
                }
                await Task.Delay(1);
            }
            return (false, null);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NodeID"></param>
        /// <param name="updateAction"></param>
        /// <param name="samplingInterval"></param>
        /// <param name="checkDuplicates"></param>
        /// <returns></returns>
        public async Task<bool> SubscribeNodeDataChangeAsync<T>(string NodeID, Action<T> updateAction, int samplingInterval ,bool checkDuplicates =false)
        {
            var ret = await SubscribeNodesDataChangeAsync(new List<(string, Action<T>,int ,bool checkDuplicates)>
            {
                (NodeID, updateAction , samplingInterval , checkDuplicates)
            });
            return ret.FirstOrDefault();
        }



        /// <summary>
        /// 存放ID
        /// </summary>
        //private Dictionary<int,long> subscribeDictionary = new Dictionary<int,long>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        public async Task<IList<bool>> SubscribeNodesDataChangeAsync<T>(IList<(string NodeID, Action<T> updateAction, int samplingInterval , bool checkDuplicates)> nodeList)
        {
            return await Task.Run(async () =>
            {
                List<bool> retList = new List<bool>();
                try
                {
                    for (int i = 0; i < nodeList.Count(); i++)
                    {
                        var index = i;
                        var (NodeID, updateAction, samplingInterval, checkDuplicates) = nodeList[index];
                        bool ret = false;
                        for (int j = 0; j < 5 && !ret; j++)
                        {
                            try
                            {
                                //使用設定的響應速度進行分類
                                OpcSubscription opcSubscription = m_OpcUaClient.Subscriptions.FirstOrDefault(x => Equals(x.Tag, samplingInterval));

                                if (opcSubscription == null)
                                {
                                    opcSubscription = m_OpcUaClient.SubscribeNodes();
                                    opcSubscription.Tag = samplingInterval;
                                }
                                //檢查是否存在
                                if (checkDuplicates &&
                                opcSubscription.MonitoredItems.Any(item => item.NodeId.OriginalString == NodeID))
                                {
                                    ret = true;
                                    break;
                                }


                                // Create an OpcMonitoredItem for the NodeId.
                                var item = new OpcMonitoredItem(NodeID, OpcAttribute.Value);
                             
                                item.DataChangeReceived += (sender, e) =>
                                {
                                    /*OpcMonitoredItem item = (OpcMonitoredItem)sender;
                                    Console.WriteLine(
                                            "Data Change from Index {0}: {1}",
                                            item.Tag,
                                            e.Item.Value)*/
                                    try
                                    {
                                        if (e.Item.Value.Value is T Tvalue)
                                        {
                                            updateAction?.Invoke(Tvalue);
                                        }
                                    }
                                    catch
                                    {

                                    }
                                };
                                // You can set your own values on the "Tag" property
                                // that allows you to identify the source later.
                                item.Tag = index;
                                // Set a custom sampling interval on the 
                                // monitored item.
                                item.SamplingInterval = samplingInterval;



                                // Add the item to the subscription.
                                opcSubscription.AddMonitoredItem(item);
                                // After adding the items (or configuring the subscription), apply the changes.
                                CancellationTokenSource cts = new CancellationTokenSource(10000);
                                await WaitForCondition.WaitAsync(()=>m_OpcUaClient.State, OpcClientState.Connected, cts.Token);
                                opcSubscription.ApplyChanges();
                                ret = true;
                                break;
                            }
                            catch (Exception ex)
                            {
                                ret = false;
                                //Debugger.Break();
                                Debug.WriteLine(ex.ToString());
                            }
                        }
                        retList.Add(ret);
                    }

                }
                catch (Exception ex)
                {
                    Debugger.Break();
                    Debug.WriteLine(ex.ToString());
                }
                return retList;
            });
        }

        /// <summary>
        /// 移掉訂閱的節點(使用響應速度分類)
        /// </summary>
        /// <param name="NodeID"></param>
        public async Task<bool> UnsubscribeNodeAsync(string NodeID, int samplingInterval)
        {
            return await Task.Run(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        OpcSubscription opcSubscription = m_OpcUaClient.Subscriptions.FirstOrDefault(x => Equals(x.Tag, samplingInterval));
                        if (opcSubscription != null)
                        {
                            List<OpcMonitoredItem> removeItems = new();
                            foreach (var monitoredItem in opcSubscription.MonitoredItems)
                            {
                                if (NodeID == monitoredItem.NodeId)
                                {
                                    removeItems.Add(monitoredItem);
                                }
                            }
                            foreach (var mitem in removeItems)
                            {
                                opcSubscription.RemoveMonitoredItem(mitem);
                            }
                            opcSubscription.ApplyChanges();
                            return true;
                        }
                        else
                            return true;
                    }
                    catch
                    {

                    }
                }
                return false;
            });
        }







    }
}
