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
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                AnimationManager.CurrentAnimation.SpriteSheet, Position - new Vector2(0, 20) + (AnimationManager.CurrentAnimation.Effects == SpriteEffects.FlipHorizontally ? new Vector2(-64, 0) : Vector2.Zero),
                AnimationManager.CurrentFrame, Color.White,
                0, Vector2.Zero, AnimationManager.CurrentAnimation.Scale, AnimationManager.CurrentAnimation.Effects, 0);
        }
    }
}
