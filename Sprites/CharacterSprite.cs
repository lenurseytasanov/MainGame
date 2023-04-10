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
    public class CharacterSprite : AnimatedSprite
    {
        public Direction Direction { get; set; }

        public int AttackNumber { get; set; }

        public StateCharacter State { get; set; }
    }
}
