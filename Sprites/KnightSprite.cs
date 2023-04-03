using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MainGame.Sprites
{
    public class KnightSprite : AnimatedSprite
    {
        public Direction Direction { get; set; }

        public bool OnGround { get; set; }
    }
}
