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
    public class GameOver : Screen
    {
        public GameOver(Game game) : base(game) { }

        public override void Initialize()
        { }

        public override void Update(GameTime gameTime)
        {
            var keys = Keyboard.GetState().GetPressedKeys();

            foreach (var key in keys)
            {
                switch (key)
                {
                    case Keys.R:
                        ReStarted?.Invoke(this, EventArgs.Empty);
                        break;
                    case Keys.Escape:
                        Game.Exit();
                        break;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Brown);

            SpriteBatch.Begin();

            var font1 = Game.Content.Load<SpriteFont>("Fonts/font1");
            var font2 = Game.Content.Load<SpriteFont>("Fonts/font2");
            var message1 = "You're dead!";
            var message2 = "Press \"R\" to restart";
            SpriteBatch.DrawString(
                font2,
                message1,
                new Vector2(
                    Game.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - font2.MeasureString(message1).X / 2,
                    Game.GraphicsDevice.PresentationParameters.BackBufferHeight / 4),
                Color.Black);
            SpriteBatch.DrawString(
                font1,
                message2,
                new Vector2(
                    Game.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - font1.MeasureString(message2).X / 2,
                    Game.GraphicsDevice.PresentationParameters.BackBufferHeight / 2),
                Color.Black);

            SpriteBatch.End();
        }

        public event EventHandler ReStarted;
    }
}
