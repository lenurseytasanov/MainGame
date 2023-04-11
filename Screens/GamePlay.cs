using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Managers;
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

        private SpriteFactory _spriteFactory;

        public int LevelWidth { get; set; }
        public int LevelHeight { get; set; }

        private int _playerId;
        private Vector2 _playerPosition;

        private readonly Dictionary<int, int> _spriteTypeToId = new Dictionary<int, int>();
        private readonly Dictionary<int, Sprite> _sprites = new Dictionary<int, Sprite>();

        public override void Initialize()
        {
            _playerPosition = new Vector2(Graphics.PreferredBackBufferWidth / 2, 0);
        }

        public override void LoadContent(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            _spriteFactory = new BackgroundFactory(Game.Content);
            var sprite = _spriteFactory.CreateSprite(0);
            sprite.Size = new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
            _sprites.Add(0, sprite);
        }

        public override void Update(GameTime gameTime)
        {
            var keys = Keyboard.GetState().GetPressedKeys();

            foreach (var pair in _sprites.Where(pair => pair.Value is CharacterSprite))
            {
                var o = pair.Value as CharacterSprite;
                o.SetAnimations(_spriteTypeToId[pair.Key]);
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
                sprite.Draw?.Invoke(sprite, _spriteBatch, GetPlayerShift());
            }
            _spriteBatch.End();
        }

        private int GetPlayerShift()
        {
            var shiftOfPlayer = (int)_playerPosition.X - (int)_sprites[_playerId].Position.X;
            if (_sprites[_playerId].Position.X < _playerPosition.X)
                shiftOfPlayer = 0;
            if (_sprites[_playerId].Position.X > LevelWidth - _playerPosition.X)
                shiftOfPlayer = Graphics.PreferredBackBufferWidth - LevelWidth;
            return shiftOfPlayer;
        }

        public void LoadParameters(Dictionary<int, IGameObject> gameObjects, int playerId)
        {
            _playerId = playerId;

            foreach (var o in gameObjects.Where(o => !_sprites.ContainsKey(o.Key)))
            {
                switch (o.Value.SpriteId)
                {
                    case >= 1 and <= 11:
                        _spriteFactory = new CharacterSpriteFactory(Game.Content);
                        break;
                    case >= 12:
                        _spriteFactory = new StaticSpriteFactory(Game.Content);
                        break;
                }
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
                    chrSpr.State = chr.State;
                    chrSpr.AttackNumber = chr.AttackNumber;
                }
            }
        }

        //private void DrawRepeatableTexture(object sender, SpriteBatch sb)
        //{
        //    var sprite = sender as Sprite;
        //    var effects = SpriteEffects.None;

        //    var shiftOnPlayer = (int)_playerPosition.X - (int)_sprites[_playerId].Position.X;
        //    var textureCols = sprite.Size.Width / sprite.Texture.Width;
        //    var textureRows = sprite.Size.Height / sprite.Texture.Height;
        //    for (var i = 0; i < textureCols; i++)
        //        for (var j = 0; j < textureRows; j++)
        //            sb.Draw(sprite.Texture,
        //                new Rectangle(
        //                    shiftOnPlayer + (int)sprite.Position.X + i * sprite.Texture.Width,
        //                    (int)sprite.Position.Y + j * sprite.Texture.Height,
        //                    sprite.Texture.Width, sprite.Texture.Height),
        //                sprite.Texture.Bounds, Color.White,
        //                0, Vector2.Zero, effects ^= SpriteEffects.FlipHorizontally, 1);

        //    if (sprite.Size.Height % sprite.Texture.Height > 0)
        //        for (var i = 0; i < textureCols; i++)
        //            sb.Draw(sprite.Texture,
        //                new Rectangle(shiftOnPlayer + (int)sprite.Position.X + i * sprite.Texture.Width, (int)sprite.Position.Y + textureRows * sprite.Texture.Height,
        //                    sprite.Texture.Width, sprite.Size.Height - textureRows * sprite.Texture.Height),
        //                sprite.Texture.Bounds, Color.White,
        //                0, Vector2.Zero, effects ^= SpriteEffects.FlipHorizontally, 1);

        //    if (sprite.Size.Width % sprite.Texture.Width > 0)
        //        for (var i = 0; i < textureRows; i++)
        //            sb.Draw(sprite.Texture,
        //                new Rectangle(shiftOnPlayer + (int)sprite.Position.X + textureCols * sprite.Texture.Width, (int)sprite.Position.Y + i * sprite.Texture.Height,
        //                    sprite.Size.Width - textureCols * sprite.Texture.Width, sprite.Texture.Height),
        //                sprite.Texture.Bounds, Color.White,
        //                0, Vector2.Zero, effects ^= SpriteEffects.FlipHorizontally, 1);
        //    if (sprite.Size.Height % sprite.Texture.Height > 0 && sprite.Size.Width % sprite.Texture.Width > 0)
        //        sb.Draw(sprite.Texture,
        //        new Rectangle(shiftOnPlayer + (int)sprite.Position.X + textureCols * sprite.Texture.Width, (int)sprite.Position.Y + textureRows * sprite.Texture.Height,
        //            sprite.Size.Width - textureCols * sprite.Texture.Width, sprite.Size.Height - textureRows * sprite.Texture.Height),
        //        sprite.Texture.Bounds, Color.White,
        //        0, Vector2.Zero, effects ^= SpriteEffects.FlipHorizontally, 1);

        //    sprite.Position += new Vector2(sprite.Texture.Width, 0);
        //}

        public event EventHandler<AttackEventArgs> Attacked; 

        public event EventHandler<MoveEventArgs> Moved;

        public event EventHandler<CycleEventArgs> CycleFinished;
    }
}
