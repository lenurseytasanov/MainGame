using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MainGame.Sprites
{
    public class KnightSprite : AnimatedSprite
    {
        public Direction Direction { get; set; }

        public Attack Attack { get; set; }

        public bool IsOnGround { get; set; }

        public bool IsDead { get; set; }

        public bool IsHurt { get; set; }
    }
}
