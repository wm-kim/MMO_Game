﻿using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Player : GameObject
    {
        public ClientSession Session { get; set; }

        public Player()
        {
            ObjectType = GameObjectType.Player;
            Speed = 20.0f;
        }

        // player도 pvp가 된다고 가정
        public override void OnDamaged(GameObject attacker, int damage)
        {
            Console.WriteLine($"TODO : damage {damage}");
        }
    }
}
