using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MainGame
{
    public interface IGameObject
    {
        public Vector2 Position { get; set; }

        public Vector2 Speed { get; set; }

        public ControlEventArgs.Direction Direction { get; set; }

        public void Update();
    }
}
