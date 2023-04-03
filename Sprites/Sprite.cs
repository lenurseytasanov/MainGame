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

        public Direction Direction { get; set; }

        public Texture2D Texture { get; set; }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
