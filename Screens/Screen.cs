﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MainGame.Screens
{
    public abstract class Screen
    {
        protected SpriteBatch SpriteBatch;

        protected Game Game;

        protected Screen(Game game)
        {
            Game = game;
        }

        public abstract void Initialize();

        public void LoadContent(SpriteBatch spriteBatch)
        {
            SpriteBatch = spriteBatch;
        }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);
    }
}
