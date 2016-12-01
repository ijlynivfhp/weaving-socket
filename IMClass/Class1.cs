﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using StandardModel;
using MyInterface;
using Newtonsoft.Json;

namespace IMClass
{

    class User
    {
       public string token;
        [JsonIgnore]
        public Socket soc;
        public string name;
    }
    class say
    {
        public string token;
        public string context;
        public DateTime time;
        public string name;
    }
    public class im : MyInterface.TCPCommand
    {
        public override void Bm_errorMessageEvent(Socket soc, _baseModel _0x01, string message)
        {
             
        }

        public override byte Getcommand()
        {
            return 0x31;
        }
        List<User> listsoc = new List<User>();
        public override bool Run(string data, Socket soc)
        {
            return true;
        }
        [InstallFun("forever")]
        public void login(Socket soc, _baseModel _0x01)
        {
            online [] onlieuser= GetOnline();
            User[] listsoctemp = new User[listsoc.Count];
            listsoc.CopyTo(0, listsoctemp, 0, listsoctemp.Length);
            //为什么写这两句，是因为多线程中，添加和删除集合的操作，都会对其他线程有影响，所以先
            //拷贝一份副本
            foreach (User u in listsoctemp)
            {
                if(u.token==_0x01.Token)
                 return;
            }
            String Token = _0x01.Token;//这个就是上线人员的Token了 
            User du = new User();
            du.soc = soc;
            du.token = Token;
            du.name = _0x01.Root;
            listsoc.Add(du);
            //发送回执
            SendRoot<String>(soc, 0x31, "login", "ok", 0, _0x01.Token);

            SendRoot<List<User>>(soc, 0x31, "userlist", listsoc, 0, _0x01.Token);
            foreach (User u in listsoctemp)
            {
                try
                {
                    SendRoot<List<User>>(u.soc, 0x31, "userlist", listsoc, 0, u.token);
                }
                catch { }
            }
         
        }
        [InstallFun("forever")]
        public void say(Socket soc, _baseModel _0x01)
        {

            User[] listsoctemp = new User[listsoc.Count];
            listsoc.CopyTo(0, listsoctemp, 0, listsoctemp.Length);
            say s=_0x01.GetRoot<say>();
            s.time = DateTime.Now;
            //为什么写这两句，是因为多线程中，添加和删除集合的操作，都会对其他线程有影响，所以先
            //拷贝一份副本
            foreach (User du in listsoctemp)
            {
                if(du!=null)
                try { SendRoot<say>(du.soc, 0x31, "say", s, 0, du.token);  } catch { }
            }
        }
        /// <summary>
        /// 新增重写的方法，连接离线后激活的方法
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="soc"></param>
        public override void Tokenout(string Token, Socket soc)
        { }
        /// <summary>
        /// 新增重写的方法，连接上线后激活的方法
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="soc"></param>
        public override void Tokenin(string Token, Socket soc)
        { }
        public override void Runcommand(byte command, string data, Socket soc)
        {

            string[] temp = data.Split('|');
            if (temp[0] == "in")//in是上线，out是下线
            {

            }
            else if (temp[0] == "out")
            {
                String Token = temp[1];//这个就是下线人员的Token了
                User[] listsoctemp = new User[listsoc.Count];
                listsoc.CopyTo(0, listsoctemp, 0, listsoctemp.Length);
                //为什么写这两句，是因为多线程中，添加和删除集合的操作，都会对其他线程有影响，所以先
                //拷贝一份副本
                foreach (User du in listsoctemp)
                {
                    if (Token == du.token)
                    {

                        User[] listsoctemp2 = new User[listsoc.Count];
                        listsoc.CopyTo(0, listsoctemp2, 0, listsoctemp2.Length);
                        foreach (User u in listsoctemp2)
                        {
                            if (u.token != Token)
                                try
                                {
                                    SendRoot<User>(u.soc, 0x31, "logout", du, 0, u.token);
                                }
                                catch { }
                        }
                        listsoc.Remove(du);
                        return;
                    }

                }

            }
        }
        public override void TCPCommand_EventDeleteConnSoc(Socket soc)
        {
            
        }

        public override void TCPCommand_EventUpdataConnSoc(Socket soc)
        {
             
        }
    }
}