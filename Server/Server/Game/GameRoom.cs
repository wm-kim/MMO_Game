using Google.Protobuf;
using Google.Protobuf.Protocol;
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
        // 내가 소속되어있는 map, 좌표를 기준으로 player가 있는지 없는지 찾고 싶음 -> map 안에서 관리
        Map _map = new Map();
        
        public void Init(int mapId)
        {   
            // mapId에 따른 경로도 나중에 공식화해서 지정이 될 것
            _map.LoadMap(mapId, "../../../../../Common/Mapdata");
        }

        // 인자로 접속한 player의 정보 전달
        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null) return;

            lock(_lock)
            {
                _players.Add(newPlayer.Info.PlayerId, newPlayer);
                newPlayer.Room = this;

                // 본인한테 정보 전송
                {
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = newPlayer.Info;
                    newPlayer.Session.Send(enterPacket);

                    // 나한테 나머지 player에 대한 정보를 보내줌
                    S_Spawn spawnPacket = new S_Spawn();
                    foreach(Player p in _players.Values)
                    {
                        if (newPlayer != p) spawnPacket.Players.Add(p.Info);
                    }
                    newPlayer.Session.Send(spawnPacket);
                    // 지금은 contents code와 network로 보내는 코드 같이 묶여있음
                }
                // 타인한테 정보 전송
                {
                    S_Spawn spawnPacket = new S_Spawn();
                    spawnPacket.Players.Add(newPlayer.Info);
                    foreach (Player p in _players.Values)
                    {
                        if (newPlayer != p) p.Session.Send(spawnPacket);
                    }
                }
            }
        }

        public void LeaveGame(int playerId)
        {
            lock (_lock)
            {
                Player player = null;
                if (_players.Remove(playerId, out player) == false)
                    return; 

                player.Room = null;

                // 본인한테 정보 전송
                {
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    player.Session.Send(leavePacket);
                }

                // 타인에게 정보 전송
                {
                    S_Despawn despawnPacket = new S_Despawn();
                    despawnPacket.PlayerIds.Add(player.Info.PlayerId);
                    foreach(Player p in _players.Values)
                    {
                        if(player != p) p.Session.Send(despawnPacket);
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
                PlayerInfo info = player.Info;

                // 다른 좌표로 이동할 경우 갈 수 있는지 확인, 해킹 방지
                // 좌표는 이동안했는데 상태가 idle로 변경할 때도 사용하기 때문에 그럴때는 통과시켜줘야함
                if (movePosInfo.PosX != info.PosInfo.PosX || movePosInfo.PosY != info.PosInfo.PosY)
                {
                    if (_map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
                        return;
                }

                // 검증 완료
                info.PosInfo.State = movePosInfo.State;
                info.PosInfo.MoveDir = movePosInfo.MoveDir;
                // 좌표 이동하는 부분이 gameRoom에서가 아니라 map에서 하는 이유, player 2차배열 변경을 위해서
                // 왜 ApplyMove안에서는 lock을 사용하지 않는가? - 상위단계인 gameRoom에서 이미 lock을 잡고 있으므로
                _map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));

                // 다른 플레이어한테도 알려준다.
                S_Move resMovePacket = new S_Move();
                resMovePacket.PlayerId = player.Info.PlayerId;
                resMovePacket.PosInfo = movePacket.PosInfo;

                BroadCast(resMovePacket);
            }
        }

        public void HandleSkill(Player player, C_Skill skillPacket)
        {
            if (player == null) return;
            // player가 방에 소속되어있는지 double check해도 좋음

            // 나중에는 job이나 task 방식으로 만들어야함
            lock(_lock)
            {
                PlayerInfo info = player.Info;
                if (info.PosInfo.State != CreatureState.Idle) return;

                // TODO : 스킬 사용 가능 여부 확인 (ex cooldown)

                // 통과
                info.PosInfo.State = CreatureState.Skill;

                S_Skill skill = new S_Skill() { Info = new SkillInfo() };
                skill.PlayerId = info.PlayerId;
                // skill sheet - json or xml
                skill.Info.SkillId = 1;
                BroadCast(skill);

                // TODO : 데미지 판정 - 조심해서 짜야함
                // 만약 client쪽에서 skill을 쓰겠다는 거짓 packet을 1초에 1000개를 보낸다고한다면?

                // Idle로 바뀌어서 들어왔다면 MoveDir이 none일 것이고 player가 서 있는 cellPos를 반환한다.
                Vector2Int skillPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
                Player target = _map.Find(skillPos);
                if(target != null)
                {
                    Console.WriteLine("Hit Player");
                }
            }
        }
         
        public void BroadCast(IMessage packet)
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
