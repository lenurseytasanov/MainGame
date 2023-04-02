using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainGame
{
    public class ControlEventArgs
    {
        public enum Direction
        {
            Left,
            Right,
            Down,
            Up
        }

        public Direction Dir { get; set; }
    }
}
