﻿using client;
using WeaveBase;
using System;
using System.Windows.Forms;
namespace test2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int count = 0;
        private void Form1_Load(object sender, EventArgs e)
        {
          
          
        }
        [InstallFunAttribute("forever")]//客户端也支持像服务端那样写，刚才看懂返回的内容也是testaabb，所以客户端也要把方法命名testaabb
        public void login(System.Net.Sockets.Socket soc, WeaveBase.WeaveSession _0x01)
        {
           // MessageBox.Show(_0x01.GetRoot<int>().ToString());
            //  Gw_EventMylog("",_0x01.Getjson());
        }
        private void P2pc_timeoutevent()
        {
            
        }
        private void P2pc_receiveServerEvent(byte command, string text)
        {
            count++;
            MessageBox.Show(text);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //在加个发送
            
            //这样就可以了，我们试试
        }

        private void button2_Click(object sender, EventArgs e)
        {
         

            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(tt));

            t.Start();
            timer1.Start();
        }

        void tt()
        {
            for (int i = 0; i < 500; i++)
            {
                P2Pclient p2pc = new P2Pclient(false);
                p2pc.receiveServerEvent += P2pc_receiveServerEvent;//接收数据事件
                p2pc.timeoutevent += P2pc_timeoutevent;//超时（掉线）事件
                p2pc.start("122.114.53.233", 18989, false);//11002 是网关的端口号，刚才WEB网关占用了11001，我改成11002了
                p2pc.Tokan = "123";
                p2pc.SendRoot<int>(0x01, "login", 99987, 0);
                System.Threading.Thread.Sleep(5);
            }

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Invoke((EventHandler)delegate { label1.Text = count.ToString(); });
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            P2Pclient p2pc = new P2Pclient(false);
            p2pc.receiveServerEvent += P2pc_receiveServerEvent;//接收数据事件
            p2pc.timeoutevent += P2pc_timeoutevent;//超时（掉线）事件
            p2pc.start("122.114.53.233", 18989, false);//11002 是网关的端口号，刚才WEB网关占用了11001，我改成11002了
            p2pc.Tokan = "123";
            p2pc.SendRoot<int>(0x01, "login", 99987, 0);
            System.Threading.Thread.Sleep(5);
        }
    }
}
