using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    public class Knight : IGameObject
    {
        public Vector2 Position { get; set; }

        public Vector2 Speed { get; set; }

        public Direction Direction { get; set; } = Direction.Right;

        public float Mass { get; set; } = 1.0f;

        public void Update()
        {
            Position += Speed;
            Direction = Speed.X > 0 ? Direction.Right
                : Speed.X < 0 ? Direction.Left : Direction;

            Speed += new Vector2(0, 0.9f) / Mass;
            Speed = new Vector2(0, Speed.Y);
        }
    }
}
