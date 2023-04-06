using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Models;
using MainGame.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MainGame.Screens
{
    public class GamePlay : Screen
    {
        private SpriteBatch _spriteBatch;
        public GraphicsDeviceManager Graphics { get; set; }

        private int _playerId;

        private Dictionary<int, Sprite> _sprites = new Dictionary<int, Sprite>();
        private Vector2 _playerPosition;

        public override void Initialize()
        {
            _playerPosition = new Vector2(Graphics.PreferredBackBufferWidth / 2 - 20, 0);
        }

        public override void LoadContent(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

        public override void Update(GameTime gameTime)
        {
            var keys = Keyboard.GetState().GetPressedKeys();

            foreach (var o in _sprites.Values.OfType<KnightSprite>())
            {
                if (o.IsAttacking)
                    o.SetAnimation(o.Direction == Direction.Right ? "AttackRight" : "AttackLeft");
                else if (!o.OnGround)
                    o.SetAnimation(o.Direction == Direction.Right ? "JumpRight" : "JumpLeft");
                else
                    o.SetAnimation(o.Direction == Direction.Right ? "IdleRight" : "IdleLeft");
            }

            var player = _sprites[_playerId] as KnightSprite;
            foreach (var key in keys)
            {
                switch (key)
                {
                    case Keys.Left:
                        if (player.OnGround)
                            player.SetAnimation("RunLeft");
                        PlayerMoved?.Invoke(this, new ControlEventArgs() { Dir = Direction.Left });
                        break;
                    case Keys.Right:
                        if (player.OnGround)
                            player.SetAnimation("RunRight");
                        PlayerMoved?.Invoke(this, new ControlEventArgs() { Dir = Direction.Right });
                        break;
                    case Keys.Up:
                        PlayerMoved?.Invoke(this, new ControlEventArgs() { Dir = Direction.Up });
                        break;
                    case Keys.A:
                        PlayerAttacked?.Invoke(this, EventArgs.Empty);
                        break;
                }
            }

            foreach (var sprite in _sprites.Values.OfType<AnimatedSprite>())
            {
                sprite.Update(gameTime);
            }

            CycleFinished?.Invoke(this, new CycleEventArgs() { ElapsedTime = gameTime });
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Aqua);

            _spriteBatch.Begin(SpriteSortMode.BackToFront);
            foreach (var sprite in _sprites.Values)
            {
                sprite.Draw?.Invoke(sprite, _spriteBatch);
            }
            _spriteBatch.End();
        }

        public void LoadParameters(Dictionary<int, IGameObject> gameObjects, int playerId)
        {
            _playerId = playerId;

            foreach (var o in _sprites.Where(o => !gameObjects.ContainsKey(o.Key)))
            {
                _sprites.Remove(o.Key);
            }
            foreach (var o in gameObjects.Where(o => !_sprites.ContainsKey(o.Key)))
            {
                switch (o.Value.SpriteId)
                {
                    case 1:
                        var sprite = new KnightSprite()
                        {
                            Draw = (sender, sb) =>
                            {
                                var sprite = sender as AnimatedSprite;
                                sb.Draw(
                                    sprite.AnimationManager.CurrentAnimation.SpriteSheet,
                                    new Vector2(
                                        o.Key == _playerId
                                            ? _playerPosition.X
                                            : _playerPosition.X + sprite.Position.X - gameObjects[playerId].Position.X,
                                        sprite.Position.Y - 20) +
                                    (sprite.AnimationManager.CurrentAnimation.Effects == SpriteEffects.FlipHorizontally
                                        ? new Vector2(-100, 0)
                                        : Vector2.Zero),
                                    sprite.AnimationManager.CurrentFrame, Color.White,
                                    0, Vector2.Zero, sprite.AnimationManager.CurrentAnimation.Scale,
                                    sprite.AnimationManager.CurrentAnimation.Effects, 0);
                            },
                            Animations = new Dictionary<string, Animation>()
                            {
                                {
                                    "IdleRight",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Idle"), FrameCount = 4,
                                        Scale = 1.5f
                                    }
                                },
                                {
                                    "IdleLeft",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Idle"), FrameCount = 4,
                                        Scale = 1.5f,
                                        Effects = SpriteEffects.FlipHorizontally
                                    }
                                },
                                {
                                    "RunRight",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Run"), FrameCount = 7,
                                        Scale = 1.5f
                                    }
                                },
                                {
                                    "RunLeft",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Run"), FrameCount = 7,
                                        Scale = 1.5f,
                                        Effects = SpriteEffects.FlipHorizontally
                                    }
                                },
                                {
                                    "JumpRight",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Jump"), FrameCount = 6,
                                        Scale = 1.5f
                                    }
                                },
                                {
                                    "JumpLeft",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Jump"), FrameCount = 6,
                                        Scale = 1.5f,
                                        Effects = SpriteEffects.FlipHorizontally
                                    }
                                },
                                {
                                    "AttackRight",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Attack 1"), FrameCount = 5,
                                        Scale = 1.5f
                                    }
                                },
                                {
                                    "AttackLeft",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Attack 1"), FrameCount = 5,
                                        Scale = 1.5f,
                                        Effects = SpriteEffects.FlipHorizontally
                                    }
                                }
                            }
                        };
                        sprite.Initialize();
                        _sprites.Add(o.Key, sprite);
                        break;
                    case 2:
                        _sprites.Add(o.Key, new Sprite()
                        {
                            Draw = (sender, sb) =>
                            {
                                var sprite = sender as Sprite;
                                var effects = SpriteEffects.None;
                                for (var i = 0; i < 30; i++)
                                {
                                    sb.Draw(sprite.Texture, new Vector2(_playerPosition.X + sprite.Position.X - gameObjects[playerId].Position.X, sprite.Position.Y), sprite.Texture.Bounds, Color.White,
                                        0, Vector2.Zero, 1, effects ^= SpriteEffects.FlipHorizontally, 1);
                                    sprite.Position += new Vector2(sprite.Texture.Width, 0);
                                }
                            },
                            Texture = Game.Content.Load<Texture2D>("ground/rock")
                        });
                        break;
                }
            }
            foreach (var o in gameObjects)
            {
                if (!_sprites.ContainsKey(o.Key))
                {

                }
                _sprites[o.Key].Position = gameObjects[o.Key].Position;
                if (_sprites[o.Key] is KnightSprite knightSprite)
                {
                    var knight = gameObjects[o.Key] as Knight;
                    knightSprite.Direction = knight.Direction;
                    knightSprite.OnGround = knight.OnGround;
                    knightSprite.IsAttacking = knight.IsAttacking;
                }
            }
        }

        public event EventHandler PlayerAttacked; 

        public event EventHandler<ControlEventArgs> PlayerMoved;

        public event EventHandler<CycleEventArgs> CycleFinished;
    }
}
