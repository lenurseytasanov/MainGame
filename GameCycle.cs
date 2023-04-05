using System.Collections.Generic;
using MainGame.Models;
using MainGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MainGame
{
    public class GameCycle : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Dictionary<string, Screen> _screens;

        private string _currentScreen;
        private PhysicalModel _model;

        public GameCycle()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = 430;
            _graphics.ApplyChanges();

            _screens = new Dictionary<string, Screen>
            {
                { "GamePlay", new GamePlay() { Game = this } }
            };

            _model = new PhysicalModel();

            var gamePlay = _screens["GamePlay"] as GamePlay;
            gamePlay.PlayerMoved += (sender, args) => _model.MovePlayer(args.Dir);
            gamePlay.CycleFinished += (sender, args) => _model.Update();
            _model.Updated += (sender, args) => gamePlay.LoadParameters(args.Objects);
            _model.Initialized += (sender, args) => gamePlay.LoadStartParameters(args.Objects, args.PlayerId);

            _model.Initialize();

            foreach (var screen in _screens.Values)
            {
                screen.Initialize();
            }

            _currentScreen = "GamePlay";
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (var screen in _screens.Values)
            {
                screen.LoadContent(_spriteBatch);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _screens[_currentScreen].Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _screens[_currentScreen].Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}