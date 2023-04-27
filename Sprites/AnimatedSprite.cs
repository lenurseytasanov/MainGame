using MainGame.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainGame.Sprites
{
    public class AnimatedSprite : Sprite
    {
        public Dictionary<string, Animation> Animations { get; set; }

        public AnimationManager AnimationManager { get; set; } = new AnimationManager();

        public void SetAnimation(string animationName)
        {
            AnimationManager.Play(Animations[animationName]);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 shift)
        {
            spriteBatch.Draw(
                    AnimationManager.CurrentAnimation.SpriteSheet, Position,
                    AnimationManager.CurrentFrame, Color.White,
                    0, Vector2.Zero, AnimationManager.CurrentAnimation.Scale, AnimationManager.CurrentAnimation.Effects,
                    LayerDepth);
        }

        public virtual void Update(GameTime gameTime)
        {
            AnimationManager.Update(gameTime);
        }
    }
}
