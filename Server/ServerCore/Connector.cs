using System.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

// 연결하는 애가 필요한 이유
// 1. 연결하는 부분과 recieve & send 부분을 공용으로 사용하면 좋기 때문
// 2. 서버에 컨텐츠를 올릴때 서버를 하나짜리로 만들것인지 분산처리할것인지, 한서버가 다른 서버와 통신하기 위해서
// 한쪽은 Listener역할을 하고 다른 한쪽은 Connector 역할을 한다.
// 사실상 다른 server에 대한 client 역할을 한다고 보면 된다.

namespace ServerCore
{
    public class Connector
    {
        // 이런식으로 하지 않는 이유 : 경우에 따라서는 여러명을 받을 수 있으므로
        // Socket _socket; 
        Func<Session> _sesionFactory;

        // count 옵션을 넣어서 여러개를 만들도록 추가, 다수의 client를 test하고 싶음
        public void Connect(IPEndPoint endPoint, Func<Session> sesionFactory, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _sesionFactory = sesionFactory;

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnConnectCompleted;
                args.RemoteEndPoint = endPoint;
                args.UserToken = socket;

                RegisterConnect(args);
            }
        }

        void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket;
            if (socket == null) return;

            bool pending = socket.ConnectAsync(args);
            if (pending == false) OnConnectCompleted(null, args); 
        }

        // Session을 생성하고 OnConnected 호출
        void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                Session session = _sesionFactory.Invoke();
                // args.ConnectSocket 대신에 userToken에서 넘겨준것을 받아도 됨
                session.Start(args.ConnectSocket);
                session.OnConnected(args.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine($"OnConnectedCompleted Fail! {args.SocketError}");
            }
        }
    }
}