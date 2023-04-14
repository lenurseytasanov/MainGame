using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainGame.Misc
{
    public class LevelChangeArgs : EventArgs
    {
        public Direction Direction { get; set; }

        public string LevelName { get; set; }
    }
}
