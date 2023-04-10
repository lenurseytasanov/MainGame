using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public int LevelWidth { get; set; }
        public int LevelHeight { get; set; }

        private int _playerId;

        private Dictionary<int, int> _spriteTypeToId = new Dictionary<int, int>();

        private Dictionary<int, Sprite> _sprites = new Dictionary<int, Sprite>();
        private Vector2 _playerPosition;

        public override void Initialize()
        {
            _playerPosition = new Vector2(Graphics.PreferredBackBufferWidth / 2, 0);
        }

        public override void LoadContent(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            _sprites.Add(0, new Sprite()
            {
                Position = Vector2.Zero,
                Size = new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight),
                Draw = (sender, sb) =>
                {
                    var sprite = sender as Sprite;
                    var shiftOnPlayer = (int)_playerPosition.X - (int)_sprites[_playerId].Position.X;
                    sb.Draw(sprite.Texture,
                        new Rectangle(
                            (int)sprite.Position.X,
                            (int)sprite.Position.Y,
                            sprite.Size.Width, sprite.Size.Height),
                        sprite.Texture.Bounds, Color.White,
                        0, Vector2.Zero, SpriteEffects.None, 1);
                },
                Texture = Game.Content.Load<Texture2D>("dungeon/Background/Pale/Background")
            });
        }

        public override void Update(GameTime gameTime)
        {
            var keys = Keyboard.GetState().GetPressedKeys();

            foreach (var pair in _sprites.Where(pair => pair.Value is CharacterSprite))
            {
                var o = pair.Value as CharacterSprite;
                if ((o.State & StateCharacter.Dead) != 0)
                    o.SetAnimation(o.Direction == Direction.Right ? "DeadRight" : "DeadLeft");
                else if ((o.State & StateCharacter.Hurt) != 0)
                    o.SetAnimation(o.Direction == Direction.Right ? "HurtRight" : "HurtLeft");
                else if ((o.State & StateCharacter.OnGround) == 0)
                    o.SetAnimation(o.Direction == Direction.Right ? "JumpRight" : "JumpLeft");
                else if ((o.State & StateCharacter.Standing) == 0)
                    o.SetAnimation(o.Direction == Direction.Right ? "RunRight" : "RunLeft");
                else
                    o.SetAnimation(o.Direction == Direction.Right ? "IdleRight" : "IdleLeft");
                switch (_spriteTypeToId[pair.Key])
                {
                    case 1:
                        if ((o.State & StateCharacter.Standing) != 0 && (o.State & StateCharacter.Attacking) != 0)
                            switch (o.AttackNumber)
                            {
                                case 0:
                                    o.SetAnimation(o.Direction == Direction.Right ? "AttackSlash1Right" : "AttackSlash1Left");
                                    break;
                                case 1:
                                    o.SetAnimation(o.Direction == Direction.Right ? "AttackSlash2Right" : "AttackSlash2Left");
                                    break;
                                case 2:
                                    o.SetAnimation(o.Direction == Direction.Right ? "AttackPierceRight" : "AttackPierceLeft");
                                    break;
                            }
                        else if ((o.State & StateCharacter.Attacking) != 0)
                            o.SetAnimation(o.Direction == Direction.Right ? "AttackRunRight" : "AttackRunLeft");
                        break;
                    case 2:
                        if ((o.State & StateCharacter.Standing) != 0 && (o.State & StateCharacter.Attacking) != 0)
                            switch (o.AttackNumber)
                            {
                                case 0:
                                    o.SetAnimation(o.Direction == Direction.Right ? "Attack1Right" : "Attack1Left");
                                    break;
                            }
                        else if ((o.State & StateCharacter.Attacking) != 0)
                            o.SetAnimation(o.Direction == Direction.Right ? "AttackRunRight" : "AttackRunLeft");
                        break;
                }
            }

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

            _spriteBatch.Begin(SpriteSortMode.BackToFront, samplerState: SamplerState.LinearWrap);
            foreach (var sprite in _sprites.Values)
            {
                sprite.Draw?.Invoke(sprite, _spriteBatch);
            }
            _spriteBatch.End();
        }

        public void LoadParameters(Dictionary<int, IGameObject> gameObjects, int playerId)
        {
            _playerId = playerId;

            foreach (var o in gameObjects.Where(o => !_sprites.ContainsKey(o.Key)))
            {
                switch (o.Value.SpriteId)
                {
                    case >= 1 and <= 11:
                        var sprite = new CharacterSprite()
                        {
                            Draw = (sender, sb) =>
                            {
                                var sprite = sender as AnimatedSprite;
                                var shiftOnPlayer = (int)_playerPosition.X - (int)_sprites[_playerId].Position.X;
                                if (_sprites[_playerId].Position.X < _playerPosition.X)
                                    shiftOnPlayer = 0;
                                if (_sprites[_playerId].Position.X > LevelWidth - _playerPosition.X)
                                    shiftOnPlayer = Graphics.PreferredBackBufferWidth - LevelWidth;
                                sb.Draw(
                                    sprite.AnimationManager.CurrentAnimation.SpriteSheet,
                                    new Rectangle(
                                        shiftOnPlayer + (int)sprite.Position.X + 
                                        (o.Value.SpriteId == 1 
                                            ? (sprite.AnimationManager.CurrentAnimation.Effects == SpriteEffects.FlipHorizontally
                                                ? -50
                                                : 50) 
                                            : 0),
                                        (int)sprite.Position.Y,
                                        sprite.Size.Width, sprite.Size.Height),
                                    sprite.AnimationManager.CurrentFrame, Color.White,
                                    0, Vector2.Zero,
                                    sprite.AnimationManager.CurrentAnimation.Effects, 0);
                            },
                            Animations = o.Value.SpriteId switch
                            {
                                1 => new Dictionary<string, Animation>()
                                {
                                    {
                                        "IdleRight",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Idle"), FrameCount = 4,
                                        }
                                    },
                                    {
                                        "IdleLeft",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Idle"), FrameCount = 4,
                                            Effects = SpriteEffects.FlipHorizontally
                                        }
                                    },
                                    {
                                        "RunRight",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Run"), FrameCount = 7,
                                        }
                                    },
                                    {
                                        "RunLeft",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Run"), FrameCount = 7,
                                            Effects = SpriteEffects.FlipHorizontally
                                        }
                                    },
                                    {
                                        "JumpRight",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Jump"), FrameCount = 6,
                                        }
                                    },
                                    {
                                        "JumpLeft",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Jump"), FrameCount = 6,
                                            Effects = SpriteEffects.FlipHorizontally
                                        }
                                    },
                                    {
                                        "AttackSlash1Right",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Attack 1"), FrameCount = 5,
                                        }
                                    },
                                    {
                                        "AttackSlash1Left",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Attack 1"), FrameCount = 5,
                                            Effects = SpriteEffects.FlipHorizontally
                                        }
                                    },
                                    {
                                        "AttackSlash2Right",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Attack 2"), FrameCount = 4
                                        }
                                    },
                                    {
                                        "AttackSlash2Left",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Attack 2"), FrameCount = 4,
                                            Effects = SpriteEffects.FlipHorizontally
                                        }
                                    },
                                    {
                                        "AttackPierceRight",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Attack 3"), FrameCount = 4,
                                        }
                                    },
                                    {
                                        "AttackPierceLeft",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Attack 3"), FrameCount = 4,
                                            Effects = SpriteEffects.FlipHorizontally
                                        }
                                    },
                                    {
                                        "AttackRunRight",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Run+Attack"), FrameCount = 6,
                                        }
                                    },
                                    {
                                        "AttackRunLeft",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Run+Attack"), FrameCount = 6,
                                            Effects = SpriteEffects.FlipHorizontally
                                        }
                                    },
                                    {
                                        "HurtRight",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Hurt"), FrameCount = 2,
                                        }
                                    },
                                    {
                                        "HurtLeft",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Hurt"), FrameCount = 2,
                                            Effects = SpriteEffects.FlipHorizontally
                                        }
                                    },
                                    {
                                        "DeadRight",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Dead"), FrameCount = 6, 
                                            IsLooping = false
                                        }
                                    },
                                    {
                                        "DeadLeft",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Knight_1/Dead"), FrameCount = 6,
                                            IsLooping = false,
                                            Effects = SpriteEffects.FlipHorizontally
                                        }
                                    },
                                },
                                2 => new Dictionary<string, Animation>()
                                {
                                    {
                                        "IdleRight",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Orc/Orc_Berserk/Idle"), FrameCount = 5
                                        }
                                    },
                                    {
                                        "IdleLeft",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Orc/Orc_Berserk/Idle"), FrameCount = 5,
                                            Effects = SpriteEffects.FlipHorizontally
                                        }
                                    },
                                    {
                                        "RunRight",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Orc/Orc_Berserk/Run"), FrameCount = 6
                                        }
                                    },
                                    {
                                        "RunLeft",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Orc/Orc_Berserk/Run"), FrameCount = 6,
                                            Effects = SpriteEffects.FlipHorizontally
                                        }
                                    },
                                    {
                                        "JumpRight",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Orc/Orc_Berserk/Jump"), FrameCount = 5
                                        }
                                    },
                                    {
                                        "JumpLeft",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Orc/Orc_Berserk/Jump"), FrameCount = 5,
                                            Effects = SpriteEffects.FlipHorizontally
                                        }
                                    },
                                    {
                                        "HurtRight",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Orc/Orc_Berserk/Hurt"), FrameCount = 2
                                        }
                                    },
                                    {
                                        "HurtLeft",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Orc/Orc_Berserk/Hurt"), FrameCount = 2,
                                            Effects = SpriteEffects.FlipHorizontally
                                        }
                                    },
                                    {
                                        "DeadRight",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Orc/Orc_Berserk/Dead"), FrameCount = 4,
                                            IsLooping = false
                                        }
                                    },
                                    {
                                        "DeadLeft",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Orc/Orc_Berserk/Dead"), FrameCount = 4,
                                            IsLooping = false,
                                            Effects = SpriteEffects.FlipHorizontally
                                        }
                                    },
                                    {
                                        "AttackRunRight",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Orc/Orc_Berserk/Run+Attack"), FrameCount = 5,
                                        }
                                    },
                                    {
                                        "AttackRunLeft",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Orc/Orc_Berserk/Run+Attack"), FrameCount = 5,
                                            Effects = SpriteEffects.FlipHorizontally
                                        }
                                    },
                                    {
                                        "Attack1Right",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Orc/Orc_Berserk/Attack_1"), FrameCount = 4,
                                        }
                                    },
                                    {
                                        "Attack1Left",
                                        new Animation()
                                        {
                                            SpriteSheet = Game.Content.Load<Texture2D>("Orc/Orc_Berserk/Attack_1"), FrameCount = 4,
                                            Effects = SpriteEffects.FlipHorizontally
                                        }
                                    },
                                }
                            }
                        };
                        sprite.Initialize();
                        _spriteTypeToId.Add(o.Key, o.Value.SpriteId);
                        _sprites.Add(o.Key, sprite);
                        break;
                    case >= 12:
                        _sprites.Add(o.Key, new Sprite()
                        {
                            Draw = (sender, sb) =>
                            {
                                var sprite = sender as Sprite;
                                var shiftOnPlayer = (int)_playerPosition.X - (int)_sprites[_playerId].Position.X;
                                if (_sprites[_playerId].Position.X < _playerPosition.X)
                                    shiftOnPlayer = 0;
                                if (_sprites[_playerId].Position.X > LevelWidth - _playerPosition.X)
                                    shiftOnPlayer = Graphics.PreferredBackBufferWidth - LevelWidth;
                                sb.Draw(sprite.Texture,
                                    new Rectangle(
                                        shiftOnPlayer + (int)sprite.Position.X,
                                        (int)sprite.Position.Y,
                                        sprite.Size.Width, sprite.Size.Height),
                                    sprite.Texture.Bounds, Color.White,
                                    0, Vector2.Zero, SpriteEffects.None, 0.5f);
                            },
                            Texture = Game.Content.Load<Texture2D>(o.Value.SpriteId switch
                            {
                                12 => "dungeon/Tiles_rock/tile2",
                                13 => "dungeon/Tiles_lava/lava_tile3",
                                14 => "dungeon/Tiles_rock/tile5",
                                15 => "dungeon/Tiles_rock/tile1",
                                16 => "dungeon/Tiles_rock/tile3",
                                17 => "dungeon/Tiles_rock/tile4",
                                18 => "dungeon/Tiles_rock/tile6",
                                19 => "dungeon/Tiles_rock/tile12",
                                20 => "dungeon/Tiles_rock/tile13",
                                _ => throw new Exception("Unknown texture")
                            })
                        });
                        break;
                }
            }
            foreach (var o in gameObjects)
            {
                _sprites[o.Key].Position = gameObjects[o.Key].Position;
                _sprites[o.Key].Size = gameObjects[o.Key].Size;
                if (_sprites[o.Key] is CharacterSprite chrSpr)
                {
                    var chr = gameObjects[o.Key] as Character;
                    chrSpr.Direction = chr.Direction;
                    chrSpr.State = chr.State;
                    chrSpr.AttackNumber = chr.AttackNumber;
                }
            }
        }

        private void DrawRepeatableTexture(object sender, SpriteBatch sb)
        {
            var sprite = sender as Sprite;
            var effects = SpriteEffects.None;

            var shiftOnPlayer = (int)_playerPosition.X - (int)_sprites[_playerId].Position.X;
            var textureCols = sprite.Size.Width / sprite.Texture.Width;
            var textureRows = sprite.Size.Height / sprite.Texture.Height;
            for (var i = 0; i < textureCols; i++)
                for (var j = 0; j < textureRows; j++)
                    sb.Draw(sprite.Texture,
                        new Rectangle(
                            shiftOnPlayer + (int)sprite.Position.X + i * sprite.Texture.Width,
                            (int)sprite.Position.Y + j * sprite.Texture.Height,
                            sprite.Texture.Width, sprite.Texture.Height),
                        sprite.Texture.Bounds, Color.White,
                        0, Vector2.Zero, effects ^= SpriteEffects.FlipHorizontally, 1);

            if (sprite.Size.Height % sprite.Texture.Height > 0)
                for (var i = 0; i < textureCols; i++)
                    sb.Draw(sprite.Texture,
                        new Rectangle(shiftOnPlayer + (int)sprite.Position.X + i * sprite.Texture.Width, (int)sprite.Position.Y + textureRows * sprite.Texture.Height,
                            sprite.Texture.Width, sprite.Size.Height - textureRows * sprite.Texture.Height),
                        sprite.Texture.Bounds, Color.White,
                        0, Vector2.Zero, effects ^= SpriteEffects.FlipHorizontally, 1);

            if (sprite.Size.Width % sprite.Texture.Width > 0)
                for (var i = 0; i < textureRows; i++)
                    sb.Draw(sprite.Texture,
                        new Rectangle(shiftOnPlayer + (int)sprite.Position.X + textureCols * sprite.Texture.Width, (int)sprite.Position.Y + i * sprite.Texture.Height,
                            sprite.Size.Width - textureCols * sprite.Texture.Width, sprite.Texture.Height),
                        sprite.Texture.Bounds, Color.White,
                        0, Vector2.Zero, effects ^= SpriteEffects.FlipHorizontally, 1);
            if (sprite.Size.Height % sprite.Texture.Height > 0 && sprite.Size.Width % sprite.Texture.Width > 0)
                sb.Draw(sprite.Texture,
                new Rectangle(shiftOnPlayer + (int)sprite.Position.X + textureCols * sprite.Texture.Width, (int)sprite.Position.Y + textureRows * sprite.Texture.Height,
                    sprite.Size.Width - textureCols * sprite.Texture.Width, sprite.Size.Height - textureRows * sprite.Texture.Height),
                sprite.Texture.Bounds, Color.White,
                0, Vector2.Zero, effects ^= SpriteEffects.FlipHorizontally, 1);

            sprite.Position += new Vector2(sprite.Texture.Width, 0);
        }

        public event EventHandler<AttackEventArgs> Attacked; 

        public event EventHandler<MoveEventArgs> Moved;

        public event EventHandler<CycleEventArgs> CycleFinished;
    }
}
