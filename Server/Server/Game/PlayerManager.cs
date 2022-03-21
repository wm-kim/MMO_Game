using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    //monster나 gameobject도 구별할 수있는 식별자가 필요하니까 공용매니저가 필요할수도
    public class PlayerManager
    {

        public static PlayerManager Instance { get; } = new PlayerManager();

        object _lock = new object();
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        int _playerId = 1; // TODO id를 관리할때 int32를 다쓸지는 의문이다. bit flag처럼 쪼개서 사용하는 경우가 많다

        // Player 새로 발급
        public Player Add()
        {
            Player player = new Player();

            lock (_lock)
            {
                player.Info.PlayerId = _playerId;
                _players.Add(_playerId, player);
                _playerId++;

                return player;
            }
        }

        public bool Remove(int playerId)
        {
            lock (_lock)
            {
                return _players.Remove(playerId);
            }
        }

        public Player Find(int playerId)
        {
            lock (_lock)
            {
                Player player = null;
                if (_players.TryGetValue(playerId, out player)) return player;
                return null;
            }
        }
    }
}

