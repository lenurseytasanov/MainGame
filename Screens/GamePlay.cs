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

        private int _playerId;

        private Dictionary<int, Sprite> _sprites = new Dictionary<int, Sprite>();

        public override void Initialize()
        {
            foreach (var sprite in _sprites.Values.OfType<AnimatedSprite>())
            {
                sprite.Initialize();
            }
        }

        public override void LoadContent(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
        }

        public override void Update(GameTime gameTime)
        {
            var keys = Keyboard.GetState().GetPressedKeys();

            (_sprites[3] as AnimatedSprite).SetAnimation("IdleRight");
            var player = _sprites[_playerId] as KnightSprite;
            if (!player.OnGround)
                player.SetAnimation(player.Direction == Direction.Right ? "JumpRight" : "JumpLeft");
            else
                player.SetAnimation(player.Direction == Direction.Right ? "IdleRight" : "IdleLeft");
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
                }
            }

            foreach (var sprite in _sprites.Values.OfType<AnimatedSprite>())
            {
                sprite.Update(gameTime);
            }

            CycleFinished?.Invoke(this, EventArgs.Empty);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.Aqua);

            _spriteBatch.Begin(SpriteSortMode.BackToFront);
            foreach (var sprite in _sprites.Values)
            {
                sprite.Draw(sprite, _spriteBatch);
            }
            _spriteBatch.End();
        }

        public void LoadStartParameters(Dictionary<int, IGameObject> gameObjects, int playerId)
        {
            _playerId = playerId;

            foreach (var o in gameObjects.Where(o => !_sprites.ContainsKey(o.Key)))
            {
                switch (o.Value.SpriteId)
                {
                    case 1:
                        _sprites.Add(o.Key, new KnightSprite()
                        {
                            Draw = (sender, sb) =>
                            {
                                var sprite = sender as AnimatedSprite;
                                sb.Draw(
                                    sprite.AnimationManager.CurrentAnimation.SpriteSheet,
                                    new Vector2(
                                        o.Key == _playerId ? 350 : 350 + sprite.Position.X - gameObjects[playerId].Position.X,
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
                                    new Animation() { SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Idle"), FrameCount = 4, Scale = 1.5f }
                                },
                                {
                                    "IdleLeft",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Idle"), FrameCount = 4, Scale = 1.5f,
                                        Effects = SpriteEffects.FlipHorizontally
                                    }
                                },
                                {
                                    "RunRight",
                                    new Animation() { SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Run"), FrameCount = 7, Scale = 1.5f  }
                                },
                                {
                                    "RunLeft",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Run"), FrameCount = 7, Scale = 1.5f,
                                        Effects = SpriteEffects.FlipHorizontally
                                    }
                                },
                                {
                                    "JumpRight",
                                    new Animation() { SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Jump"), FrameCount = 6, Scale = 1.5f }
                                },
                                {
                                    "JumpLeft",
                                    new Animation()
                                    {
                                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Jump"), FrameCount = 6, Scale = 1.5f,
                                        Effects = SpriteEffects.FlipHorizontally
                                    }
                                }
                            }
                        });
                        break;
                    case 2:
                        _sprites.Add(o.Key, new Sprite()
                        {
                            Draw = (sender, sb) =>
                            {
                                var sprite = sender as Sprite;
                                var effects = SpriteEffects.None;
                                for (var i = 0; i < 1000; i++)
                                {
                                    sb.Draw(sprite.Texture, new Vector2(350 + sprite.Position.X - gameObjects[playerId].Position.X, sprite.Position.Y), sprite.Texture.Bounds, Color.White,
                                        0, Vector2.Zero, 1, effects ^= SpriteEffects.FlipHorizontally, 1);
                                    sprite.Position += new Vector2(sprite.Texture.Width, 0);
                                }
                            },
                            Texture = Game.Content.Load<Texture2D>("ground/rock")
                        });
                        break;
                }
            }
        }

        public void LoadParameters(Dictionary<int, IGameObject> gameObjects)
        {
            foreach (var o in gameObjects)
            {
                
                _sprites[o.Key].Position = gameObjects[o.Key].Position;
                if (_sprites[o.Key] is KnightSprite knight)
                {
                    knight.Direction = gameObjects[o.Key].Direction;
                    knight.OnGround = (gameObjects[o.Key] as Knight).OnGround;
                }
            }
        }

        public event EventHandler<ControlEventArgs> PlayerMoved;

        public event EventHandler CycleFinished;
    }
}
