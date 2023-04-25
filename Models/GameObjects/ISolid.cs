using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MainGame.Models.GameObjects
{
    public interface ISolid : IGameObject
    {
        public Rectangle PhysicalBound { get; set; }

        public bool IsTouchLeft(ISolid other);

        public bool IsTouchRight(ISolid other);

        public bool IsTouchTop(ISolid other);

        public bool IsTouchBottom(ISolid other);
    }
}
