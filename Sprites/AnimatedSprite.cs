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

        public AnimationManager AnimationManager { get; set; }

        public void SetAnimation(string animationName)
        {
            AnimationManager.Play(Animations[animationName]);
        }

        public virtual void Initialize()
        {
            Animations ??= new Dictionary<string, Animation>();
            AnimationManager ??= new AnimationManager();
            Draw ??= (sender, spriteBatch, shift) =>
            {
                var o = sender as AnimatedSprite;
                spriteBatch.Draw(
                    o.AnimationManager.CurrentAnimation.SpriteSheet, o.Position,
                    o.AnimationManager.CurrentFrame, Color.White,
                    0, Vector2.Zero, o.AnimationManager.CurrentAnimation.Scale, o.AnimationManager.CurrentAnimation.Effects,
                    0);
            };
        }

        public virtual void Update(GameTime gameTime)
        {
            AnimationManager.Update(gameTime);
        }
    }
}
