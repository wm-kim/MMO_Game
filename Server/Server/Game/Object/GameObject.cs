using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class GameObject
    {
        public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
        public int Id
        {
            get { return Info.ObjectId; }
            set { Info.ObjectId = value; }
        }

        // GameRoom을 get set할때 lock을 걸어주면 되지 않을까? - 그렇지 않다.
        // get을 하는 순간 참조값을 넘겨주기 때문이다.
        public GameRoom Room { get; set; }

        public ObjectInfo Info { get; set; } = new ObjectInfo() { };

        public PositionInfo PosInfo { get; private set; } = new PositionInfo();

        public GameObject()
        {
            Info.PosInfo = PosInfo;
        }

        public Vector2Int CellPos
        {
            get
            {
                return new Vector2Int(PosInfo.PosX, PosInfo.PosY);
            }
            set
            {
                PosInfo.PosX = value.x;
                PosInfo.PosY = value.y;
            }
        }

        // Wrapper : PosInfo의 Dir를 굳이 꺼내와서 넘겨주는것이 번거롭다 
        public Vector2Int GetFrontCellPos()
        {
            return GetFrontCellPos(PosInfo.MoveDir);
        }


        public Vector2Int GetFrontCellPos(MoveDir dir)
        {
            Vector2Int cellPos = CellPos;
            switch (dir)
            {
                case MoveDir.Up:
                    cellPos += Vector2Int.up;
                    break;
                case MoveDir.Down:
                    cellPos += Vector2Int.down;
                    break;
                case MoveDir.Left:
                    cellPos += Vector2Int.left;
                    break;
                case MoveDir.Right:
                    cellPos += Vector2Int.right;
                    break;
            }

            return cellPos;
        }
    }
}
