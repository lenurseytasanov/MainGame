using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Misc;
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
                if (o.IsDead)
                    o.SetAnimation(o.Direction == Direction.Right ? "DeadRight" : "DeadLeft");
                else if (o.IsHurt)
                    o.SetAnimation(o.Direction == Direction.Right ? "HurtRight" : "HurtLeft");
                else if (o.Attack != Attack.None)
                    switch (o.Attack)
                    {
                        case Attack.DownSlash:
                            o.SetAnimation(o.Direction == Direction.Right ? "AttackSlash1Right" : "AttackSlash1Left");
                            break;
                        case Attack.UpSlash:
                            o.SetAnimation(o.Direction == Direction.Right ? "AttackSlash2Right" : "AttackSlash2Left");
                            break;
                        case Attack.Pierce:
                            o.SetAnimation(o.Direction == Direction.Right ? "AttackPierceRight" : "AttackPierceLeft");
                            break;
                    }
                    
                else if (!o.IsOnGround)
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
                        player.SetAnimation(player.Attack == Attack.None ? "RunLeft" : "AttackRunLeft");
                        PlayerMoved?.Invoke(this, new ControlEventArgs() { Dir = Direction.Left });
                        break;
                    case Keys.Right:
                        player.SetAnimation(player.Attack == Attack.None ? "RunRight" : "AttackRunRight");
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
                                    "AttackSlash1Right",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Attack 1"), FrameCount = 5,
                                        Scale = 1.5f
                                    }
                                },
                                {
                                    "AttackSlash1Left",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Attack 1"), FrameCount = 5,
                                        Scale = 1.5f,
                                        Effects = SpriteEffects.FlipHorizontally
                                    }
                                },
                                {
                                    "AttackSlash2Right",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Attack 2"), FrameCount = 4,
                                        Scale = 1.5f
                                    }
                                },
                                {
                                    "AttackSlash2Left",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Attack 2"), FrameCount = 4,
                                        Scale = 1.5f,
                                        Effects = SpriteEffects.FlipHorizontally
                                    }
                                },
                                {
                                    "AttackPierceRight",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Attack 3"), FrameCount = 4,
                                        Scale = 1.5f
                                    }
                                },
                                {
                                    "AttackPierceLeft",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Attack 3"), FrameCount = 4,
                                        Scale = 1.5f,
                                        Effects = SpriteEffects.FlipHorizontally
                                    }
                                },
                                {
                                    "AttackRunRight",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Run+Attack"), FrameCount = 6,
                                        Scale = 1.5f
                                    }
                                },
                                {
                                    "AttackRunLeft",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Run+Attack"), FrameCount = 6,
                                        Scale = 1.5f,
                                        Effects = SpriteEffects.FlipHorizontally
                                    }
                                },
                                {
                                    "HurtRight",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Hurt"), FrameCount = 2,
                                        Scale = 1.5f
                                    }
                                },
                                {
                                    "HurtLeft",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Hurt"), FrameCount = 2,
                                        Scale = 1.5f,
                                        Effects = SpriteEffects.FlipHorizontally
                                    }
                                },
                                {
                                    "DeadRight",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Dead"), FrameCount = 6,
                                        Scale = 1.5f, IsLooping = false
                                    }
                                },
                                {
                                    "DeadLeft",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Dead"), FrameCount = 6,
                                        Scale = 1.5f, IsLooping = false,
                                        Effects = SpriteEffects.FlipHorizontally
                                    }
                                },
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
                                sb.Draw(sprite.Texture, new Vector2(_playerPosition.X + sprite.Position.X - gameObjects[playerId].Position.X, sprite.Position.Y), sprite.Texture.Bounds, Color.White,
                                    0, Vector2.Zero, 1, effects ^= SpriteEffects.FlipHorizontally, 1);
                                sprite.Position += new Vector2(sprite.Texture.Width, 0);
                            },
                            Texture = Game.Content.Load<Texture2D>("ground/rock")
                        });
                        break;
                }
            }
            foreach (var o in gameObjects)
            {
                _sprites[o.Key].Position = gameObjects[o.Key].Position;
                if (_sprites[o.Key] is KnightSprite knightSprite)
                {
                    var knight = gameObjects[o.Key] as Knight;
                    knightSprite.Direction = knight.Direction;
                    knightSprite.IsOnGround = knight.IsOnGround;
                    knightSprite.Attack = knight.Attack;
                    knightSprite.IsDead = knight.HealthPoints <= 0;
                    knightSprite.IsHurt = knight.IsHurt;
                }
            }
        }

        public event EventHandler PlayerAttacked; 

        public event EventHandler<ControlEventArgs> PlayerMoved;

        public event EventHandler<CycleEventArgs> CycleFinished;
    }
}
