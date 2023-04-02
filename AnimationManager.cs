using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Sprites;
using Microsoft.Xna.Framework;

namespace MainGame
{
    public class AnimationManager
    {
        public Animation CurrentAnimation;

        public Rectangle CurrentFrame => new Rectangle(
                CurrentAnimation.Cycle.FrameIndex * CurrentAnimation.FrameWidth, 0,
                CurrentAnimation.FrameWidth, CurrentAnimation.FrameHeight
            );

        public TimeSpan TimePerFrame { get; set; } = TimeSpan.FromMilliseconds(100);

        public void Play(Animation animation)
        {
            if (CurrentAnimation != null && CurrentAnimation.Equals(animation)) return;

            CurrentAnimation = animation;

            CurrentAnimation.Cycle ??= new AnimationCycle()
            {
                ElapsedTime = TimeSpan.Zero,
                FrameIndex = 0
            };
        }

        public void Update(GameTime gameTime)
        {
            CurrentAnimation.Cycle.ElapsedTime += gameTime.ElapsedGameTime;

            if (CurrentAnimation.Cycle.ElapsedTime > TimePerFrame)
            {
                CurrentAnimation.Cycle.ElapsedTime = TimeSpan.Zero;
                CurrentAnimation.Cycle.FrameIndex++;
                CurrentAnimation.Cycle.FrameIndex %= CurrentAnimation.FrameCount;
            }
        }
    }
}
