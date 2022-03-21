using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Game;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();
        // GameRoom도 나중에는 하나가 아니라 RoomManger가 있어야한다.
        // public static GameRoom Room = new GameRoom();

        static void FlushRoom()
        {
            JobTimer.Instance.Push(FlushRoom, 250); //  다시 예약
        }

        static void Main(string[] args)
        {
            // 1번방 생성, 나중에는 데이터로 빼서 시작지역을 정해줌.
            // 지금은 1번방만 사용할 것이다.
            RoomManager.Instance.Add();

            string host = Dns.GetHostName(); // local 컴퓨터의 host이름
            IPHostEntry ipHost = Dns.GetHostEntry(host); // 네트워크망 안에 있는 DNS 서버가 해줌
            // 배열로 되어있는 이유, 트래픽이 많을 경우 ip를 여러개 사용하여 부하 분산
            IPAddress ipAddr = ipHost.AddressList[0];
            // 최종 주소, client가 엉뚱한 port 번호로 접속을 하려면 입장 못함
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); 

            // 개선을 하자면 중간에 SessionManager를 둬서 만들어주도록한다. session id나 count 관리
            // Manager는 core에 들어가도되고 컨텐츠단에서 관리해도 됨
            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");

            //FlushRoom(); // 예약
            JobTimer.Instance.Push(FlushRoom);
            // 다른 thread에서 JobTimer라는 중앙관리 시스템에 일감을 던져서 예약하게될것

            while(true)
            {
                // 예약된거 처리
                JobTimer.Instance.Flush();
            }
        }
    }
}
