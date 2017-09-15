using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.NuclexGui;

namespace WordCollector2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        //private SpriteBatch _spriteBatch;
        //private Sprite _sprite;
        //private Camera2D _camera;

        private readonly InputListenerComponent _inputManager;
        private readonly GuiManager _gui;
        private readonly GuiInputService _inputService;

        private Color _backgroundColor;

        private HubConnection _connection;
        private IHubProxy _proxy;

        private MenuScene _menuWindow;
        private GameScene _gameWindow;

        public Game1()
        {
            // сначала наследуемые
            this.Window.AllowUserResizing = true;
            this.Window.AllowAltF4 = true;
            this.Window.ClientSizeChanged += Window_ClientSizeChanged;
            this.IsMouseVisible = true;
            this.Content.RootDirectory = "Content";

            // объявленные нами
            this._graphicsDeviceManager = new GraphicsDeviceManager(this);
            this._backgroundColor = Color.CornflowerBlue;
            this._inputManager = new InputListenerComponent(this);

            // Then, we create GUI.
            this._inputService = new GuiInputService(_inputManager);
            this._gui = new GuiManager(this.Services, this._inputService);

            // Create a GUI screen and attach it as a default to GuiManager.
            // That screen will also act as a root parent for every other control that we create.
            this._gui.Screen = new GuiScreen(800, 480);
            this._gui.Screen.Desktop.Bounds = new UniRectangle(
                new UniScalar(0f, 0), 
                new UniScalar(0f, 0), 
                new UniScalar(1f, 0), 
                new UniScalar(1f, 0));

            /*BoxingViewportAdapter viewportAdapter = 
                new BoxingViewportAdapter(this.Window, this.GraphicsDevice, 800, 480);
            this._camera = new Camera2D(viewportAdapter);*/

            this._connection = new HubConnection("127.0.0.1");
            this._proxy = this._connection.CreateHubProxy("GlobalHub");
            //this._proxy.
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            this._gui.Screen.Height = this.Window.ClientBounds.Height;
            this._gui.Screen.Width = this.Window.ClientBounds.Width;
            /*this._graphicsDeviceManager.PreferredBackBufferHeight = this.Window.ClientBounds.Height;
            this._graphicsDeviceManager.PreferredBackBufferWidth = this.Window.ClientBounds.Width;
            this._graphicsDeviceManager.ApplyChanges();*/
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Perform second-stage initialization
            this._gui.Initialize();

            this._menuWindow = new MenuScene();
            this._menuWindow.BtnStart.Pressed += _menuWindow_BtnStart_Pressed;
            this._gui.Screen.Desktop.Children.Add(this._menuWindow);

            Task.Run(async () => await this._connection.Start());
        }

        void _menuWindow_BtnStart_Pressed(object sender, EventArgs e)
        {
            this._gui.Screen.Desktop.Children.Remove(this._gameWindow);
            this._gameWindow = new GameScene();
            this._inputService.KeyboardListener.KeyTyped += 
                (s, args) => this._gameWindow.OnKeyTyped(args);
            this._gui.Screen.Desktop.Children.Add(this._gameWindow);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);

            //TODO: use this.Content to load your game content here 

            /*var logoTexture = Content.Load<Texture2D>("logo-square-128");
            _sprite = new Sprite(logoTexture)
            {
                Position = viewportAdapter.Center.ToVector2()
            };*/
            //SpriteFont font = this.Content.Load<SpriteFont>("DefaultFont");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // For Mobile devices, this logic will close the Game when the Back button is pressed
            // Exit() is obsolete on iOS
            #if !__IOS__ &&  !__TVOS__
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            #endif

            if (this._gameWindow != null)
                this._gameWindow.BringToFront();
            else
                this._menuWindow.BringToFront();

            // TODO: Add your update logic here
            // Update both InputManager (which updates states of each device) and GUI
            this._inputManager.Update(gameTime);
            this._gui.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            
            //TODO: Add your drawing code here
            this.GraphicsDevice.Clear(this._backgroundColor);
            this._gui.Draw(gameTime);

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            //this.graphics.Dispose();
            //this.spriteBatch.Dispose();
            base.UnloadContent();
        }
    }
}

