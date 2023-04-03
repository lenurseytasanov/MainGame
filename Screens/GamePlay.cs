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
        private Dictionary<int, Sprite> _sprites;

        public override void Initialize()
        {
            _playerId = 1; //danger
            _sprites = new Dictionary<int, Sprite>
            {
                { 1, new KnightSprite() },
                { 2, new Sprite() }
            };

            foreach (var sprite in _sprites.Values.OfType<AnimatedSprite>())
            {
                sprite.Initialize();
            }
            _sprites[1].Draw = (sender, sb) =>
            {
                var o = sender as KnightSprite;
                sb.Draw(
                    o.AnimationManager.CurrentAnimation.SpriteSheet,
                    new Vector2(350, o.Position.Y - 20) +
                    (o.AnimationManager.CurrentAnimation.Effects == SpriteEffects.FlipHorizontally
                        ? new Vector2(-90, 0)
                        : Vector2.Zero),
                    o.AnimationManager.CurrentFrame, Color.White,
                    0, Vector2.Zero, o.AnimationManager.CurrentAnimation.Scale,
                    o.AnimationManager.CurrentAnimation.Effects,
                    0);
            };
            _sprites[2].Draw = (sender, sb) =>
            {
                var o = sender as Sprite;
                var effects = SpriteEffects.None;
                for (var i = 0; i < 1000; i++)
                {
                    sb.Draw(o.Texture, new Vector2(o.Position.X - _sprites[_playerId].Position.X, o.Position.Y), o.Texture.Bounds, Color.White,
                        0, Vector2.Zero, 1, effects ^= SpriteEffects.FlipHorizontally, 1);
                    o.Position += new Vector2(o.Texture.Width, 0);
                }
            };
        }

        public override void LoadContent(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;

            (_sprites[1] as AnimatedSprite)!.LoadAnimations(new Dictionary<string, Animation>()
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
            });
            

            _sprites[2].Texture = Game.Content.Load<Texture2D>("ground/rock");
        }

        public override void Update(GameTime gameTime)
        {
            var keys = Keyboard.GetState().GetPressedKeys();

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

        public void LoadParameters(Dictionary<int, IGameObject> gameObjects)
        {
            foreach (var key in gameObjects.Values.Select(o => o.SpriteId))
            {
                _sprites[key].Position = gameObjects[key].Position;
                if (_sprites[key] is KnightSprite knight)
                {
                    knight.Direction = gameObjects[key].Direction;
                    knight.OnGround = (gameObjects[key] as Knight).OnGround;
                }
            }
        }

        public event EventHandler<ControlEventArgs> PlayerMoved;

        public event EventHandler CycleFinished;
    }
}
