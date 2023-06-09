﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MainGame.Misc;
using MainGame.Models;
using MainGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MainGame;

public class GameCycle : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Dictionary<string, Screen> _screens;
    private string _currentScreen;

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
            { "GamePlay", new GamePlay(this) },
            { "MainMenu", new MainMenu(this) },
            { "GameOver", new GameOver(this) },
            { "Victory", new Victory(this) }
        };

        InitializePhysic();
        InitializeAI();
        InitializeScreens();

        _currentScreen = "MainMenu";
        base.Initialize();
    }

    private void InitializePhysic()
    {
        _physic.LevelChanged += (sender, args) => (_screens["GamePlay"] as GamePlay)!.Reset();
        _physic.Updated += (sender, args) => (_screens["GamePlay"] as GamePlay)!.LoadParameters(args.Level);
        _physic.Updated += (sender, args) => _enemyAI.LoadParameters(args.Level);
        _physic.Initialize();
    }

    private void InitializeScreens()
    {
        var gamePlay = _screens["GamePlay"] as GamePlay;
        gamePlay.Moved += (sender, args) => _physic.Move(args.Id, args.Dir);
        gamePlay.Attacked += (sender, args) => _physic.Attack(args.Id);
        gamePlay.CycleFinished += (sender, args) => _physic.Update(args.ElapsedTime);
        gamePlay.PlayerDead += (sender, args) => ChangeScreen("GameOver");
        gamePlay.BossDead += (sender, args) => ChangeScreen("Victory");

        var mainMenu = _screens["MainMenu"] as MainMenu;
        mainMenu.Started += (sender, args) => _currentScreen = "GamePlay";

        var gameOver = _screens["GameOver"] as GameOver;
        gameOver.ReStarted += (sender, args) =>
        {
            _currentScreen = "GamePlay";
            (_screens["GamePlay"] as GamePlay)!.Reset();
            _physic.Reset();
        };
        foreach (var screen in _screens.Values)
        {
            screen.Initialize();
        }
    }

    private async Task ChangeScreen(string screen)
    {
        await Task.Delay(2000);
        _currentScreen = screen;
    }

    private void InitializeAI()
    {
        _enemyAI.Moved += (sender, args) => _physic.Move(args.Id, args.Dir);
        _enemyAI.MeleeAttack += (sender, args) => _physic.Attack(args.Id);
        _enemyAI.RangeAttack += (sender, args) => _physic.Shoot(args.Id);
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
        
        if (_currentScreen == "GamePlay")
            _enemyAI.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _screens[_currentScreen].Draw(gameTime);

        base.Draw(gameTime);
    }
}