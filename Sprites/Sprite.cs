using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainGame.Sprites
{
    public class Sprite
    {
        public Vector2 Position { get; set; }

        public Rectangle Size { get; set; }

        public Texture2D Texture { get; set; }

        public float LayerDepth { get; set; }

        public float Rotation { get; set; } = 0;

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 shift)
        {
            spriteBatch.Draw(Texture,
                new Rectangle(
                    (int)shift.X + (int)Position.X,
                    (int)shift.Y + (int)Position.Y,
                    Size.Width, Size.Height),
                Texture.Bounds, Color.White,
                Rotation, Vector2.Zero, SpriteEffects.None, LayerDepth);
        }
    }
}
