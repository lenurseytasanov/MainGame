using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MainGame.Models.GameObjects
{
    public class Ground : ISolid
    {
        public Vector2 Position { get; set; }

        public Rectangle Size { get; set; }

        public int SpriteId { get; set; }

        public Rectangle PhysicalBound { get; set; }

        public bool IsTouchLeft(ISolid other)
        {
            return PhysicalBound.Right > other.PhysicalBound.Left &&
                   PhysicalBound.Left < other.PhysicalBound.Left &&
                   PhysicalBound.Bottom > other.PhysicalBound.Top &&
                   PhysicalBound.Top < other.PhysicalBound.Bottom;
        }

        public bool IsTouchRight(ISolid other)
        {
            return PhysicalBound.Left < other.PhysicalBound.Right &&
                   PhysicalBound.Right > other.PhysicalBound.Right &&
                   PhysicalBound.Bottom > other.PhysicalBound.Top &&
                   PhysicalBound.Top < other.PhysicalBound.Bottom;
        }

        public bool IsTouchTop(ISolid other)
        {
            return PhysicalBound.Bottom > other.PhysicalBound.Top &&
                   PhysicalBound.Top < other.PhysicalBound.Top &&
                   PhysicalBound.Right > other.PhysicalBound.Left &&
                   PhysicalBound.Left < other.PhysicalBound.Right;
        }

        public bool IsTouchBottom(ISolid other)
        {
            return PhysicalBound.Top < other.PhysicalBound.Bottom &&
                   PhysicalBound.Bottom > other.PhysicalBound.Bottom &&
                   PhysicalBound.Right > other.PhysicalBound.Left &&
                   PhysicalBound.Left < other.PhysicalBound.Right;
        }
    }
}
