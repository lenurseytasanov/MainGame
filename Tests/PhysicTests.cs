using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Misc;
using MainGame.Models;
using MainGame.Models.GameObjects;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MainGame.Tests;

[TestFixture]
public class PhysicTests
{
    private readonly GameTime _gameTime = new GameTime();

    private PhysicalModel _physic;
    private Dictionary<int, IGameObject> _objects;
    private int _playerID;

    [SetUp]
    public void GameInit()
    {
        _physic = new PhysicalModel();
        _physic.Updated += (sender, args) =>
        {
            _playerID = args.Level.PlayerId;
            _objects = args.Level.Objects;
        };
        _physic.Initialize();
    }


    [TestCase(Direction.Left)]
    [TestCase(Direction.Right)]
    public void MovingTest(Direction direction)
    {
        var startPosition = _objects[_playerID].Position.X;
        for (var i = 0; i < 5; i++)
        {
            _physic.Move(_playerID, direction);
        }
        _physic.Update(_gameTime);
        var endPosition = _objects[_playerID].Position.X;
        Assert.IsTrue(direction == Direction.Left ? endPosition < startPosition : endPosition > startPosition);
    }

    [Test]
    public void JumpTest()
    {
        var startPosition = _objects[_playerID].Position.Y;
        _physic.Move(_playerID, Direction.Up);
        for (var i = 0; i < 60; i++)
            _physic.Update(_gameTime);
        var endPosition = _objects[_playerID].Position.Y;
        Assert.AreEqual(startPosition, endPosition, 30);

    }
}