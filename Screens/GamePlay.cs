using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using MainGame.Managers;
using MainGame.Misc;
using MainGame.Models;
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

        private Rectangle _levelSize;

        private int WindowWidth => Game.GraphicsDevice.PresentationParameters.BackBufferWidth;
        private int WindowHeight => Game.GraphicsDevice.PresentationParameters.BackBufferHeight;

        private int _playerId;
        private Vector2 _screenCenter;

        private bool _screenChangingInvoked;

        private readonly Dictionary<int, int> _spriteTypeToId = new Dictionary<int, int>();
        private readonly Dictionary<int, Sprite> _sprites = new Dictionary<int, Sprite>();

        public void Reset()
        {
            _sprites.Clear();
            _spriteTypeToId.Clear();
            _screenChangingInvoked = false;
        }

        public override void Initialize()
        {
            _screenCenter = new Vector2(WindowWidth / 2, WindowHeight / 2);
        }

        public GamePlay(Game game) : base(game) { }

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
            if (_sprites[_playerId].Position.X > _levelSize.Width - _screenCenter.X)
                shift.X = WindowWidth - _levelSize.Width;

            shift.Y = WindowHeight - _levelSize.Height;
            return shift;
        }

        public void LoadParameters(LevelModel level)
        {
            _playerId = level.PlayerId;
            _levelSize = level.LevelSize;

            if (!_sprites.ContainsKey(level.BackgroundId))
            {
                _spriteFactory = new BackgroundFactory(Game.Content);
                var sprite = _spriteFactory.CreateSprite(level.BackgroundId);
                sprite.Size = new Rectangle(0, 0, WindowWidth, WindowHeight);
                _sprites.Add(level.BackgroundId, sprite);
            }

            if (!_sprites.ContainsKey(-1))
            {
                _spriteFactory = new BarsFactory(Game.Content);
                var sprite = _spriteFactory.CreateSprite(-1);
                _sprites.Add(-1, sprite);
            }

            if (level.BossId > 0 && !_sprites.ContainsKey(-2))
            {
                _spriteFactory = new BarsFactory(Game.Content);
                var sprite = _spriteFactory.CreateSprite(-2);
                _sprites.Add(-2, sprite);
            }

            foreach (var o in level.Objects.Where(o => !_sprites.ContainsKey(o.Key)))
            {
                _spriteFactory = o.Value.SpriteId switch
                {
                    >= 1 and <= 11 => new CharacterSpriteFactory(Game.Content),
                    >= 12 => new StaticSpriteFactory(Game.Content)
                };
                var sprite = _spriteFactory.CreateSprite(o.Value.SpriteId);
                _spriteTypeToId.Add(o.Key, o.Value.SpriteId);
                _sprites.Add(o.Key, sprite);

                if (o.Value.SpriteId == 50)
                {
                    var speed = (level.Objects[o.Key] as Fireball)!.Speed;
                    var basic = new Vector2(1, 0);
                    _sprites[o.Key].Rotation = (float)Math.Atan2(speed.Y - basic.Y, speed.X - basic.X) - (float)Math.PI / 2;
                }
            }

            foreach (var o in level.Objects)
            {
                _sprites[o.Key].Position = level.Objects[o.Key].Position;
                _sprites[o.Key].Size = level.Objects[o.Key].Size;
                if (_sprites[o.Key] is CharacterSprite chrSpr)
                {
                    var chr = level.Objects[o.Key] as Character;
                    chrSpr.Direction = chr.Direction;
                    if ((chrSpr.State & StateCharacter.Attacking) != 0 &&
                        (chr.State & StateCharacter.Attacking) == 0)
                    {
                        chrSpr.AttackNumber++;
                        chrSpr.AttackNumber %= chrSpr.AttackCount;
                    }
                    chrSpr.State = chr.State;
                    if (o.Key == level.PlayerId)
                    {
                        if (!_screenChangingInvoked && (chr.State & StateCharacter.Dead) != 0)
                        {
                            PlayerDead?.Invoke(this, EventArgs.Empty);
                            _screenChangingInvoked = true;
                        }
                        (_sprites[-1] as StateSprite).CurrentState = 10 * chr.HealthPoints / 10;
                    }
                    if (o.Key == level.BossId)
                    {
                        if (!_screenChangingInvoked && (chr.State & StateCharacter.Dead) != 0)
                        {
                            BossDead?.Invoke(this, EventArgs.Empty);
                            _screenChangingInvoked = true;
                        }
                        (_sprites[-2] as StateSprite).CurrentState = 10 * chr.HealthPoints / 20;
                    }
                }
            }
        }

        public event EventHandler PlayerDead;

        public event EventHandler BossDead;

        public event EventHandler<AttackEventArgs> Attacked; 

        public event EventHandler<MoveEventArgs> Moved;

        public event EventHandler<CycleEventArgs> CycleFinished;
    }
}
