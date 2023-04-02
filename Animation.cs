using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace MainGame
{
    public class Animation
    {
        public Texture2D SpriteSheet { get; set; }

        public AnimationCycle Cycle { get; set; }

        public int FrameCount { get; set; }

        public int FrameWidth => SpriteSheet.Width / FrameCount;

        public int FrameHeight => SpriteSheet.Height;

        public SpriteEffects Effects { get; set; } = SpriteEffects.None;

        public float Scale { get; set; } = 1f;
    }
}
