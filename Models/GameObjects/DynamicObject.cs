using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Misc;
using Microsoft.Xna.Framework;

namespace MainGame.Models.GameObjects
{
    public abstract class DynamicObject : IGameObject
    {
        public Vector2 Position { get; set; }

        public Rectangle Size { get; set; }

        public int SpriteId { get; set; }

        public Vector2 Speed { get; set; }

        public Vector2 Forces { get; set; }

        public float Acceleration { get; set; } = 4.0f;

        public float AirResistance { get; set; } = 0.6f;

        public float Mass { get; set; } = 1.0f;

        public virtual void Update(GameTime gameTime)
        {
            Size = new Rectangle(Size.X + (int)Speed.X, Size.Y + (int)Speed.Y, Size.Width, Size.Height);
            Position += new Vector2((int)Speed.X, (int)Speed.Y);
            Speed += Mass > 0 ? Forces / Mass : Vector2.Zero;
            Forces = Vector2.Zero;
            Forces += new Vector2(0, 1f) * Mass; // gravity
            Forces += new Vector2(-Speed.X * AirResistance, 0); // resistance
        }
    }
}
