﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GD_MachineConnect;
using GD_MachineConnect.Machine;
using GD_MachineConnect.Machine.Interfaces;
using Opc.Ua;
using OpcUaHelper;
using static System.Net.Mime.MediaTypeNames;
using static GD_MachineConnect.Machine.GD_OpcUaHelperClient;

namespace GD_MachineConnectTest
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
           // string HostString = "127.0.0.1";
            //int Port = 62541;
            //string ServerDataPath = "SharpNodeSettings/OpcUaServer";
            string HostString = "192.168.1.123";
            int Port = 4842;
            string ServerDataPath = "";


            var BaseUrl = new Uri($"opc.tcp://{HostString}:{Port}");
            var CombineUrl = new Uri(BaseUrl, ServerDataPath);
            var ServerUrl = CombineUrl.ToString();
            
            OpcUaHelper.Forms.FormBrowseServer formBrowseServer = new OpcUaHelper.Forms.FormBrowseServer(ServerUrl);
            formBrowseServer.ShowDialog();

            Task.Run(async () =>
            {
                try
                {
                    //var Opcua = new GD_OpcUaHelperClient();
                    /*Opcua.UserIdentity = new UserIdentity("Administrator", "pass");
                    if (await Opcua.OpcuaConnectAsync(HostString, Port, ServerDataPath))
                    {
                        Opcua.ReadNode("ns=4;s=APPL.Feeding1.sv_rFeedingPosition", out float Pos);

                        //Opcua.WriteNode();
                        //Opcua.ReadNode_TEST();
                        // Opcua.ReadReference_Test();
                        // Opcua.ReadAllReference("ns=4;i=0");
                        Console.WriteLine("------------------------------------");
                        //Opcua.ReadAllReference("ns=5;i=1");
                        Opcua.Disconnect();
                    }*/

                    GD_Stamping_Opcua SMachine = new GD_Stamping_Opcua(HostString, Port, ServerDataPath, null, null);
                    if (await SMachine.AsyncConnect())
                    {
                        var status = await SMachine.GetMachineStatus();
                        var a = await SMachine.GetEngravingRotateStation();
                        var b = await SMachine.GetRotatingTurntableInfo();
                        SMachine.Disconnect();
                    }
                }
                catch (Exception ex)
                {

                }



            });

            Console.ReadKey();
            //opc.tcp://127.0.0.1:62541/SharpNodeSettings/OpcUaServer




            Thread.Sleep(1000);

            short input = 0;

            while (true)
            {
                 Console.WriteLine("...");
                // Console.WriteLine("請輸入temp1的值(short)");
                //var shortstring = Console.ReadLine();

                //if (shortstring.ToLower() == "exit")
                   // break;

              //  if (short.TryParse(shortstring, out input))
                {
                    Task.Run(async () =>
                        {
                            await Task.Delay(1000);
                            var Opcua = new GD_OpcUaHelperClient();
                            Opcua.UserIdentity = new UserIdentity("Administrator", "pass");
                            if (await Opcua.OpcuaConnectAsync(HostString, Port, ServerDataPath))
                            {
                                //Opcua.ReadAllReference("ns=4;s=APPL.EngravingRotate1", out _);

                                //var allR = Opcua.ReadAllReference();
                                //var newid = new NodeId("ns=2;s=Devices/M1/FAC1/TEST Device1/Temp1");

                           await     Opcua.AsyncReadNode<short>("ns=2;s=Devices/M1/FAC1/TEST Device1/Temp1");
                     await           Opcua.AsyncWriteNode("ns=2;s=Devices/M1/FAC1/TEST Device1/Temp1", (short)input);

                                //Opcua.ReadNode_TEST();
                                // Opcua.ReadReference_Test();
                                 List<NodeTypeValue> Listdata = new List<NodeTypeValue>();
                                //Opcua.ReadAllReference("ns=4;s=APPL.EngravingRotate1" ,out Listdata);
                                Opcua.Disconnect();
                            }
                        });
                }
            }






            Console.WriteLine("Press any key");
            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();







        }
    }
}
