using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using MainGame.Managers;
using MainGame.Misc;
using MainGame.Models.GameObjects;
using MainGame.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MainGame.Screens
{
    public class GamePlay : Screen
    {
        private SpriteFactory _spriteFactory;

        public Rectangle LevelSize { get; private set; }

        private int WindowWidth => Game.GraphicsDevice.PresentationParameters.BackBufferWidth;
        private int WindowHeight => Game.GraphicsDevice.PresentationParameters.BackBufferHeight;

        private int _playerId;
        private Vector2 _screenCenter;

        private readonly Dictionary<int, int> _spriteTypeToId = new Dictionary<int, int>();
        private readonly Dictionary<int, Sprite> _sprites = new Dictionary<int, Sprite>();

        public void Reset()
        {
            _sprites.Clear();
            _spriteTypeToId.Clear();
        }

        public override void Initialize()
        {
            _screenCenter = new Vector2(WindowWidth / 2, WindowHeight / 2);
        }

        public override void Update(GameTime gameTime)
        {
            var keys = Keyboard.GetState().GetPressedKeys();

            foreach (var key in keys)
            {
                switch (key)
                {
                    case Keys.Left:
                        Moved?.Invoke(this, new MoveEventArgs() { Id = _playerId, Dir = Direction.Left });
                        break;
                    case Keys.Right:
                        Moved?.Invoke(this, new MoveEventArgs() { Id = _playerId, Dir = Direction.Right });
                        break;
                    case Keys.Up:
                        Moved?.Invoke(this, new MoveEventArgs() { Id = _playerId, Dir = Direction.Up });
                        break;
                    case Keys.A:
                        Attacked?.Invoke(this, new AttackEventArgs() { Id = _playerId });
                        break;
                    case Keys.Escape:
                        Game.Exit();
                        break;
                }
            }

            CycleFinished?.Invoke(this, new CycleEventArgs() { ElapsedTime = gameTime });
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var pair in _sprites.Where(pair => pair.Value is CharacterSprite))
            {
                var chr = pair.Value as CharacterSprite;
                chr.SetAnimations(_spriteTypeToId[pair.Key]);
                chr.Update(gameTime);
            }

            SpriteBatch.Begin(SpriteSortMode.BackToFront);
            foreach (var sprite in _sprites.Values)
            {
                sprite.Draw(SpriteBatch, GetPlayerShift());
            }
            SpriteBatch.End();
        }

        private Vector2 GetPlayerShift()
        {
            var shift = _screenCenter - _sprites[_playerId].Position;
            if (_sprites[_playerId].Position.X < _screenCenter.X)
                shift.X = 0;
            if (_sprites[_playerId].Position.X > LevelSize.Width - _screenCenter.X)
                shift.X = WindowWidth - LevelSize.Width;

            shift.Y = WindowHeight - LevelSize.Height;
            return shift;
        }

        public void LoadParameters(Dictionary<int, IGameObject> gameObjects, int playerId, Rectangle levelSize)
        {
            _playerId = playerId;
            LevelSize = levelSize;

            if (!_sprites.ContainsKey(0))
            {
                _spriteFactory = new BackgroundFactory(Game.Content);
                var sprite = _spriteFactory.CreateSprite(0);
                sprite.Size = new Rectangle(0, 0, WindowWidth, WindowHeight);
                _sprites.Add(0, sprite);
            }

            if (!_sprites.ContainsKey(-1))
            {
                _spriteFactory = new BarsFactory(Game.Content);
                var sprite = _spriteFactory.CreateSprite(-1);
                _sprites.Add(-1, sprite);
            }

            foreach (var o in gameObjects.Where(o => !_sprites.ContainsKey(o.Key)))
            {
                _spriteFactory = o.Value.SpriteId switch
                {
                    >= 1 and <= 11 => new CharacterSpriteFactory(Game.Content),
                    >= 12 => new StaticSpriteFactory(Game.Content)
                };
                var sprite = _spriteFactory.CreateSprite(o.Value.SpriteId);
                _spriteTypeToId.Add(o.Key, o.Value.SpriteId);
                _sprites.Add(o.Key, sprite);
            }

            foreach (var o in gameObjects)
            {
                _sprites[o.Key].Position = gameObjects[o.Key].Position;
                _sprites[o.Key].Size = gameObjects[o.Key].Size;
                if (_sprites[o.Key] is CharacterSprite chrSpr)
                {
                    var chr = gameObjects[o.Key] as Character;
                    chrSpr.Direction = chr.Direction;
                    if ((chrSpr.State & StateCharacter.Attacking) != 0 &&
                        (chr.State & StateCharacter.Attacking) == 0)
                    {
                        chrSpr.AttackNumber++;
                        chrSpr.AttackNumber %= chrSpr.AttackCount;
                    }
                    chrSpr.State = chr.State;
                    if (o.Key == playerId)
                    {
                        if ((chr.State & StateCharacter.Dead) != 0)
                        {
                            PlayerDead?.Invoke(this, EventArgs.Empty);
                            return;
                        }
                        (_sprites[-1] as StateSprite).CurrentState = chr.HealthPoints == 10 ? 10 : chr.HealthPoints % 10;
                    }
                }
            }
        }

        public event EventHandler PlayerDead;

        public event EventHandler<AttackEventArgs> Attacked; 

        public event EventHandler<MoveEventArgs> Moved;

        public event EventHandler<CycleEventArgs> CycleFinished;
    }
}
