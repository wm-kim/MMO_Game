using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class GameRoom
    {
        object _lock = new object();
        public int RoomId { get; set; }

        // List<Player> _players = new List<Player>();
        // Dictinary를 이용해 id로 player를 빠르게 찾을 수 있게 함, Grid 단위로 player를 들고 있게 해도됨
        // playerId는 PlayerManager에서 발급
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
        Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();
        // type 별로 dictionary를 들고 있을 수도 있고 하나의 dictionary로 관리할 수 도 있다. (장단점이 있음)


        // 내가 소속되어있는 map, 좌표를 기준으로 player가 있는지 없는지 찾고 싶음 -> map 안에서 관리
        public Map Map { get; private set; } = new Map();

        public void Init(int mapId)
        {
            // mapId에 따른 경로도 나중에 공식화해서 지정이 될 것
            Map.LoadMap(mapId);

            // TEMP Stat값은 Monster 생성자에서 값을 초기화 해주고 있다.
            Monster monster = ObjectManager.Instance.Add<Monster>();
            monster.CellPos = new Vector2Int(5, 5);
            EnterGame(monster);
        }

        // GameRoom을 몇 tick 단위로 업데이트 
        public void Update()
        {
            lock (_lock)
            {
                foreach(Monster monster in _monsters.Values)
                {
                    monster.Update();
                }

                foreach (Projectile projectile in _projectiles.Values)
                {
                    projectile.Update();
                }
            }
        }

        // 인자로 접속한 player의 정보 전달
        // 이제는 중의적으로 Object 입장. arrow나 monster...
        public void EnterGame(GameObject gameObject)
        {
            if (gameObject == null) return;

            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

            lock(_lock)
            {
                if(type == GameObjectType.Player)
                {
                    Player player = gameObject as Player;

                    _players.Add(gameObject.Id, player);
                    player.Room = this;

                    Map.ApplyMove(player, new Vector2Int(player.CellPos.x, player.CellPos.y));

                    // 본인한테 정보 전송
                    {
                        S_EnterGame enterPacket = new S_EnterGame();
                        enterPacket.Player = player.Info;
                        player.Session.Send(enterPacket);

                        // 나한테 나머지 player에 대한 정보를 보내줌
                        S_Spawn spawnPacket = new S_Spawn();
                        foreach (Player p in _players.Values)
                        {
                            if (player != p) spawnPacket.Objects.Add(p.Info);
                        }

                        foreach (Monster m in _monsters.Values)
                            spawnPacket.Objects.Add(m.Info);

                        foreach (Projectile p in _projectiles.Values)
                            spawnPacket.Objects.Add(p.Info);

                        player.Session.Send(spawnPacket);
                        // 지금은 contents code와 network로 보내는 코드 같이 묶여있음
                    }
                }
                else if (type == GameObjectType.Monster)
                {
                    Monster monster = gameObject as Monster;
                    _monsters.Add(gameObject.Id, monster);
                    monster.Room = this;

                    Map.ApplyMove(monster, new Vector2Int(monster.CellPos.x, monster.CellPos.y));
                }
                else if(type == GameObjectType.Projectile)
                {
                    Projectile projectile = gameObject as Projectile;
                    _projectiles.Add(gameObject.Id, projectile);
                    projectile.Room = this;
                }
                // 타인한테 정보 전송
                {
                    S_Spawn spawnPacket = new S_Spawn();
                    spawnPacket.Objects.Add(gameObject.Info);
                    foreach (Player p in _players.Values)
                    {
                        // 본인의 정보는 S_EnterGame에서 받기 때문
                        if (p.Id != gameObject.Id) p.Session.Send(spawnPacket);
                    }
                }
            }
        }

        // player가 나갈 때 뿐만 아니라 Object가 despawn할 때
        // 화살의 경우 update에서 tick 마다 확인해서 갈 수 없다면 이 method 호출
        public void LeaveGame(int objectId)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

            lock (_lock)
            {
                if(type == GameObjectType.Player)
                {
                    Player player = null;
                    if (_players.Remove(objectId, out player) == false)
                        return;

                    player.Room = null;
                    Map.ApplyLeave(player);

                    // 본인한테 정보 전송
                    {
                        S_LeaveGame leavePacket = new S_LeaveGame();
                        player.Session.Send(leavePacket);
                    }
                }
                else if (type == GameObjectType.Monster)
                {
                    Monster monster = null;
                    if (_monsters.Remove(objectId, out monster) == false) return;

                    monster.Room = null;
                    Map.ApplyLeave(monster);
                }
                else if (type == GameObjectType.Projectile)
                {
                    Projectile projectile = null;
                    if (_projectiles.Remove(objectId, out projectile) == false) return;
                    projectile.Room = null;
                    // 화살은 충돌 대상이 아니기 때문에 _map.ApplyLeave 하지 않아도됨
                }


                // 타인에게 정보 전송, 물체 사라졌다고 알리는 부분
                {
                    S_Despawn despawnPacket = new S_Despawn();
                    despawnPacket.ObjectIds.Add(objectId);
                    foreach(Player p in _players.Values)
                    {
                        if(p.Id != objectId) p.Session.Send(despawnPacket);
                    }
                }
            }
        }

        // Server안에서 이동시켜주는 logic
        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null) return;

            lock (_lock)
            {
                // TODO : 검증 client를 신용할 수 없다.
                PositionInfo movePosInfo = movePacket.PosInfo;
                ObjectInfo info = player.Info;

                // 다른 좌표로 이동할 경우 갈 수 있는지 확인, 해킹 방지
                // 좌표는 이동안했는데 상태가 idle로 변경할 때도 사용하기 때문에 그럴때는 통과시켜줘야함
                if (movePosInfo.PosX != info.PosInfo.PosX || movePosInfo.PosY != info.PosInfo.PosY)
                {
                    if (Map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
                        return;
                }

                // 검증 완료
                info.PosInfo.State = movePosInfo.State;
                info.PosInfo.MoveDir = movePosInfo.MoveDir;
                // 좌표 이동하는 부분이 gameRoom에서가 아니라 map에서 하는 이유, player 2차배열 변경을 위해서
                // 왜 ApplyMove안에서는 lock을 사용하지 않는가? - 상위단계인 gameRoom에서 이미 lock을 잡고 있으므로
                Map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));

                // 다른 플레이어한테도 알려준다.
                S_Move resMovePacket = new S_Move();
                resMovePacket.ObjectId = player.Info.ObjectId;
                resMovePacket.PosInfo = movePacket.PosInfo;

                Broadcast(resMovePacket);
            }
        }

        // Skill이 많아지면 skill class를 따로 빼서 player에 넣는 방식으로 구현
        public void HandleSkill(Player player, C_Skill skillPacket)
        {
            if (player == null) return;
            // player가 방에 소속되어있는지 double check해도 좋음

            // 나중에는 job이나 task 방식으로 만들어야함
            lock(_lock)
            {
                ObjectInfo info = player.Info;
                if (info.PosInfo.State != CreatureState.Idle) return;

                // TODO : 스킬 사용 가능 여부 확인 (ex cooldown)

                // 통과
                info.PosInfo.State = CreatureState.Skill;

                S_Skill skill = new S_Skill() { Info = new SkillInfo() };
                skill.ObjectId = info.ObjectId;
                // skill sheet - json or xml
                // echo 처럼 다시 돌려줌
                skill.Info.SkillId = skillPacket.Info.SkillId;  
                Broadcast(skill);

                Data.Skill skillData = null;
                if (DataManager.SkillDict.TryGetValue(skillPacket.Info.SkillId, out skillData) == false) return;

                switch(skillData.skillType)
                {
                    case SkillType.SkillAuto:
                        {
                            // TODO : 데미지 판정 - 조심해서 짜야함
                            // 만약 client쪽에서 skill을 쓰겠다는 거짓 packet을 1초에 1000개를 보낸다고한다면?
                            // Client에서 coroutine을 이용해 cooldown 시간을 설정한다.

                            // Idle로 바뀌어서 들어왔다면 MoveDir이 none일 것이고 player가 서 있는 cellPos를 반환한다.
                            // 때문에 Dir의 non을 없앰
                            Vector2Int skillPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                            GameObject target = Map.Find(skillPos);
                            if (target != null)
                            {
                                Console.WriteLine("Hit GameObject");
                            }
                        }
                        break;
                    case SkillType.SkillProjectile:
                        {
                            // 투사체가 화살 말고도 있다면 어떤 projectile인지 구분할 수 있는 정보도 있어야하겠다.

                            // 기억은 하지 않지만 새로 생성해준다.
                            // Arrow는 GameRoom에서 관리되어야하는 object
                            Arrow arrow = ObjectManager.Instance.Add<Arrow>();
                            if (arrow == null) return;

                            arrow.Owner = player;
                            arrow.Data = skillData;
                            arrow.PosInfo.State = CreatureState.Moving;
                            arrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
                            arrow.PosInfo.PosX = player.PosInfo.PosX;
                            arrow.PosInfo.PosY = player.PosInfo.PosY;
                            // Projectile의 speed 정보를 StatInfo에도 넘겨줌
                            arrow.Speed = skillData.projectile.speed;

                            // GameRooom에서 기억후 Spawn 패킷 쏴줌
                            EnterGame(arrow);
                        }
                        break;
                }
            }
        }

        public Player FindPlayer(Func<GameObject, bool> condition)
        {

            foreach (Player player in _players.Values)
            {
                if(condition.Invoke(player)) return player;
            }
            return null;
        }

         
        public void Broadcast(IMessage packet)
        {
            // 지금은 간단하게 하기 위해 jobQueue에 넣지 않고 lock만 잡는다.
            lock(_lock)
            {
                foreach(Player p in _players.Values)
                {
                    p.Session.Send(packet);
                }
            }
        }
    }
}
