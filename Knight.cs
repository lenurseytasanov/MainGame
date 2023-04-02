using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MainGame
{
    public class Knight : IGameObject
    {
        public Vector2 Position { get; set; }

        public Vector2 Speed { get; set; }

        public ControlEventArgs.Direction Direction { get; set; } = ControlEventArgs.Direction.Right;

        public void Update()
        {
            Position += Speed;
            Direction = Speed.X > 0 ? ControlEventArgs.Direction.Right 
                : Speed.X < 0 ? ControlEventArgs.Direction.Left : Direction;
            Speed = Vector2.Zero;
        }
    }
}
