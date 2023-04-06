using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MainGame.Misc
{
    public class CycleEventArgs : EventArgs
    {
        public GameTime ElapsedTime { get; set; }
    }
}
