using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    public abstract class Solid : IGameObject
    {
        public Rectangle PhysicalBound { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Speed { get; set ; }

        public Direction Direction { get; set; }

        public bool IsTouchLeft(Solid other)
        {
            return PhysicalBound.Right + Speed.X > other.PhysicalBound.Left &&
                   PhysicalBound.Left < other.PhysicalBound.Left &&
                   PhysicalBound.Bottom > other.PhysicalBound.Top &&
                   PhysicalBound.Top < other.PhysicalBound.Bottom;
        }

        public bool IsTouchRight(Solid other)
        {
            return PhysicalBound.Left + Speed.X < other.PhysicalBound.Right &&
                   PhysicalBound.Right > other.PhysicalBound.Right &&
                   PhysicalBound.Bottom > other.PhysicalBound.Top &&
                   PhysicalBound.Top < other.PhysicalBound.Bottom;
        }

        public bool IsTouchTop(Solid other)
        {
            return this.PhysicalBound.Bottom + this.Speed.Y > other.PhysicalBound.Top &&
                   this.PhysicalBound.Top < other.PhysicalBound.Top &&
                   this.PhysicalBound.Right > other.PhysicalBound.Left &&
                   this.PhysicalBound.Left < other.PhysicalBound.Right;
        }

        public bool IsTouchBottom(Solid other)
        {
            return this.PhysicalBound.Top + this.Speed.Y < other.PhysicalBound.Bottom &&
                   this.PhysicalBound.Bottom > other.PhysicalBound.Bottom &&
                   this.PhysicalBound.Right > other.PhysicalBound.Left &&
                   this.PhysicalBound.Left < other.PhysicalBound.Right;
        }

        public abstract void Update();
    }
}
