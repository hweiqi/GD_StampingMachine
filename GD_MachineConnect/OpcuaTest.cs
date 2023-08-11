﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_MachineConnect
{
    public class OpcuaTest
    {
        public void TestOpen()
        {
            string HostString = "127.0.0.1";
            int Port = 62541;
            string ServerDataPath = "SharpNodeSettings/OpcUaServer";

            var BaseUrl = new Uri($"opc.tcp://{HostString}:{Port}");
            var CombineUrl = new Uri(BaseUrl, ServerDataPath);
            var ServerUrl = CombineUrl.ToString();
            OpcUaHelper.Forms.FormBrowseServer formBrowseServer = new OpcUaHelper.Forms.FormBrowseServer(ServerUrl);
            formBrowseServer.ShowDialog();
        }
    }
}
