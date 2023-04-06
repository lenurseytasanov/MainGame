using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainGame.Misc
{
    public class ControlEventArgs : EventArgs
    {
        public Direction Dir { get; set; }
    }
}
