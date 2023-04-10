using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    public class StaticSolidObject : IGameObject
    {
        public Vector2 Position { get; set; }

        public Rectangle Size { get; set; }

        public Rectangle PhysicalBound { get; set; }

        public int SpriteId { get; set; }
    }
}
