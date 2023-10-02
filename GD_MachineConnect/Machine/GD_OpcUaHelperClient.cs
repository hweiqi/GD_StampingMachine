﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Opc.Ua;
using OpcUaHelper;

namespace GD_MachineConnect.Machine
{

    //https://www.cnblogs.com/daji2020/p/16627627.html

    public class GD_OpcUaHelperClient
    {
        readonly OpcUaClient  m_OpcUaClient = new OpcUaClient();
        public GD_OpcUaHelperClient(string hostPath , int port =0 , string dataPath = null, IUserIdentity userIdentity = null)
        {
            HostPath = hostPath;
            if(Port !=0)
                Port = port;
            else
                Port = null;
            DataPath = dataPath;
            UserIdentity = userIdentity;
        }
        public GD_OpcUaHelperClient(string hostPath, int? port = null, string dataPath = null, IUserIdentity userIdentity =null)
        {
            HostPath = hostPath;
            Port = port;
            DataPath = dataPath;
            UserIdentity = userIdentity;
        }


        public IUserIdentity UserIdentity
        {
            get;set;
        }

        public string HostPath { get; set; }
        public int? Port { get; set; }
        public string DataPath { get; set; } = null;

        public int ConntectMillisecondsTimeout = 1000;


        private Uri CombineUrl(string hostPath , int? port ,string dataPath)
        {
            //var hostPath = HostPath;
            if (!hostPath.Contains("opc.tcp://"))
                hostPath = "opc.tcp://" + hostPath;
            if (port.HasValue)
                hostPath += $":{port}";
            var BaseUrl = new Uri(hostPath);
            return new Uri(BaseUrl, dataPath);
        }

        private const int retryCounter = 5;



        public async Task<bool> AsyncConnect()
        {
            return await AsyncConnect(HostPath,Port,DataPath,UserIdentity);
        }

        public async Task<bool> AsyncConnect(string hostPath, int? port, string dataPath, IUserIdentity userIdentity)
        {
            if (m_OpcUaClient.Connected)
                return true;

            try
            {
                m_OpcUaClient.UserIdentity = userIdentity;
                var baseUrl = CombineUrl(hostPath, port, dataPath);
                await m_OpcUaClient.ConnectServer(baseUrl.ToString());
                ConnectException = null;
                return true;
            }
            catch (Exception ex)
            {
                ConnectException = ex;
            }
            return false;
        }

        public Exception ConnectException { get;private set; }





        public void Disconnect()
        {
            m_OpcUaClient.Disconnect();
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
                    if (!m_OpcUaClient.Connected)
                    {
                       await AsyncConnect();
                    }
                    ret = await m_OpcUaClient.WriteNodeAsync(NodeTreeString, WriteValue);
                    if (ret)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {

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
            T NodeValue = default(T);
            bool IsSucessful = false;
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    if (!m_OpcUaClient.Connected)
                    {
                       await AsyncConnect();
                    }
                    NodeValue = await m_OpcUaClient.ReadNodeAsync<T>(NodeID);
                    IsSucessful = true;
                    break;
                }
                catch (Exception ex)
                {
                    IsSucessful = false;
                }
            }


            return (IsSucessful, NodeValue);

        }




        /*public void ReadNoteAttributes(string NodeTreeString)
        {
            this.OpcuaConnectAsync();
            OpcNodeAttribute[] nodeAttributes = m_OpcUaClient.ReadNoteAttributes(NodeTreeString);
            foreach (var item in nodeAttributes)
            {
                Console.Write(string.Format("{0,-30}", item.Name));
                Console.Write(string.Format("{0,-20}", item.Type));
                Console.Write(string.Format("{0,-20}", item.StatusCode));
                Console.WriteLine(string.Format("{0,20}", item.Value));
            }
            Console.WriteLine("-------------------------------------");

            m_OpcUaClient.Disconnect();

        }*/



        /*public bool ReadAllReference(string NodeTreeString , out List<NodeTypeValue> NodeValue)
        {
            NodeValue = new List<NodeTypeValue>();

            m_OpcUaClient.ConnectServer();
            //取得所有節點
            try
            {
                List<ReferenceDescription> referencesList = new List<ReferenceDescription>();

                var NodeIDStringList = new List<string>() { NodeTreeString };
                var ExistedNodeIDStringList = new List<string>();
                while (true)
                {
                    var NodeSearchList = NodeIDStringList.Except(ExistedNodeIDStringList);
                    var NextSearchList = new List<string>();
                    foreach (var NodeIDString in NodeSearchList)
                    {
                        try
                        {
                            ReferenceDescription[] references = m_OpcUaClient.BrowseNodeReference(NodeIDString);
                            foreach (var Ref in references)
                            {

                                //展開
                                NextSearchList.Add(Ref.NodeId.ToString());
                                referencesList.Add(Ref);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    ExistedNodeIDStringList.AddRange(NodeSearchList);
                    NodeIDStringList.AddRange(NextSearchList);

                    if (NextSearchList.Count == 0)
                        break;
                }

                //展開所有節點
                var GetNodeValue = new List<NodeTypeValue>();
                referencesList.ForEach(reference =>
                {
                    try
                    {
                        //ReadNoteAttributes(reference.NodeId.ToString());
                        ReadNode(reference.NodeId.ToString(), out object NValue);

                        GetNodeValue.Add(new NodeTypeValue()
                        {
                            NodeID = reference.NodeId,
                            NodeDisplayName = reference.DisplayName,
                            NodeValue = NValue
                        });
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                });
                //印出節點資料
                GetNodeValue.ForEach(NValue =>
                {
                    try { 
                    Type NodeType = null;
                    if (NValue.NodeValue != null)
                        NodeType = NValue.NodeValue.GetType();

                    Console.Write(string.Format("{0,-20}", nameof(NValue.NodeID)));
                    Console.WriteLine(string.Format("{0,0}", NValue.NodeID));
                    Console.Write(string.Format("{0,-20}", nameof(NValue.NodeDisplayName)));
                    Console.WriteLine(string.Format("{0,0}", NValue.NodeDisplayName));
                    Console.Write(string.Format("{0,-20}", nameof(Type)));
                    Console.WriteLine(string.Format("{0,0}", NodeType));
                    Console.Write(string.Format("{0,-20}", nameof(NValue.NodeValue)));
                    Console.WriteLine(string.Format("{0,0}", NValue.NodeValue));
                    Console.WriteLine("".PadLeft(50, '-'));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                });


                NodeValue = GetNodeValue;
                return true;

            }
            catch (Exception ex)
            {

            }
            finally 
            {
                m_OpcUaClient.Disconnect();
            }
            return false;
        }*/

        public class NodeTypeValue
        {
            public ExpandedNodeId NodeID { get; set; }
            public LocalizedText NodeDisplayName { get; set; }
            public object NodeValue { get; set; }
        }


    }
}
