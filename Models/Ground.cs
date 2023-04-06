using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    public class Ground : Solid
    {
        public override void Update(GameTime gameTime)
        {
            PhysicalBound = new Rectangle((int)Position.X, (int)Position.Y, 20000, 100);
        }
    }
}
