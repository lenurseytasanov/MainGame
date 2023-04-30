using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MainGame.Screens;

public class Victory : Screen
{
    public Victory(Game game) : base(game)
    { }

    public override void Initialize()
    { }

    public override void Update(GameTime gameTime)
    {
        var keys = Keyboard.GetState().GetPressedKeys();

        foreach (var key in keys)
        {
            if (key == Keys.Escape) Game.Exit();
        }
    }

    public override void Draw(GameTime gameTime)
    {
        Game.GraphicsDevice.Clear(Color.Olive);

        SpriteBatch.Begin();

        var font1 = Game.Content.Load<SpriteFont>("Fonts/font1");
        var font2 = Game.Content.Load<SpriteFont>("Fonts/font2");
        var message1 = "Victory!";
        SpriteBatch.DrawString(
            font2,
            message1,
            new Vector2(
                Game.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - font2.MeasureString(message1).X / 2,
                Game.GraphicsDevice.PresentationParameters.BackBufferHeight / 3),
            Color.Bisque);

        SpriteBatch.End();
    }
}