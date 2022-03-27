using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class RoomManager
    {
        public static RoomManager Instance { get; } = new RoomManager();

        object _lock = new object();
        Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
        int _roomId = 1; // 하나씩 늘려갈 것

        // GameRoom 생성
        public GameRoom Add(int mapId)
        {
            GameRoom gameRoom = new GameRoom();
            // Init을 static으로 만들어서 객체를 넘겨주는 방식도 좋다.
            gameRoom.Push(gameRoom.Init, mapId);

            lock(_lock)
            {
                gameRoom.RoomId = _roomId;
                _rooms.Add(_roomId, gameRoom);
                _roomId++; // _roomId가 겹칠일은 없을 것
            }

            return gameRoom;
        }

        public bool Remove(int roomId)
        {
            lock (_lock)
            {
                return _rooms.Remove(roomId);
            }
        }

        public GameRoom Find(int roomId)
        {
            lock(_lock)
            {
                GameRoom room = null;
                if (_rooms.TryGetValue(roomId, out room)) return room;
                return null;
            }
        }
    }
}
