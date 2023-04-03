using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MainGame.Sprites
{
    public class GroundSprite : Sprite
    {
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var effects = SpriteEffects.None;
            for (var i = 0; i < 1000; i++)
            {
                spriteBatch.Draw(Texture, Position, Texture.Bounds, Color.White,
                    0, Vector2.Zero, 1, effects ^= SpriteEffects.FlipHorizontally, 1);
                Position += new Vector2(Texture.Width, 0);
            }
        }
    }
}
