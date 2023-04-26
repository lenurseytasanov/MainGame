using System.Collections.Generic;
using MainGame.Misc;
using MainGame.Models;
using MainGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MainGame;
// physic.init() -> physic.updated -> screen.Loadparams() -> enemy.loadparams() -> enemy.init() -> screen.init() - screen.loadcontent()
// screen.update() -> screen.cyclefin -> enemy.update() -> enemy.cyclefin -> physic.update() -> physic.updated -> screen.loadparams -> enemy.loadparams
public class GameCycle : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Dictionary<string, Screen> _screens;
    private Screen _currentScreen;

    private EnemyAI _enemyAI;
    private PhysicalModel _physic;

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
        _screens = new Dictionary<string, Screen>
        {
            { "GamePlay", new GamePlay() { Game = this } },
            { "MainMenu", new MainMenu() { Game = this } },
            { "GameOver", new GameOver() { Game = this } }
        };

        InitializePhysic();
        InitializeAI();
        InitializeScreens();

        _currentScreen = _screens["MainMenu"];
        base.Initialize();
    }

    private void InitializePhysic()
    {
        _physic.LevelChanged += (sender, args) => (_screens["GamePlay"] as GamePlay)!.Reset();
        _physic.Updated += (sender, args) => (_screens["GamePlay"] as GamePlay)!.LoadParameters(args.Objects, args.PlayerId, args.LevelSize);
        _physic.Updated += (sender, args) => _enemyAI.LoadParameters(args.Objects, args.PlayerId);
        _physic.Initialize();
    }

    private void InitializeScreens()
    {
        var gamePlay = _screens["GamePlay"] as GamePlay;
        gamePlay.Moved += (sender, args) => _physic.Move(args.Id, args.Dir);
        gamePlay.Attacked += (sender, args) => _physic.Attack(args.Id);
        gamePlay.CycleFinished += (sender, args) => _physic.Update(args.ElapsedTime);
        gamePlay.PlayerDead += (sender, args) => _currentScreen = _screens["GameOver"];

        var mainMenu = _screens["MainMenu"] as MainMenu;
        mainMenu.Started += (sender, args) => _currentScreen = _screens["GamePlay"];

        var gameOver = _screens["GameOver"] as GameOver;
        gameOver.ReStarted += (sender, args) =>
        {
            _currentScreen = _screens["GamePlay"];
            (_screens["GamePlay"] as GamePlay)!.Reset();
            InitializePhysic();
        };
        foreach (var screen in _screens.Values)
        {
            screen.Initialize();
        }
    }

    private void InitializeAI()
    {
        _enemyAI.Moved += (sender, args) => _physic.Move(args.Id, args.Dir);
        _enemyAI.Attacked += (sender, args) => _physic.Attack(args.Id);
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
        _currentScreen.Update(gameTime);
        
        if (_currentScreen == _screens["GamePlay"])
            _enemyAI.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _currentScreen.Draw(gameTime);

        base.Draw(gameTime);
    }
}