using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MainGame
{
    public abstract class Screen
    {
        public Game Game { get; set; }

        public abstract void Initialize();

        public abstract void LoadContent(SpriteBatch spriteBatch);

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);
    }
}
