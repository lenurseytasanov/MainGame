using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MainGame.Sprites;

public class BackgroundSprite : Sprite
{
    public override void Draw(SpriteBatch spriteBatch, Vector2 shift)
    {
        spriteBatch.Draw(Texture,
            new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                Size.Width, Size.Height),
            Texture.Bounds, Color.White,
            0, Vector2.Zero, SpriteEffects.None, LayerDepth);
    }
}