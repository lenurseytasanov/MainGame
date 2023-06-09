﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MainGame.Models.GameObjects
{
    public class Lava : IDamaging
    {
        public Vector2 Position { get; set; }

        public Rectangle Size { get; set; }

        public Rectangle HitBox { get; set; }

        public int Damage { get; set; } = 2;

        public int SpriteId { get; set; }
    }
}
