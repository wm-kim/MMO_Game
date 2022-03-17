using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    // 모든 정보를 clientsession에 넣지 않고, 새로운 class 예를 들면 player class를 판다음에 
    // 거기다 컨텐츠 코드를 넣어넣고 player가 연결된 client sessions을 물고있게끔 만들어줌
    // 일단은 간단하게 하기 위해서 여기다 모든 정보를 넣어둠
    class ClientSession : PacketSession
    {
       
        public int SessionId { get; set; } // Session을 구분하기 위한 id
        public GameRoom Room { get; set; }  // client session에서 내가 어떤 방에 있는지 궁금할 수 있으니까
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }


        // 엔진과 컨텐츠 분리
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine("OnConnected : {0}", endPoint);
            // client가 접속 했을 때 바로 입장시키지 않고 client쪽으로 승인을 보내고
            // client쪽에서 모든 resource를 load 했을 때 ok packet을 보내면 방에 입장

            // Client Session의 Room이 아닌 Program 전역에 있는 Room을 이용하므로 여기서는 문제 없음
            Program.Room.Push(() => Program.Room.Enter(this));
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            if(Room != null)
            {
                // 한번더 null check를 하던가 Room을 꺼내서 사용한다.
                // Room은 null로 없어지더라도 실제 지역 객체 room은 계속 존재하고 있다.
                GameRoom room = Room;
                room.Push(() => room.Leave(this));

                // 사실 문제 있는 코드. 일감이 뒤로 밀린 상태에서
                // client가 접속을 끊으면 Room을 찾지 못한 상태에서 crash가 난다.

                // Room.Push(() => Room.Leave(this));

                Room = null; // 혹시라도 2번 호출할까봐 null로 밀어줌
            }

            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            // singleton이 아니라 handler 방식으로 등록해서 호출하는 방식으로해도 상관없다.
            // PacketManager에서 packet을 deserialize하고 handler를 호출한다.
            // 인자로 handler callback함수를 받는다. 명시하지 않으면 처음 register한 것이 들어감
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numofBytes)
        {
            // Console.WriteLine($"Transferred bytes : {numofBytes}");
        }
    }
}
