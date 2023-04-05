using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    public class Knight : Solid
    {
        public float Mass { get; set; } = 1.0f;

        public Vector2 Forces { get; set; }

        public bool OnGround { get; set; }

        public override void Update()
        {
            Position += Speed;
            Direction = Speed.X switch
            {
                > 0 => Direction.Right,
                < 0 => Direction.Left,
                _ => Direction
            };

            PhysicalBound = new Rectangle((int)Position.X, (int)Position.Y, 50, 100);

            Speed += Forces / Mass;
            Forces = new Vector2(0, 1f) + new Vector2(-Speed.X, 0) * 0.3f;
        }
    }
}
