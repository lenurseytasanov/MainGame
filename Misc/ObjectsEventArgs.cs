using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Models.GameObjects;
using Microsoft.Xna.Framework;

namespace MainGame.Misc
{
    public class ObjectsEventArgs : EventArgs
    {
        public Dictionary<int, IGameObject> Objects { get; set; }

        public int PlayerId { get; set; }

        public Rectangle LevelSize { get; set; }
    }
}
