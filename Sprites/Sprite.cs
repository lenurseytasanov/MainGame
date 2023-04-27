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

        public Action<object, SpriteBatch, Vector2> Draw { get; set; } = (sender, spriteBatch, shift) =>
        {
            var o = sender as Sprite;
            spriteBatch.Draw(o.Texture, o.Position, Color.White);
        };
    }
}
