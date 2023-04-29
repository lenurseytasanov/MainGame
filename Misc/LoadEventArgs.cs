using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Models;
using MainGame.Models.GameObjects;
using Microsoft.Xna.Framework;

namespace MainGame.Misc
{
    public class LoadEventArgs : EventArgs
    {
        public LevelModel Level { get; set; }
    }
}
