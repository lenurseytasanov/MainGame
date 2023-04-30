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
        public MainMenu(Game game) : base(game) { }

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

            SpriteBatch.Draw(Game.Content.Load<Texture2D>("Backgrounds/stonewall"), Vector2.Zero, Color.White);
            var font1 = Game.Content.Load<SpriteFont>("Fonts/font1");
            var font2 = Game.Content.Load<SpriteFont>("Fonts/font2");
            var message1 = "\"Enter\" - start";
            var message2 = "\"Esc\" - close game";
            var message3 = "\"A\" - attacking";
            var message4 = "< > ^ - moving";
            var message5 = "Controls:";
            SpriteBatch.DrawString(
                font2,
                message5,
                new Vector2(
                    Game.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - font2.MeasureString(message5).X / 2,
                    Game.GraphicsDevice.PresentationParameters.BackBufferHeight / 6),
                Color.Bisque);
            SpriteBatch.DrawString(
                font1, 
                message1, 
                new Vector2(
                    Game.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - font1.MeasureString(message1).X / 2, 
                    Game.GraphicsDevice.PresentationParameters.BackBufferHeight / 3), 
                Color.Gray);
            SpriteBatch.DrawString(
                font1,
                message2,
                new Vector2(
                    Game.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - font1.MeasureString(message2).X / 2,
                    Game.GraphicsDevice.PresentationParameters.BackBufferHeight / 3 + font1.MeasureString(message1).Y + 50),
                Color.Gray);
            SpriteBatch.DrawString(
                font1,
                message3,
                new Vector2(
                    Game.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - font1.MeasureString(message3).X / 2,
                    Game.GraphicsDevice.PresentationParameters.BackBufferHeight / 3 + font1.MeasureString(message2).Y + 100 +
                    font1.MeasureString(message1).Y),
                Color.Gray);
            SpriteBatch.DrawString(
                font1,
                message4,
                new Vector2(
                    Game.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - font1.MeasureString(message4).X / 2,
                    Game.GraphicsDevice.PresentationParameters.BackBufferHeight / 3 + 150 + font1.MeasureString(message3).Y +
                    font1.MeasureString(message2).Y + font1.MeasureString(message1).Y),
                Color.Gray);

            SpriteBatch.End();
        }

        public event EventHandler Started;
    }
}
