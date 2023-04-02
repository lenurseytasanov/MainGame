using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MainGame
{
    public class GameCycle : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Screen _currentScreen;
        private PhysicalModel _model;

        public GameCycle()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _currentScreen = new GamePlay() { Game = this };
            var gameplay = _currentScreen as GamePlay;
            gameplay.Initialize();

            _model = new PhysicalModel();
            _model.Initialize();

            gameplay.PlayerMoved += (sender, args) => _model.MovePlayer(args.Dir);
            gameplay.CycleFinished += (sender, args) => _model.Update();
            _model.Updated += (sender, args) => gameplay.LoadParameters(args.Objects);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _currentScreen.LoadContent(_spriteBatch);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _currentScreen.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _currentScreen.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}