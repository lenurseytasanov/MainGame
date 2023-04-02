using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MainGame.Sprites
{
    public class KnightSprite : Sprite
    {
        public override void Initialize()
        {
            Animations = new Dictionary<string, Animation>();
            AnimationManager = new AnimationManager();
        }

        public override void Update(GameTime gameTime)
        {
            AnimationManager.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(
                AnimationManager.CurrentAnimation.SpriteSheet, Position + (AnimationManager.CurrentAnimation.Effects == SpriteEffects.FlipHorizontally ? new Vector2(-64, 0) : Vector2.Zero),
                AnimationManager.CurrentFrame, Color.White,
                0, Vector2.Zero, AnimationManager.CurrentAnimation.Scale, AnimationManager.CurrentAnimation.Effects, 0);
            spriteBatch.End();
        }
    }
}
