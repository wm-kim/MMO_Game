using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Arrow : Projectile
    {
        // 피아 식별을 위해서, 보통 주인의 stat을 보고 dmg를 결정하는 경우도 많다
        public GameObject Owner { get; set; }

        long _nextMoveTick = 0;

        // 무조건 GameRoom의 tick을 따라갈 필요는 없겠다.
        public override void Update()
        {
            // TODO
            if (Owner == null || Room == null) return;

            if (_nextMoveTick >= Environment.TickCount64) return;

            _nextMoveTick = Environment.TickCount64 + 50;

            // 앞으로 이동하는 연산
            Vector2Int destPos = GetFrontCellPos();
            if(Room.Map.CanGo(destPos))
            {
                CellPos = destPos;

                S_Move movePacket = new S_Move();
                movePacket.ObjectId = Id;
                movePacket.PosInfo = PosInfo;
                Room.BroadCast(movePacket);

                Console.WriteLine("move arrow");
            }
            else 
            {
                GameObject target = Room.Map.Find(destPos);
                if(target != null)
                {
                    // TODO 피격 판정
                }

                // 소멸, type 추출후, 기억에서 삭제 한다음, despawn packet 보냄
                Room.LeaveGame(Id);
            }
        }
    }
}
