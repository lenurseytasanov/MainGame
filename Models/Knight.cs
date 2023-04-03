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

        public override void Update()
        {
            Position += Speed;
            Direction = Speed.X > 0 ? Direction.Right
                : Speed.X < 0 ? Direction.Left : Direction;

            PhysicalBound = new Rectangle((int)Position.X, (int)Position.Y, 100, 100);

            Speed += Forces / Mass;
            Forces = new Vector2(0, 1f) - Speed * 0.1f;
        }
    }
}
