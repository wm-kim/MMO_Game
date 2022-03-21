﻿using Google.Protobuf.Protocol;
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

        // Dictinary를 이용해 id로 player를 빠르게 찾을 수 있게 할 수도 있고
        // Grid 단위로 player를 들고 있게 해도됨
        List<Player> _players = new List<Player>();
        
        // 인자로 접속한 player의 정보 전달
        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null) return;

            lock(_lock)
            {
                _players.Add(newPlayer);
                newPlayer.Room = this;

                // 본인한테 정보 전송
                {
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = newPlayer.Info;
                    newPlayer.Session.Send(enterPacket);

                    // 나한테 나머지 player에 대한 정보를 보내줌
                    S_Spawn spawnPacket = new S_Spawn();
                    foreach(Player p in _players)
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
                    foreach (Player p in _players)
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
                Player player = _players.Find(p => p.Info.PlayerId == playerId);
                if (player == null) return;

                _players.Remove(player);
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
                    foreach(Player p in _players)
                    {
                        if(player != p) p.Session.Send(despawnPacket);
                    }
                }
            }
        }
    }
}
