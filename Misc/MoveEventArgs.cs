using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainGame.Misc
{
    public class MoveEventArgs : EventArgs
    {
        public int Id { get; set; }

        public float Speed { get; set; }

        public Direction Dir { get; set; }
    }
}
