using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using Server.Game;

namespace Server
{
    public class ClientSession : PacketSession
    {
        public Player MyPlayer { get; set; }
        public int SessionId { get; set; } // Session을 구분하기 위한 id

        // size와 protocol id를 wrapping 해서 보내는 함수
        public void Send(IMessage packet)
        {
            // 안에 있는건 대문자로 하는 것이 convention이긴 한데 이름이 다음과 같이 변함
            // MsgId.SChat
            // 이름을 이용해서 찾아줌 (자동화)
            string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
            // 이름이 없으면 정상적으로 parsing이 안되면서 crash가 날 수 있음. exception 해도 됨
            // Reflection 이용
            MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);

            // 이렇게 만들어줘야지 Session에서 사용할 수 있다.
            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            // 내부에서 byte배열을 한번 더 할당하지만 비트연산을 이용해서 직접넣는 방법이 있다.
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

            Send(new ArraySegment<byte>(sendBuffer));
        }

        // 엔진과 컨텐츠 분리
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine("OnConnected : {0}", endPoint);
            // client가 접속 했을 때 바로 입장시키지 않고 client쪽으로 로드해야할 정보들을 보내준다음
            // client쪽에서 모든 resource를 load 했을 때 ok packet을 보내면 방에 입장

            // DB에서 player 정보를 받아와서 client에 보내주는 작업이 필요

            // Player를 만든다.
            MyPlayer = PlayerManager.Instance.Add();
            {
                // 기본값이라 값이 안온것 같을 때는 Client쪽에서도 기본값으로 다시 초기화해준다.
                MyPlayer.Info.Name = $"Player_{MyPlayer.Info.PlayerId}";
                MyPlayer.Info.PosInfo.State = CreatureState.Idle;
                MyPlayer.Info.PosInfo.MoveDir = MoveDir.None;
                MyPlayer.Info.PosInfo.PosX = 0;
                MyPlayer.Info.PosInfo.PosY = 0;

                MyPlayer.Session = this;
            }

            // Room의 player목록에 추가, S_EnterGame, S_Spawn 보냄
            RoomManager.Instance.Find(1).EnterGame(MyPlayer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            // 방에서 자신을 제외시키고 S_LeaveGame S_Despawn 보냄
            RoomManager.Instance.Find(1).LeaveGame(MyPlayer.Info.PlayerId);

            SessionManager.Instance.Remove(this);

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
            Console.WriteLine($"Transferred bytes : {numofBytes}");
        }
    }
}
