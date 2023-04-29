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

        public int CurrentState { get; set; }

        public float Scale { get; set; }

        public override void Draw(SpriteBatch spriteBatch, Vector2 shift)
        {
            spriteBatch.Draw(States[CurrentState], 
                Position, 
                States[CurrentState].Bounds, Color.White, 
                Rotation, Vector2.Zero, Scale, SpriteEffects.None, LayerDepth);
        }
    }
}
