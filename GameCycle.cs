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

        private EnemyAI _enemyAI;
        private PhysicalModel _physic;
        private LevelModel _level;

        public GameCycle()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.ApplyChanges();

            _enemyAI = new EnemyAI();
            _physic = new PhysicalModel();
            _level = new LevelModel();
            _screens = new Dictionary<string, Screen>
            {
                { "GamePlay", new GamePlay() { Game = this, Graphics = _graphics } }
            };

            InitializeLevel();
            InitializePhysic();
            InitializeAI();
            InitializeScreens();

            _currentScreen = "GamePlay";
            base.Initialize();
        }

        private void InitializeLevel()
        {
            _level.Initialize();
        }

        private void InitializeScreens()
        {
            var gamePlay = _screens["GamePlay"] as GamePlay;
            gamePlay.LevelWidth = _level.FieldWidth;
            gamePlay.LevelHeight = _level.FieldHeight;
            gamePlay.Moved += (sender, args) => _physic.Move(args.Id, args.Speed, args.Dir);
            gamePlay.Attacked += (sender, args) => _physic.Attack(args.Id);
            gamePlay.CycleFinished += (sender, args) => _enemyAI.Update(args.ElapsedTime);

            foreach (var screen in _screens.Values)
            {
                screen.Initialize();
            }
        }

        private void InitializePhysic()
        {
            _physic.Updated += (sender, args) => (_screens["GamePlay"] as GamePlay)!.LoadParameters(args.Objects, args.PlayerId);
            _physic.Updated += (sender, args) => _enemyAI.LoadParameters(args.Objects, args.PlayerId);
            _physic.Initialize(_level);
        }

        private void InitializeAI()
        {
            _enemyAI.CycleFinished += (sender, args) => _physic.Update(args.ElapsedTime);
            _enemyAI.Attacked += (sender, args) => _physic.Attack(args.Id);
            _enemyAI.Moved += (sender, args) => _physic.Move(args.Id, args.Speed, args.Dir);
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