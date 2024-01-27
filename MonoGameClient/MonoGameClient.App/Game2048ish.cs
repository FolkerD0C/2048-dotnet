using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameClient.App.Assets;
using MonoGameClient.App.Utils;

namespace MonoGameClient.App
{
    public class Game2048ish : Game
    {
        static readonly Game2048ish instance = new();
        internal static Game2048ish Instance => instance;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Button button;

        private Game2048ish()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            ContentCentral.Load(Content);

            button = new(ContentCentral.LongButton1, new(100, 100), 100, 600, "", ContentCentral.ComicMono48);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            MouseUtils.Update();

            button.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
    
            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            button.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
