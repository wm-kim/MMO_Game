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

        public void Update()
        {
            // TODO
        }
    }
}
