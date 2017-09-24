using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.NuclexGui;
using Contracts;

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
        private IHubProxy<IServerContract, IClientContract> _proxy;

        private MenuScene _menuWindow;
        private GameScene _gameWindow;

        private string CurrentGameId = string.Empty;
        private string CurrentEnemyNick = string.Empty;

        public Game1()
        {
            // сначала наследуемые
            this.Window.AllowUserResizing = true;
            this.Window.AllowAltF4 = true;
            this.Window.ClientSizeChanged += this.Window_ClientSizeChanged;
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
            this._gui.Screen = new GuiScreen(320, 240);
            this._gui.Screen.Desktop.Bounds = new UniRectangle(
                new UniScalar(0f, 0), 
                new UniScalar(0f, 0), 
                new UniScalar(1f, 0), 
                new UniScalar(1f, 0));

            /*BoxingViewportAdapter viewportAdapter = 
                new BoxingViewportAdapter(this.Window, this.GraphicsDevice, 800, 480);
            this._camera = new Camera2D(viewportAdapter);*/

            this._connection = new HubConnection("http://localhost:8080/signalr");
            this._connection.DeadlockErrorTimeout = TimeSpan.FromMinutes(10);
            this._connection.TransportConnectTimeout = TimeSpan.FromMinutes(10);

            this._proxy = this._connection.CreateHubProxy("GlobalHub")
                .AsHubProxy<IServerContract, IClientContract>();
            this._proxy.SubscribeOn<string>(
                c => c.OnShowMessage, 
                s =>
                {
                    MessageScene msg = new MessageScene();
                    msg.SetText(s);
                    this._gui.Screen.Desktop.Children.Add(msg);
                    msg.BringToFront();
                });

            this._proxy.SubscribeOn<char>(
                c => c.OnCanDoStep,
                ch =>
                {
                    this._gameWindow.AddMessage("Противник добавил букву [" + ch + "]");
                    this._gameWindow.TbWord.Text += ch;
                    this._gameWindow.AddMessage("Ваш ход");
                });

            this._proxy.SubscribeOn<string, string, char>(
                c => c.OnGameStarted,
                (gameId, enemyNick, startChar) =>
                {
                    this.CurrentGameId = gameId;
                    this.CurrentEnemyNick = enemyNick;
                    this.ShowGameWindow(startChar);
                });
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
            this._menuWindow.BtnSaveNick.Pressed += (sender, e) => 
                this._proxy.Call(s => s.Connect(this._menuWindow.TbNickName.Text));
            this._gui.Screen.Desktop.Children.Add(this._menuWindow);
            this._gui.Screen.FocusChanged += this._menuWindow.OnFocusChanged;
            this._menuWindow.BringToFront();

            //MessageScene msg = new MessageScene("fadggsgfsdhfdhdfhjsgjsjksksgkgksfkgg4234235623664");
            //this._gui.Screen.Desktop.Children.Add(msg);
            //msg.BringToFront();
            Task.Run(async () => await this._connection.Start());
        }

        void ShowGameWindow(char startChar)
        {
            this._gameWindow?.Close();
            this._gui.Screen.Desktop.Children.Remove(this._gameWindow);

            this._gameWindow = new GameScene(this.CurrentGameId, startChar);
            this._gameWindow.AddMessage("Игра началась");
            this._gameWindow.AddMessage("Противник : " + this.CurrentEnemyNick);
            this._gameWindow.AddMessage("Случайная стартовая буква : [" + startChar + "]");
            this._inputService.KeyboardListener.KeyTyped += this._gameWindow.OnKeyTyped;
            this._gameWindow.BtnNextStep.Pressed += (control, args) =>
            {
                this._gameWindow.AddMessage("Вы передали ход противнику");
                this._proxy.Call(s => s.DoStep(
                        this.CurrentGameId, 
                        this._gameWindow.TbWord.Text[this._gameWindow.TbWord.Text.Length - 1]));
            };
            this._gui.Screen.Desktop.Children.Add(this._gameWindow);
            this._gameWindow.BringToFront();
        }

        void _menuWindow_BtnStart_Pressed(object sender, EventArgs e)
        {
            if (this._connection.State != ConnectionState.Connected)
            {
                MessageScene msg = new MessageScene();
                msg.SetText("Отсутствует подключение к серверу");
                this._gui.Screen.Desktop.Children.Add(msg);
                msg.BringToFront();
                return;
            }

            if (!this._menuWindow.IsValidNick())
            {
                MessageScene msg = new MessageScene();
                msg.SetText(
                    "Имя игрока должно состоять из латинских букв и цифр и иметь длину от 3 до 100 символов");
                this._gui.Screen.Desktop.Children.Add(msg);
                msg.BringToFront();
                return;
            }
            
            this._gui.Screen.Desktop.Children.Remove(this._gameWindow);

            Tuple<string, string, char> gameData = this._proxy.Call(s => s.CreateNewGame());

            if (string.IsNullOrWhiteSpace(gameData.Item1))
            {
                MessageScene msg;
                if (string.IsNullOrWhiteSpace(gameData.Item2))
                {
                    msg = new MessageScene();
                    msg.SetText("На сервере нет пользователей, чтобы с вами сыграть");
                }
                else
                {
                    msg = new MessageScene();
                    msg.SetText("Не удалось создать игру с игроком [" + gameData.Item2 + "]");
                }
                this._gui.Screen.Desktop.Children.Add(msg);
                msg.BringToFront();
                return;
            }

            if (string.IsNullOrWhiteSpace(gameData.Item2))
            {
                MessageScene msg = new MessageScene();
                msg.SetText("Не удалось получить имя противника в игре [" + gameData.Item1 + "]");
                this._gui.Screen.Desktop.Children.Add(msg);
                msg.BringToFront();
                return;
            }

            this.CurrentGameId = gameData.Item1;
            this.CurrentEnemyNick = gameData.Item2;
            this.ShowGameWindow(gameData.Item3);
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

            /*if (this._gameWindow != null)
                this._gameWindow.BringToFront();
            else
                this._menuWindow.BringToFront();*/

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

