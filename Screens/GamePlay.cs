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
                { 2, new GroundSprite() }
            };

            foreach (var sprite in _sprites.Values.OfType<AnimatedSprite>())
            {
                sprite.Initialize();
            }
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
                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Idle"), FrameCount = 4,
                        Effects = SpriteEffects.FlipHorizontally,
                        Scale = 1.5f
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
                        SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Run"), FrameCount = 7,
                        Effects = SpriteEffects.FlipHorizontally,
                        Scale = 1.5f
                    }
                }
            });

            _sprites[2].Texture = Game.Content.Load<Texture2D>("ground/rock");
        }

        public override void Update(GameTime gameTime)
        {
            var keys = Keyboard.GetState().GetPressedKeys();

            var player = _sprites[_playerId] as AnimatedSprite;
            player.SetAnimation(player.Direction == Direction.Right ? "IdleRight" : "IdleLeft");
            foreach (var key in keys)
            {
                switch (key)
                {
                    case Keys.Left:
                        player.SetAnimation("RunLeft");
                        PlayerMoved?.Invoke(this, new ControlEventArgs() { Dir = Direction.Left });
                        break;
                    case Keys.Right:
                        player.SetAnimation("RunRight");
                        PlayerMoved?.Invoke(this, new ControlEventArgs() { Dir = Direction.Right });
                        break;
                    case Keys.Up:
                        //player.SetAnimation("Idle");
                        PlayerMoved?.Invoke(this, new ControlEventArgs() { Dir = Direction.Up });
                        break;
                    case Keys.Down:
                        //player.SetAnimation("Idle");
                        PlayerMoved?.Invoke(this, new ControlEventArgs() { Dir = Direction.Down });
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
                sprite.Draw(gameTime, _spriteBatch);
            }
            _spriteBatch.End();
        }

        public void LoadParameters(Dictionary<int, IGameObject> gameObjects)
        {
            foreach (var key in gameObjects.Keys)
            {
                _sprites[key].Position = gameObjects[key].Position;
                if (_sprites[key] is KnightSprite)
                    _sprites[key].Direction = gameObjects[key].Direction;
            }
        }

        public event EventHandler<ControlEventArgs> PlayerMoved;

        public event EventHandler CycleFinished;
    }
}
