using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using Google.Protobuf.Protocol;
using static Google.Protobuf.Protocol.Person.Types;
using Google.Protobuf;

namespace Server
{
    // 모든 정보를 clientsession에 넣지 않고, 새로운 class 예를 들면 player class를 판다음에 
    // 거기다 컨텐츠 코드를 넣어넣고 player가 연결된 client sessions을 물고있게끔 만들어줌
    // 일단은 간단하게 하기 위해서 여기다 모든 정보를 넣어둠
    class ClientSession : PacketSession
    {
       
        public int SessionId { get; set; } // Session을 구분하기 위한 id

        // 엔진과 컨텐츠 분리
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine("OnConnected : {0}", endPoint);
            // client가 접속 했을 때 바로 입장시키지 않고 client쪽으로 승인을 보내고
            // client쪽에서 모든 resource를 load 했을 때 ok packet을 보내면 방에 입장

            // protoTest
            S_Chat chat = new S_Chat()
            {
                Context = "안녕하세요"
            };

            // 안에 있는건 대문자로 하는 것이 convention이긴 한데 이름이 다음과 같이 변함
            // MsgId.SChat

            // 이렇게 만들어줘야지 Session에서 사용할 수 있다.
            ushort size = (ushort)chat.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            // 내부에서 byte배열을 한번 더 할당하지만 비트연산을 이용해서 직접넣는 방법이 있다.
            Array.Copy(BitConverter.GetBytes(size + 4), 0, sendBuffer, 0, sizeof(ushort));
            ushort protocolId = (ushort)MsgId.SChat;
            Array.Copy(BitConverter.GetBytes(protocolId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(chat.ToByteArray(), 0, sendBuffer, 4, size);

            Send(new ArraySegment<byte>(sendBuffer));
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);

            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            // singleton이 아니라 handler 방식으로 등록해서 호출하는 방식으로해도 상관없다.
            // PacketManager에서 packet을 deserialize하고 handler를 호출한다.
            // 인자로 handler callback함수를 받는다. 명시하지 않으면 처음 register한 것이 들어감
            // PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numofBytes)
        {
            Console.WriteLine($"Transferred bytes : {numofBytes}");
        }
    }
}
