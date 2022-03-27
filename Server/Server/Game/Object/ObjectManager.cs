using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    //monster나 gameobject도 구별할 수있는 식별자가 필요하니까 공용매니저가 필요할수도
    public class ObjectManager
    {
        public static ObjectManager Instance { get; } = new ObjectManager();

        object _lock = new object();
        // 들고 있는 목적은 귓속말을 하거나 다른 player를 찾아서 무언가 하고 싶을 때를 위해서
        // ObjectManager에서는 Player들을 들고 있다.
        Dictionary<int, Player> _players = new Dictionary<int, Player>();

        // bit flag처럼 쪼개서 사용
        // 제일 상위 부호비트를 빼고 7bit는 gameobject type, 나머지는 id
        // [UNUSED(1)][TYPE(7)][ID(24)]
        int _counter = 0;

        public T Add<T>() where T : GameObject, new()
        {
            // 새로운 instance 생성
            T gameObject = new T();

            lock(_lock)
            {
                gameObject.Id = GenerateId(gameObject.ObjectType);

                if(gameObject.ObjectType == GameObjectType.Player)
                {
                    _players.Add(gameObject.Id, gameObject as Player);
                }
            }

            return gameObject;
        }

        int GenerateId(GameObjectType type)
        {
            lock(_lock)
            {
                return ((int)type << 24) | (_counter++);
            }
        }
        public static GameObjectType GetObjectTypeById(int id)
        {
            int type = (id >> 24) & 0x7F;
            return (GameObjectType)type;
        }

        public bool Remove(int objectId)
        {
            GameObjectType objectType = GetObjectTypeById(objectId);

            lock (_lock)
            {
                if(objectType == GameObjectType.Player)
                    return _players.Remove(objectId);
            }
            // player이외의 나머지 object들은 기억을 하고 있지 않다.
            return false;   
        }

        public Player Find(int objectId)
        {
            GameObjectType objectType = GetObjectTypeById(objectId);

            lock (_lock)
            {
                if (objectType == GameObjectType.Player)
                {
                    Player player = null;
                    if (_players.TryGetValue(objectId, out player)) return player;
                }

                // return은 lock 안에서 하던 밖에서 하던 상관 없다.
                return null;
            }
        }
    }
}

