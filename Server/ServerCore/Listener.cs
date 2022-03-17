using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    public class Listener
    {
        
        Socket _listenSocket;
        Func<Session> _sessionFactory; // 어떤 Session을 만들어줄지 정의

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
            // AddressFamily : IPV4 or IPV6를 사용할건지. 자동으로 만들어준것을 사용
            // TCP 사용
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            // 문지기 교육
            _listenSocket.Bind(endPoint);

            // 영업 시작
            // backlog : 최대 대기수, 갑자기 동시다발적으로 접속했을 때 몇명까지 대기할 수 있는지
            _listenSocket.Listen(backlog);

            // register : 문지기 명 수
            // 한번만 만들면 재사용 가능, 동시다발적으로 많은 유저를 받아야할 때 이부분을 for문을 걸어 늘려준다.
            for (int i = 0; i < register; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                RegisterAccept(args);
            }
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            // SocketAsyncEventArgs 초기화, 기존의 잔재를 없앰
            args.AcceptSocket = null;

            bool pending = _listenSocket.AcceptAsync(args);
            if (pending == false) // 동시다발적으로 계속 false만 나오는 경우는 거의 없다.
                OnAcceptCompleted(null, args);
        }

        // 콜백함수는 별도의 Thread를 이용, ThreadPool에서 뽑아서 실행 main과 race condition이 일어나지 않도록 주의
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                // socket을 넘겨줌
                session.Start(args.AcceptSocket);
                // 여기서 다음으로 넘어가는 순간에 client에서 연결을 끊어버리면 밑에 라인에서 에러가난다.
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
                // _onAcceptHandler.Invoke(args.AcceptSocket);
            }
            else 
                Console.WriteLine(args.SocketError.ToString());

            // stackoverflow가 일어나지 않는다.
            RegisterAccept(args);
        }
    }
}
