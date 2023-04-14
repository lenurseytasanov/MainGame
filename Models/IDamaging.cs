using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    internal interface IDamaging
    {
        public int Damage { get; set; }

        public Rectangle HitBox { get; set; }
    }
}
