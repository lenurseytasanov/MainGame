using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    public interface IGameObject
    {
        public Vector2 Position { get; set; }

        public Vector2 Speed { get; set; }

        public int SpriteId { get; set; }

        public void Update(GameTime gameTime);
    }
}
