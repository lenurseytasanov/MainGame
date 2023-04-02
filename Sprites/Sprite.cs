using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainGame.Sprites
{
    public abstract class Sprite
    {
        public Dictionary<string, Animation> Animations { get; set; }

        public AnimationManager AnimationManager { get; set; }

        public void SetAnimation(string animationName)
        {
            AnimationManager.Play(Animations[animationName]);
        }

        public Vector2 Position { get; set; }

        public Direction Direction { get; set; }

        public abstract void Initialize();

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
