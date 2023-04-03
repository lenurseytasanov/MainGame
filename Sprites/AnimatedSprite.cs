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
        public Dictionary<string, Animation> Animations { get; private set; }

        public AnimationManager AnimationManager { get; set; }

        public void SetAnimation(string animationName)
        {
            AnimationManager.Play(Animations[animationName]);
        }

        public void LoadAnimations(Dictionary<string, Animation> animations)
        {
            Animations = animations;
        }

        public virtual void Initialize()
        {
            Animations = new Dictionary<string, Animation>();
            AnimationManager = new AnimationManager();
        }

        public virtual void Update(GameTime gameTime)
        {
            AnimationManager.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                AnimationManager.CurrentAnimation.SpriteSheet, Position,
                AnimationManager.CurrentFrame, Color.White,
                0, Vector2.Zero, AnimationManager.CurrentAnimation.Scale, AnimationManager.CurrentAnimation.Effects, 0);
        }
    }
}
