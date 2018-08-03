﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Weave.Base;

using Weave.Base.Interface;
namespace Weave.Server
{

    /// <summary>
    /// HTTP服务器类，继承自IWeaveTcpBase接口
    /// </summary>
    public class HttpServer : IWeaveTcpBase
    {
        TcpListener listener;
        bool is_active = true;
       protected List<HttpProcessor> httpProcessorList = new List<HttpProcessor>();
        public event WaveReceiveEventEvent waveReceiveEvent;
        public event WeaveReceiveBitEvent weaveReceiveBitEvent;
      
        public event WeaveUpdateSocketListEvent weaveUpdateSocketListEvent;
        public event WeaveDeleteSocketListEvent weaveDeleteSocketListEvent;
        public int Port
        { get; set; }
        public HttpServer(int port)
        {
            Port = port;
        }
        public void Start(int port)
        {
            Port = port;
            acallsend = new AsyncCallback(SendDataEnd);
            Thread thread = new Thread(new ThreadStart(listen));
            thread.Start();
            //Thread thread2 = new Thread(new ThreadStart(process));
            //thread2.Start();
        }
        public int GetNetworkItemCount()
        {
            return httpProcessorList.Count;
        }
        public void KeepAliveHander(object obj)
        {
        }
        void process()
        {

            while (true)
            {
                int i = httpProcessorList.Count;
                if (i > 0)
                {
                    HttpProcessor[] hps = new HttpProcessor[i];
                    //Array.Copy(httpProcessorList, hps, i);
                    httpProcessorList.CopyTo(0, hps, 0, i);
                    // httpProcessorList.CopyTo(hps);
                    foreach (HttpProcessor hp in hps)
                    {
                        try
                        {
                            if((DateTime.Now- hp.updatetime).TotalSeconds>45) 
                              httpProcessorList.Remove(hp);
                        }
                        catch { }
                    }
                }
                Thread.Sleep(10);
            }

        }
        private void SendDataEnd(IAsyncResult ar)
        {
            try
            {
                ((Socket)ar.AsyncState).EndSend(ar);
            }
            catch
            {

            }


        }
        AsyncCallback acallsend;
        public bool Send(Socket soc, byte command, string text)
        {
            try
            {
                 
                byte[] b = System.Text.Encoding.UTF8.GetBytes(text);
                soc.Send(b, 0, b.Length, SocketFlags.None);
           
            }
            catch { return false; }
            return false;
        }
        public bool Send(Socket soc, byte command, byte[] data)
        {
            try
            {

                byte[] b = data;
                soc.Send(b, 0, b.Length, SocketFlags.None);

            }
            catch { return false; }
            return false;
        }
        void listen()
        {
            listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            while (is_active)
            {
                try
                {
                    TcpClient s = listener.AcceptTcpClient();
                    HttpProcessor processor = new HttpProcessor(s, this);
                    processor.updatetime = DateTime.Now;
                   // httpProcessorList.Add(processor);
                    System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(processor.process));
                    Thread.Sleep(1);
                }
                catch
                { }
            }
        }


        /// <summary>
        /// 处理Get请求的方法，是一个虚方法，写有具体的代码的
        /// </summary>
        /// <param name="p"></param>
        public virtual void handleGETRequest(HttpProcessor p)
        {
            p.http_url = p.http_url.Substring(1);
           // string fun = p.http_url.Split('?')[1].Split('=')[0];
            if (p.http_url == "")
                return;
            string fun = p.http_url.Split('?')[1].Split('=')[0];
            byte command = Convert.ToByte(p.http_url.Substring(0, 1), 16);
            string data = p.http_url.Split('&')[1];
            p.writeSuccess();
            p.outputStream.WriteLine(fun + "(");
            p.outputStream.Flush();
            getdata(p, command, System.Web.HttpUtility.UrlDecode(data));
            p.outputStream.WriteLine(")");
            p.outputStream.Flush();
        }

        /// <summary>
        /// 处理Post请求的方法，是一个虚方法，写有具体的代码的
        /// </summary>
        /// <param name="p"></param>
        /// <param name="inputData"></param>
        public virtual void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            p.http_url = p.http_url.Substring(1);
            if (p.http_url == "")
                return;
            byte command = Convert.ToByte(p.http_url, 16);
            string data = inputData.ReadToEnd();
            p.writeSuccess();
            p.outputStream.Flush();
            getdata(p, command, data);
            p.outputStream.Flush();

        }

        /// <summary>
        /// 是否获取到了数据的方法，，从某个HttpProcessor连接里面获取数据，根据命令command
        /// </summary>
        /// <param name="p"></param>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool getdata(HttpProcessor p, byte command, string data)
        {
             
                      waveReceiveEvent?.Invoke(command,data, p.socket.Client);
                      weaveReceiveBitEvent?.Invoke(command,  Convert.FromBase64String(data), p.socket.Client);
                   
          //  p.updatetime =Convert.ToDateTime( "0000-01-01 00:00:00");
            return true;
        }
    }
        //public class TestMain
        //{
        //    public static int Main(String[] args)
        //    {
        //        HttpServer httpServer;
        //        if (args.GetLength(0) > 0)
        //        {
        //            httpServer = new MyHttpServer(Convert.ToInt16(args[0]));
        //        }
        //        else
        //        {
        //            httpServer = new MyHttpServer(8080);
        //        }
        //        Thread thread = new Thread(new ThreadStart(httpServer.listen));
        //        thread.Start();
        //        return 0;
        //    }
        //}
}
