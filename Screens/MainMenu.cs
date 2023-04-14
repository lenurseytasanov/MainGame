using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MainGame.Screens
{
    public class MainMenu : Screen
    {
        public override void Initialize()
        { }

        public override void Update(GameTime gameTime)
        {
            var keys = Keyboard.GetState().GetPressedKeys();

            foreach (var key in keys)
            {
                switch (key)
                {
                    case Keys.Enter:
                        Started?.Invoke(this, EventArgs.Empty);
                        break;
                    case Keys.Escape:
                        Game.Exit();
                        break;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Olive);

            SpriteBatch.Begin();

            SpriteBatch.DrawString(
                Game.Content.Load<SpriteFont>("fonts/font"), 
                "Press \"Enter\" to start", 
                new Vector2(
                    Game.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - 100, 
                    Game.GraphicsDevice.PresentationParameters.BackBufferHeight / 2), 
                Color.Black);

            SpriteBatch.End();
        }

        public event EventHandler Started;
    }
}
