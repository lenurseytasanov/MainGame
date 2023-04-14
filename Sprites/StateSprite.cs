using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MainGame.Sprites
{
    public class StateSprite : Sprite
    {
        public Dictionary<int, Texture2D> States { get; set; }

        public Texture2D CurrentState { get; private set; }

        public void SetState(int name)
        {
            CurrentState = States[name];
        }

        public void Initialize()
        {
            Draw = (sender, sb, shiftOnPlayer) =>
            {
                var sprite = sender as StateSprite;
                sb.Draw(sprite.CurrentState, sprite.Position, sprite.CurrentState.Bounds, Color.White, 
                    0f, Vector2.Zero, 0.5f, SpriteEffects.None, 1);
            };
        }
    }
}
