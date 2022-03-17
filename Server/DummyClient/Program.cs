﻿using ServerCore;
using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading; 

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();
            // client 몇개를 simulate 할건지 조절, 다수의 client를 test하고 싶음
            // Generate 하는 동시에 SessionManager의 List에 들어감
            connector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); }, 10);

            while(true)
            {
                try
                {
                    // 생성해준 각 Session에서 packet을 보냄
                    SessionManager.Instance.SendForEach();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                // MMO를 만들때 이동패킷을 보통 1초에 4번정도 보내기 때문에
                Thread.Sleep(250);
            }
        }
    }
}
