using System.Collections.Generic;
using MainGame.Misc;
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

        private Dictionary<string, LevelModel> _levels;

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
            _levels = new Dictionary<string, LevelModel>()
            {
                { "level1", new LevelModel() { LoadPath = "../../../Sources/Levels/level1.txt", Name = "level1"} },
                { "level2", new LevelModel() { LoadPath = "../../../Sources/Levels/level2.txt", Name = "level2" } }
            };
            _screens = new Dictionary<string, Screen>
            {
                { "GamePlay", new GamePlay() { Game = this } },
                { "MainMenu", new MainMenu() { Game = this } },
                { "GameOver", new GameOver() { Game = this } }
            };

            InitializeLevels();
            InitializePhysic();
            InitializeAI();
            InitializeScreens();

            _currentScreen = "MainMenu";
            base.Initialize();
        }

        private void InitializeLevels()
        {
            foreach (var level in _levels.Values)
            {
                level.Initialize();
            }
        }

        private void InitializeScreens()
        {
            var gamePlay = _screens["GamePlay"] as GamePlay;
            gamePlay.LevelWidth = _levels["level1"].FieldWidth;
            gamePlay.LevelHeight = _levels["level1"].FieldHeight;
            gamePlay.Moved += (sender, args) => _physic.Move(args.Id, args.Dir);
            gamePlay.Attacked += (sender, args) => _physic.Attack(args.Id);
            gamePlay.CycleFinished += (sender, args) => _enemyAI.Update(args.ElapsedTime);
            gamePlay.PlayerDead += (sender, args) => _currentScreen = "GameOver";

            var mainMenu = _screens["MainMenu"] as MainMenu;
            mainMenu.Started += (sender, args) => _currentScreen = "GamePlay";

            var gameOver = _screens["GameOver"] as GameOver;
            gameOver.ReStarted += (sender, args) =>
            {
                _currentScreen = "GamePlay";
                (_screens["GamePlay"] as GamePlay).Reset();
                InitializeLevels();
                InitializePhysic();
            };
            foreach (var screen in _screens.Values)
            {
                screen.Initialize();
            }
        }

        private void InitializePhysic()
        {
            _physic.LevelChanged += (sender, args) =>
            {
                var gamePlay = _screens["GamePlay"] as GamePlay;
                gamePlay.Reset();
                if (args.LevelName == "level1" && args.Direction == Direction.Right)
                    _physic.Level = _levels["level2"];
                if (args.LevelName == "level2" && args.Direction == Direction.Left)
                    _physic.Level = _levels["level1"];
                _physic.Initialize();
            };
            _physic.Updated += (sender, args) => (_screens["GamePlay"] as GamePlay)!.LoadParameters(args.Objects, args.PlayerId);
            _physic.Updated += (sender, args) => _enemyAI.LoadParameters(args.Objects, args.PlayerId);
            _physic.Level = _levels["level1"];
            _physic.Initialize();
        }

        private void InitializeAI()
        {
            _enemyAI.Moved += (sender, args) => _physic.Move(args.Id, args.Dir);
            _enemyAI.Attacked += (sender, args) => _physic.Attack(args.Id);
            _enemyAI.CycleFinished += (sender, args) => _physic.Update(args.ElapsedTime);
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