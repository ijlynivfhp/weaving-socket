﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Weave.TCPClient;

namespace ConsoleApp1
{
    class Program
    {
     static   List<modbus> modlist = new List<modbus>();
        static void Main(string[] args)
        {
         
            modbus mod = new modbus();
            mod.name = "aaa";//这里可以是设备的ID号;
            mod.ip = "117.159.23.98";
            mod.port = 3308;
            modlist.Add(mod);
            foreach (modbus m in modlist)
            {
                P2Pclient client = new P2Pclient(DataType.custom);
                client.ReceiveServerEventbitobj += Client_receiveServerEventbitobj; ;
                client.Timeoutobjevent += Client_timeoutobjevent;
                if ( client.Start(m.ip, m.port, false))
                m.client = client;
            }
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(gogo));
            t.Start();

        }

        private static void Client_receiveServerEventbitobj(byte command, byte[] data, P2Pclient soc)
        {
           
        }

        static void gogo()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                foreach (modbus m in modlist)
                {
                    //发消息
                    m.client.Send(new byte[] { 0, 1, 2, 3, 4, 5 });
                }
            }
        }

        private static void Client_timeoutobjevent(P2Pclient p2pobj)
        {
           //断线重连
        }

        private static void Client_receiveServerEventbit(byte command, byte[] data)
        {
            //通过收到的数据的ID，判断回发给哪个modbus；
        }
    }
    class modbus
    {
      public  string name;
        public string ip;
        public int port;
        public P2Pclient client;
    }
}
