using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameClient.App.Assets;
using MonoGameClient.App.Utils;
using System;

namespace MonoGameClient.App
{
    public class Game2048ish : Game
    {
        static readonly Game2048ish instance = new();
        internal static Game2048ish Instance => instance;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Button button1;
        Button button2;
        Button button3;
        Button button4;

        Label label1;

        string selctedB = "None";

        private Game2048ish()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            ContentCentral.Load(Content);

            button1 = new("button1", ContentCentral.LongButton1, ContentCentral.ComicMono48, Color.Black, new Vector2(100, 100));
            button1.Click += OnClick;
            button2 = new("button2", ContentCentral.LongButton1, ContentCentral.ComicMono48, Color.Black, new Vector2(100, 250));
            button2.Click += OnClick;
            button3 = new("button3", ContentCentral.LongButton1, ContentCentral.ComicMono48, Color.Black, new Vector2(450, 100));
            button3.Click += OnClick;
            button4 = new("Exit", ContentCentral.LongButton1, ContentCentral.ComicMono48, Color.Black, new Vector2(450, 250));
            button4.Click += (sender, args) => { Exit(); };

            label1 = new("árvíztűrő tükörfúrógép", ContentCentral.LongLabel1, ContentCentral.FreeMono36, Color.Black, new Vector2(10, 375));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseUtils.Update();

            button1.Update(gameTime);
            button2.Update(gameTime);
            button3.Update(gameTime);
            button4.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(ContentCentral.ComicMono48, "Selected: " + selctedB, new Vector2(40, 40), Color.Yellow);
            button1.Draw(_spriteBatch, gameTime);
            button2.Draw(_spriteBatch, gameTime);
            button3.Draw(_spriteBatch, gameTime);
            button4.Draw(_spriteBatch, gameTime);
            
            label1.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

#nullable enable
        void OnClick(object? sender, EventArgs eventArgs)
        {
            if (sender is not null && sender is Button b)
            {
                selctedB = b.Text;
            }
        }
#nullable restore
    }
}
