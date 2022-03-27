using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Monster : GameObject
    {
        public Monster()
        {
            ObjectType = GameObjectType.Monster;

            // TEMP hard coding (monster stat)
            Stat.Level = 1;
            Stat.Hp = 100;
            Stat.MaxHp = 100;
            Stat.Speed = 5.0f;

            State = CreatureState.Idle;
        }

        // FSM - 서버쪽에서는 co
        public override void Update()
        {
            switch (State)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
                case CreatureState.Moving:
                    UpdateMoving();
                    break;
                case CreatureState.Skill:
                    UpdateSkill();
                    break;
                case CreatureState.Dead:
                    UpdateDead();
                    break;
            }
        }

        // target을 참조값으로 들고 있다.
        // Player가 나갔다고 한다면 _target을 더이상 이용하면 안되지만
        // 시점이 오묘하게 겹치면 _target이 나갔음에도 접근을 해서 공격을 하거나 할 수 있다.
        Player _target;
        int _searchCellDist = 10;
        int _chaseCellDist = 20;

        long _nextSearchTick = 0;
        protected virtual void UpdateIdle()
        {
            if (_nextSearchTick > Environment.TickCount64) return;
            _nextSearchTick = Environment.TickCount64 + 1000;

            // 주변에 플레이어가 있는지 찾고 싶다
            Player target = Room.FindPlayer(p =>
            {
                Vector2Int dir = p.CellPos - CellPos;
                return dir.cellDistFromZero <= _searchCellDist;
            });

            if (target == null) return;

            _target = target;
            State = CreatureState.Moving;
        }

        // 나중에 skill이 여러개 있을텐데 지금은 평타하나만 있다고 가정
        int _skillRange = 1;
        long _nextMoveTick = 0;

        protected virtual void UpdateMoving()
        {
            if (_nextMoveTick > Environment.TickCount64) return;
            int moveTick = (int)(1000 / Speed);
            _nextMoveTick = Environment.TickCount64 + moveTick;

            // 내가 쫒고 있는 player가 나가거나 다른 지역으로 이동한다면
            if (_target == null || _target.Room != Room)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            Vector2Int dir = _target.CellPos - CellPos;
            int dist = dir.cellDistFromZero;
            // player가 너무 빠르게 도망감
            if(dist == 0 || dist > _chaseCellDist)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadcastMove(); // idle 상태로 변경되었다는걸 알림
                return;
            }

            List<Vector2Int> path = Room.Map.FindPath(CellPos, _target.CellPos, checkObjects: false);
            if (path.Count < 2 || path.Count > _chaseCellDist)
            {
                _target = null;
                State = CreatureState.Idle;
                BroadcastMove(); // idle 상태로 변경되었다는걸 알림
                return;
            }

            // 스킬로 넘어갈지 확인 지금은 근접 공격만 하도록
            if(dist <= _skillRange && (dir.x == 0 || dir.y == 0))
            {
                _coolTick = 0;
                State = CreatureState.Skill;
                return;
            }

            // 이동
            Dir = GetDirFromVec(path[1] - CellPos);
            Room.Map.ApplyMove(this, path[1]);
            BroadcastMove();
        }

        void BroadcastMove()
        {
            // 다른 플레이어에게도 알린다.
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            Room.Broadcast(movePacket);
        }

        long _coolTick = 0;
        protected virtual void UpdateSkill()
        {
            if(_coolTick == 0)
            {
                // 유효한 타겟인지
                if(_target == null || _target.Room != Room || _target.Hp == 0)
                {
                    _target = null;
                    State = CreatureState.Moving;
                    // 더 깔끔하게 하려면 다른 packet을 파도됨
                    BroadcastMove();
                    return;
                }

                // 스킬이 아직 유효한지
                Vector2Int dir = (_target.CellPos - CellPos);
                int dist = dir.cellDistFromZero;
                bool canUseSkill = (dist <= _skillRange && (dir.x == 0 || dir.y == 0));
                if(canUseSkill == false)
                {
                    // 한번 주시한것을 계속 쫒아갈 것
                    // _target = null; UpdateMoving에서 처리해줄것
                    State = CreatureState.Moving;
                    BroadcastMove();
                    return;
                }

                // 타겟팅 방향 주시
                MoveDir lookDir = GetDirFromVec(dir);
                if(Dir != lookDir)
                {
                    Dir = lookDir;
                    BroadcastMove();
                }

                Skill skillData = null;
                // skillData의 
                DataManager.SkillDict.TryGetValue(1, out skillData);

                // 데미지 판정
                // 체력을 깎은 후에 S_ChangeHp 패킷을 broadcasting, 죽었다면 S_Die 
                _target.OnDamaged(this, skillData.damage + Stat.Attack);

                // 스킬 사용 Broadcast
                S_Skill skill = new S_Skill { Info = new SkillInfo() };
                skill.ObjectId = Id;
                skill.Info.SkillId = skillData.id;
                Room.Broadcast(skill);

                // 스킬 쿨타임 적용
                int coolTick = (int)(1000 * skillData.cooldown);
                _coolTick = Environment.TickCount64 + coolTick;
            }

            if (_coolTick > Environment.TickCount64) return;

            // skill을 사용할 준비가됨
            _coolTick = 0;
        }

        protected virtual void UpdateDead()
        {

        }
    }
}
