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


        // -> GameRoom update -> main thread 
        // 무조건 GameRoom의 tick을 따라갈 필요는 없다,
        public override void Update()
        {
            // TODO
            // Data는 상위 class인 Projectile이 가지고 기술에 대한 있는 정보
            if (Data == null || Data.projectile == null || Owner == null || Room == null) return;

            if (_nextMoveTick >= Environment.TickCount64) return;

            // speed는 1초에 움직일 수 있는 칸의 개수 1초 = 1000ms
            // 서버 쪽에서 움직이는 시간은 데이터를 기반으로 움직임
            long tick = (long)(1000 / Data.projectile.speed);
            _nextMoveTick = Environment.TickCount64 + tick;

            // 앞으로 이동하는 연산
            Vector2Int destPos = GetFrontCellPos();
            if(Room.Map.CanGo(destPos))
            {
                CellPos = destPos;

                S_Move movePacket = new S_Move();
                movePacket.ObjectId = Id;
                movePacket.PosInfo = PosInfo;
                Room.Broadcast(movePacket);

                Console.WriteLine("move arrow");
            }
            else // 갈 수 없다면
            {
                GameObject target = Room.Map.Find(destPos);
                if(target != null)
                {
                    // TODO 피격 판정
                    // OnDamaged에서 Owner를 찾아도 되고, 아니면 owner를 직접넘겨줘도 됨
                    // 최종 데미지 : 화살의 damage + player의 attack
                    target.OnDamaged(this, Data.damage + Owner.Stat.Attack);
                }

                // 소멸, type 추출후, 기억에서 삭제 한다음, despawn packet 보냄
                Room.LeaveGame(Id);
            }
        }
    }
}
