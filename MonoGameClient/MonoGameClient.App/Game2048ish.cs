using FontStashSharp;
using Game2048.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameClient.App.ContentLoading;
using Myra;
using Myra.Graphics2D.UI;
using System.IO;

namespace MonoGameClient.App
{
    public class Game2048ish : Game
    {
        static readonly Game2048ish instance = new();
        public static Game2048ish Instance => instance;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Desktop _desktop;
        private IGameManager _gameManager;
        public IGameManager BackendIntermediator => _gameManager;

        private Game2048ish()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _gameManager = new GameManager();
        }

        protected override void Initialize()
        {
            MyraContent.Load();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            MyraEnvironment.Game = this;

            VerticalStackPanel mainMenu = MainMenu.Load();

            _desktop = new();
            _desktop.Root = mainMenu;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _desktop.Render();

            base.Draw(gameTime);
        }
    }
}
