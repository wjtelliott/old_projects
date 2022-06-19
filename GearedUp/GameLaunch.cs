// 6/18/2022: This has wayy too many comments!?



using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Threading;

using GearedUpEngine.Assets.Util;
using GearedUpEngine.Assets.Entities;
using GearedUpEngine.Assets.Network.GearedUp;
using Lidgren.Network;
using System.Xml.Serialization;
using System.IO;
using GearedUpEngine.Assets.Map;
using GearedUpEngine.Assets.Menus;
using GearedUpEngine.Assets.UI;
using System;

/// <summary>
/// Namespace GearedUpEngine. Last modified on: 4/5/2017 by: William E
/// Modify as needed.
/// </summary>
namespace GearedUpEngine
{

    /// <summary>
    /// Our state the game is in. This is used to determine what
    /// to draw and update during each frame.
    /// </summary>
    public enum GameState
    {
        Splash,                     //Splash screen
        Loading,                    //Loading Textures before main menu
        PlayingMultiplayer,         //Playing in a multiplayer server (or attempting to connect)
        Menu_Main,                  //on the main menu
        HostingDedicated,           // Hosting a dedicated server (will stay on main menu until player has exited)
    }

    /// <summary>
    /// This class is used to load our content pipeline into our texture manager
    /// for later use in the game's main thread.
    /// </summary>
    class ContentLoadThread
    {
        /// <summary>
        /// Call this void to load our textures and tileset into the texture manager.
        /// The texture manager is referenced instead of passed, to make sure that
        /// we edit it for future use in the game's main thread.
        /// </summary>
        /// <param name="txm">The game's texture manager</param>
        /// <param name="content">The content pipeline</param>
        /// <param name="gfx">The graphics device data</param>
        public void ContentLoadThreadCall(ref TextureManager txm, ContentManager content, GraphicsDevice gfx)
        {
            /*
             * Load our texture manager's init.
             * This process will take longer,
             * depending on the amount of textures
             * that it has to process.
             */
            txm = new TextureManager(content, gfx);

            
        }
    }

    /// <summary>
    /// This class is used to start a Listen server
    /// on a seperate thread than our game's main
    /// thread. (the client)
    /// </summary>
    class ServerThread
    {
        /// <summary>
        /// Call this function to start a listen server
        /// </summary>
        /// <param name="server">Game server proxy</param>
        public void ServerRun(ref GearedServer server)
        {
            /*
             * Start our server.
             * All server settings will be adjusted and referenced
             * from the game's main thread.
             */
            server.StartServer();
        }
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameLaunch : Game
    {
        /// <summary>
        /// This is our application data used
        /// to identify our client/server recognition.
        /// 
        /// If our client tries to connect to a server,
        /// and their ApplicationId's are mismatched,
        /// the server will reject the client.
        /// </summary>
        const string ApplicationId = "endure_dev1";

        /// <summary>
        /// Our game's port to use. You may need
        /// to port forward this in your router
        /// settings.
        /// </summary>
        const int ApplicationPort = 14242;

        /// <summary>
        /// Graphics device data provided by MonoGame/Xna studios 
        /// </summary>
        GraphicsDeviceManager graphics;

        /// <summary>
        /// SpriteBatch to draw our textures onto our '2D' plane.
        /// Provided by MonoGame/Xna studios
        /// </summary>
        SpriteBatch spriteBatch;

        /// <summary>
        /// Texture manager used to store and distribute textures
        /// for each object and entity in the game.
        /// 
        /// This will also take tileset's and break them into frames
        /// or tiles for use. They can be called by name or index from
        /// the tile manager.
        /// </summary>
        TextureManager textureManager;

        /// <summary>
        /// This is used to manage the splash screens before the main
        /// menu is drawn. The texture manager requires some time to
        /// format and load all tiles and frames, so we will show
        /// this during that time.
        /// </summary>
        SplashManager splashes;

        /// <summary>
        /// This is our main menu manager. This is the manager used
        /// to handle all user input for the main menu before getting
        /// into single or multiplayer gameplay.
        /// </summary>
        MenuManager menus;

        /// <summary>
        /// This is our game state variable for use to determine where
        /// we are in the given context of the game's logic/frame.
        /// 
        /// Are we on the main menu?
        /// Are we loading?
        /// Are we displaying the splash screens?
        /// Are we in a multiplayer server?
        /// 
        /// And then update and render the scene accordingly.
        /// </summary>
        GameState gameState;

        /// <summary>
        /// This is our listen server for a client to host onto
        /// if they want to play while hosting a dedicated server.
        /// We initialize and run this right away for debug purposes.
        /// </summary>
        GearedServer listenServer;

        /// <summary>
        /// We use this to display text from the dedicated server on the main menu.
        /// </summary>
        DedicatedServerDisplay dedicatedServerDisplay;

        /// <summary>
        /// This is our game client we use to listen to the server's
        /// sent data and then change our game's variables and entities
        /// accordingly. We will also use this to create and send our
        /// commands to the server for other users.
        /// </summary>
        GearedClientProxy client;

        /// <summary>
        /// This is the font version of the texture manager. We will distribute
        /// and store our fonts here.
        /// </summary>
        FontManager fontManager;

        /// <summary>
        /// Here's our thread proxys we use to run our loading and
        /// server threads seperate from our game (and thereof client) thread.
        /// </summary>
        Thread loadThread;
        Thread listenServerThread;

        /// <summary>
        /// This is our client network helper to shorthand and offload some
        /// network util.
        /// </summary>
        ClientNetworking clientNetworkHelper;

        /// <summary>
        /// This is our game's chat menu. We will update and display this while
        /// we are connected to a multiplayer server. The user can utilize this
        /// to message other players on the server, and also use server commands
        /// to RCON the server and server variables.
        /// </summary>
        GameChat gameChat;

        StaticEntity staticTest;
        Tilemap tmp;
        ClientPlayer[] cps;
        Inventory clientInventory;
        Camera2D clientCamera;

        /// <summary>
        /// This variable is our camera offset for our main menu scroller.
        /// </summary>
        float mainMenuCameraOffset = 0;
        /// <summary>
        /// This variable is used to keep track of our time waiting for a server
        /// response to connect to a server. After it reaches a certain limit, we
        /// stop attempting to connect and boot to main menu.
        /// </summary>
        float timeoutCounter = 0;

        /// <summary>
        /// Our client's UID from the server
        /// </summary>
        public long ClientUID { get; private set; }

        /// <summary>
        /// Our Game's constructor. We initialize our graphics device info and
        /// setup our content pipeline within here.
        /// </summary>
        public GameLaunch()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //TODO: Setup a config manager to load user config's from/to .ini file

            //for now, default settings...

            //Setup our game's default graphic settings
            graphics.IsFullScreen = false;
            this.IsMouseVisible = true;
            //Apply our settings
            graphics.ApplyChanges();

            //Initialize our gamestate to splash screen (where the game initially starts)
            this.gameState = GameState.Splash;

            //Initialize our client
            this.client = new GearedClientProxy(ApplicationId);

            //Client network shorthand
            this.clientNetworkHelper = new ClientNetworking();

            //Some debug variables TODO: Remove if not in debug mode
            this.staticTest = new Assets.Entities.StaticEntity();
            this.tmp = new Tilemap();
            this.cps = new ClientPlayer[0];
            this.clientCamera = new Camera2D(GraphicsDevice.Viewport);
            this.dedicatedServerDisplay = new DedicatedServerDisplay();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load our splash manager
            LoadSplashManager();

            // Load our menu manager
            LoadMenus();

            // Load our font manager
            LoadFontManager();

            // Initialize our game chat with font
            this.gameChat = new GameChat(this.fontManager.GetFontByName("console"));

            // Load our textures using a new thread
            LoadTextures();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload and abort the other threads.
            // If we leave any threads still in memory or continue running,
            // the application will fail to exit.
            this.loadThread.Abort();

            // Unload our listen server if it is being used
            if (this.listenServer != null)
                this.listenServerThread.Abort();


            //TODO: Save singleplayer game if possible

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            // Get some user input for later use
            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            // Check which game state we are currently in, and update game logic accordingly.
            switch (gameState)
            {
                // Update our splash screen manager
                case GameState.Splash:

                    UpdateSplashScreen(kState);
                    
                    break;

                // Do nothing until our texture manager finishes initializing from our
                // other thread. After it is finished, abort the other thread.
                case GameState.Loading:

                    if (this.textureManager != null)
                        BreakFromLoading();
                    break;

                // Update our main menu manager.
                case GameState.Menu_Main:

                    ResetAllGameVariables();

                    UpdateMainMenu(mState, kState);

                    break;

                // Update our server (seperate thread)
                // Draw our text from it
                case GameState.HostingDedicated:

                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        this.gameState = GameState.Menu_Main;
                        this.menus.CurrentMenu = CurrentMenu.Main;
                        this.listenServerThread.Abort();
                        this.listenServer = null;
                    }

                    if (DeveloperDetails.Details != "")
                    {
                        this.dedicatedServerDisplay.Append(DeveloperDetails.Details);
                        DeveloperDetails.Details = "";
                    }

                    break;
                
                // Update our game logic, check for messages from
                // the server, and update our game's assets accordingly
                case GameState.PlayingMultiplayer:


                    // Check if we are trying to connect, if we are and our
                    // timeout has been reached. Disconnect and go back to
                    // the main menu
                    CheckTimeout();

                    UpdateGameChat(kState);

                    // Here we read our client's messages and then update the
                    // game's logic according to the server.
                    ReadClientMessages();

                    // Double check all tilemap textures and
                    // make sure we have everything serialized correctly.
                    FixTilemapErrors();

                    // beta inventory
                    this.clientInventory.Update(kState, mState);

                    // Request our client ID from the server (get our player that we currently are for use)
                    RequestClientUID();

                    // Update our camera with our uid
                    UpdateClientCamera();

                    // Update our movement with our uid
                    UpdateClientMovementKeys(kState);

                    // Update our player objects, TODO: Move to a sep method for updating all game objects
                    foreach (ClientPlayer cp in this.cps)
                        if (cp.UniqueUserID == this.ClientUID)
                            cp.UpdateSelf(mState, clientCamera);
                        else cp.Update();

                    break;

            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Clear our screen to show only black (our background before we draw our textures on it)
            GraphicsDevice.Clear(Color.Black);

            // Check our game state to see how we need to draw this scene
            switch (this.gameState)
            {
                // We are still on the splash screen,
                // allow the splash manager to draw the scene.
                case GameState.Splash:

                    DrawSplashScreen(spriteBatch);

                    break;

                // We are still waiting for the textures to load into memory
                // in our texture manager. Draw a generic loading screen until finished
                // (really shouldn't be more than a few milliseconds)
                case GameState.Loading:

                    DrawGenericLoading(spriteBatch);

                    break;
                
                // We are on the main menu. Allow the menu manager to draw this scene.
                case GameState.Menu_Main:

                    DrawMainMenus(spriteBatch);

                    break;

                case GameState.HostingDedicated:

                    spriteBatch.Begin();

                    this.dedicatedServerDisplay.Draw(spriteBatch, fontManager);

                    spriteBatch.End();

                    break;

                // We are in a multiplayer game. (TODO organize)
                case GameState.PlayingMultiplayer:
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, clientCamera.Transform);
                    this.staticTest.Draw(spriteBatch);
                    tmp.Draw(spriteBatch);

                    foreach (ClientPlayer cp in this.cps)
                    {
                        cp.Texture = this.textureManager.PlayerAnimations[1];//.GetTextureByName("player_debug");
                        cp.Draw(spriteBatch);
                    }
                    spriteBatch.End();
                    spriteBatch.Begin();
                    this.gameChat.Draw(spriteBatch);
                    this.clientInventory.Draw(spriteBatch, this.fontManager);
                    spriteBatch.End();
                    break;
            }

            

            base.Draw(gameTime);
        }


        /// <summary>
        /// Only call this after the user has been kicked or disconnected to the main menu.
        /// This will reset the game variables for the next game session.
        /// </summary>
        void ResetAllGameVariables()
        {
            this.cps = new ClientPlayer[0];
            this.tmp = new Tilemap();
            this.staticTest = new StaticEntity();
        }

        /// <summary>
        /// Update what we send to the server for the 
        /// </summary>
        /// <param name="kState"></param>
        void UpdateClientMovementKeys(KeyboardState kState)
        {

            // Update our keyboard key commands for movement to the server
            this.clientNetworkHelper.SendCharacterMovement(this.client.Client, kState);
        }

        /// <summary>
        /// We check to see if our client camera is nulled, but the player character is initialized.
        /// If this is true, we create a new camera, otherwise just update the one we have.
        /// </summary>
        void UpdateClientCamera()
        {
            
            // Is our camera initialized & our client is initialized?
            if (this.clientCamera != null && this.cps.Length > 0)

                // Update our camera with our client object
                this.clientCamera.Update(this.cps[0].GetCameraAngle());

            // Create our camera object with our viewport.
            else this.clientCamera = new Camera2D(GraphicsDevice.Viewport);
        }

        /// <summary>
        /// We check if our UID is nulled, and request it from the server if it is
        /// </summary>
        void RequestClientUID()
        {

            // Is our UID null or invalid?
            if (this.ClientUID == 0)

                // Request it from the server
                this.clientNetworkHelper.RequestClientUID(this.client.Client);
        }

        /// <summary>
        /// Load our splash manager and give it an array of textures for how
        /// many splash screens we have. It will show one after the other or
        /// until you skip them.
        /// </summary>
        void LoadSplashManager()
        {
            /*
             
                New splash manager():
                { textures 2d as an array loaded from content pipeline }
                { time to keep it on screen (in ms?). default 150 }
                { allow us to skip? default true }
             
             */
            this.splashes = new SplashManager(
                new Texture2D[]
                {
                    Content.Load<Texture2D>("Assets/UI/Other/splash_test")
                },
                150, true
                );
        }

        /// <summary>
        /// Load our menu manager. We need a button texture. For now we just use one...
        /// Our splash screen. WE NEED MORE TEXTURES.
        /// </summary>
        void LoadMenus()
        {
            this.menus = new MenuManager(Content.Load<Texture2D>("Assets/UI/Other/splash_test"));
        }

        /// <summary>
        /// Load our font manager. We use our content pipeline to grab our fonts.
        /// We also give names to our fonts so we can grab them later without having
        /// to need an array index.
        /// </summary>
        void LoadFontManager()
        {
            /*
             * New font manager()
             * { fonts as array }
             * { names as array }
             */
            this.fontManager = new FontManager(
                new SpriteFont[]
                {
                    Content.Load<SpriteFont>("Assets/Font/console"),
                    Content.Load<SpriteFont>("Assets/Font/menu"),
                    Content.Load<SpriteFont>("Assets/Font/splash")
                },
                new string[]
                {
                    "console",
                    "menu",
                    "splash"
                }
                );
        }

        /// <summary>
        /// We load our textures here in our seperate thread by initializing our texture manager.
        /// </summary>
        void LoadTextures()
        {
            if (this.loadThread == null)
            {
                this.loadThread = new Thread(() => new ContentLoadThread().ContentLoadThreadCall(ref this.textureManager, this.Content, GraphicsDevice));
                this.loadThread.Start();
            }
        }

        /// <summary>
        /// Draw our main menu system. This will update and scroll our game's background, as
        /// well as draw our main menu objects.
        /// </summary>
        /// <param name="spriteBatch">Game's spritebatch</param>
        void DrawMainMenus(SpriteBatch spriteBatch)
        {
            //Do not need linear wrap mode anymore.
            spriteBatch.Begin();

            // TODO: Move this to a different area instead of loading from file every frame?
            Texture2D t = Content.Load<Texture2D>("Assets/UI/Other/torn_city");
            
            // Draw 2 copies to immitate scrolling
            spriteBatch.Draw(t, new Rectangle(0 - (int)(mainMenuCameraOffset += 0.4f), 0, (int)(this.graphics.PreferredBackBufferWidth * 2.58), this.graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.Draw(t, new Rectangle(0 - (int)mainMenuCameraOffset + (int)(this.graphics.PreferredBackBufferWidth * 2.58), 0, (int)(this.graphics.PreferredBackBufferWidth * 2.58), this.graphics.PreferredBackBufferHeight), Color.White);

            // After we pass the first copy, move both copies back to starting position to immitate infinite scrolling.
            if (mainMenuCameraOffset >= (int)(this.graphics.PreferredBackBufferWidth * 2.58)) mainMenuCameraOffset = 0;

            

            // Draw our menu objects atop the background.
            this.menus.Draw(spriteBatch, this.fontManager.GetFontByName("menu"));
            spriteBatch.End();
        }

        /// <summary>
        /// Draw a generic string message. This shouldn't draw more than 1 - 3 frames.
        /// </summary>
        /// <param name="spriteBatch">Game's spritebatch</param>
        void DrawGenericLoading(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(this.fontManager.GetFontByName("splash"), "Loading Textures...", new Vector2((int)this.graphics.PreferredBackBufferWidth / 4, (int)this.graphics.PreferredBackBufferHeight / 4), Color.White);
            spriteBatch.End();
        }

        /// <summary>
        /// Draw our game's splash manager.
        /// </summary>
        /// <param name="spriteBatch">Game's spritebatch</param>
        void DrawSplashScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            this.splashes.Draw(spriteBatch);
            spriteBatch.End();
        }

        /// <summary>
        /// The game has finished loading textures for our texture manager
        /// from the other thread. Now we will continue onto our Main menu and
        /// abort our loading thread.
        /// </summary>
        void BreakFromLoading()
        {
            this.gameState = GameState.Menu_Main;
            this.clientInventory = new Inventory(textureManager);
            this.loadThread.Abort();
        }

        /// <summary>
        /// Check to see if we need to send chat messages to the server from us.
        /// Update our keys to the game chat to add text, etc.
        /// </summary>
        /// <param name="kState">Current Keyboard State</param>
        void UpdateGameChat(KeyboardState kState)
        {
            // Update our chat
            this.gameChat.Update(kState);

            // Are we ready to do our object action? (send a message to server)
            if (this.gameChat.DoAction)
            {
                // Check to see if our chat is a command
                if (this.gameChat.UserInput.StartsWith("."))
                {

                    // Preform command:
                    switch (gameChat.UserInput.Substring(1))
                    {
                        case "disconnect":
                            this.client.Client.Disconnect("UEXIT");
                            this.gameState = GameState.Menu_Main;
                            break;
                    }

                }
                else
                {

                    //Send Chat to server:
                    this.clientNetworkHelper.SendChat(this.client.Client, this.gameChat.UserInput);

                }

                // Reset our chat
                this.gameChat.UserInput = "";
                this.gameChat.DoAction = false;
            }
        }

        /// <summary>
        /// Check to see if we are not connected to a server and if we timeout
        /// from trying to connect.
        /// </summary>
        void CheckTimeout()
        {
            // Are we connected to a server? If we aren't, continue
            if (this.client.Client.ConnectionStatus == NetConnectionStatus.Disconnected || this.client.Client.ConnectionStatus == NetConnectionStatus.None)
            {
                int timeoutLimit = 500;

                // Advance our timeout counter, or reset it to zero if we reached our peak (500u)
                this.timeoutCounter = (this.timeoutCounter > timeoutLimit) ? 0 : this.timeoutCounter + 1f;

                // Check if we are at 500u for our timeout limit
                if (this.timeoutCounter >= timeoutLimit)
                {
                    // We timed out. Disconnect from the server we are
                    // attempting to connect to.
                    this.client.Client.Disconnect("");

                    // Put us back onto the main menu and reset our timeout counter for future use.
                    this.gameState = GameState.Menu_Main;
                    this.timeoutCounter = 0f;
                }
            }
        }

        /// <summary>
        /// Double check our tilemap for errors and then correct them.
        /// </summary>
        void FixTilemapErrors()
        {
            // Is our texture manager initialized?
            if (this.textureManager != null)
            {

                // Check each tile, if they have no texture, try reserializing their texture
                // with the texture manager.
                foreach (Tile t in this.tmp.Tiles)
                {
                    if (t.Texture == null)
                        t.Texture = this.textureManager.GetTextureByName(t.TextureSerialization);
                }
            }
        }

        /// <summary>
        /// Read our messages sent from the server to the client.
        /// Change our game logic accordingly.
        /// </summary>
        void ReadClientMessages()
        {
            // Initialize our message, use an int32 for our message type.
            NetIncomingMessage msg;
            int msgType;

            // Check all messages
            while ((msg = this.client.Client.ReadMessage()) != null)
            {

                // What kind of message do we have?
                switch (msg.MessageType)
                {

                    case NetIncomingMessageType.StatusChanged:
                        switch(this.client.Client.ConnectionStatus)
                        {
                            case NetConnectionStatus.Disconnected:
                                try
                                {
                                    gameChat.AppendChat(msg.ReadString().Substring(1));
                                } catch
                                {
                                    //idkytf its doing this
                                }
                                System.Threading.Thread.Sleep(500);
                                this.gameState = GameState.Menu_Main;
                                break;
                        }
                        break;

                    // We attempted to connect to a server. They are responding with OK.
                    // Connect.
                    case NetIncomingMessageType.DiscoveryResponse:
                        this.clientNetworkHelper.ConnectTo(this.client.Client, msg.SenderEndPoint);
                        break;
                    
                    // We are getting generic data from the server.
                    // Distribute our logic
                    case NetIncomingMessageType.Data:

                        // What kind of data are we receiving?
                        msgType = msg.ReadByte();

                        // Check our data type
                        switch (msgType)
                        {

                            // The server is sending us a complete object update (big load)
                            case (int)NetworkDataType.ObjectDownload:

                                // TODO: Update our tilemap objects later. For now just update our current test object
                                this.staticTest = (StaticEntity)this.clientNetworkHelper.DownloadObject(msg, this.textureManager);
                                break;

                            // The server is sending us a complete tilemap update (big load)
                            case (int)NetworkDataType.TileMapDownload:

                                // TODO: Update our list of tilemaps later. For now just update our current test object
                                this.tmp = (Tilemap)this.clientNetworkHelper.DownloadTileMap(msg, this.textureManager);
                                break;

                            // The server is sending us a chat message
                            case (int)NetworkDataType.GameChatMessage:

                                // Send the string straight to the game chat.
                                this.gameChat.AppendChat(msg.ReadString());
                                break;

                            // The server is sending us data that a new client has connected
                            case (int)NetworkDataType.NewClientPlayer:

                                // Download and add the player to the list of clients
                                this.AppendPlayerToArray(clientNetworkHelper.DownloadClientPlayer(msg));
                                break;

                            // Our client requested our UID and we got a response
                            case (int)NetworkDataType.ClientRequestUID:

                                this.ClientUID = (long)this.clientNetworkHelper.DownloadClientUID(msg);
                                break;

                            case (int)NetworkDataType.PlayerMovementUpdate:

                                this.clientNetworkHelper.DownloadPlayerMovement(msg, this.cps);

                                break;

                            case (byte)NetworkDataType.ClientAccountLogin:

                                this.clientNetworkHelper.SendPlayerLogin(this.client.Client, this.menus.GetUserInputString("usernameTextBox"), this.menus.GetUserInputString("passwordTextBox"));

                                break;
                        }
                        break;
                }
            }
        }

        void AppendPlayerToArray(ClientPlayer newPlayer)
        {
            Array.Resize<ClientPlayer>(ref this.cps, this.cps.Length + 1);

            newPlayer.IsPlayer = true;
            newPlayer.IsVisible = true;
            newPlayer.SetLocation(10 , 250);

            this.cps[this.cps.Length - 1] = newPlayer;
        }

        /// <summary>
        /// Update our splash screen. It changes timers, fades to black, and continues once finished to the main menu.
        /// </summary>
        void UpdateSplashScreen(KeyboardState kState)
        {
            // Update our manager.
            this.splashes.Update(kState);

            // If we are finished, go to the next step.
            if (this.splashes.IsFinished)
                this.gameState = GameState.Loading;
        }

        /// <summary>
        /// Update our main menu manager. This checks Mouse and Keyboard functions
        /// to update our menu objects accordingly. If the menu says to transition to a new
        /// game state, we update that here.
        /// </summary>
        void UpdateMainMenu(MouseState mState, KeyboardState kState)
        {

            // Update our manager.
            this.menus.Update(mState, kState);

            // Do we want to start the server?
            if (this.menus.StartListenServer && this.listenServer == null && !this.menus.StartDedicatedServer)
            {

                //Initialize our Listen server and our thread to hold it
                this.listenServer = new GearedServer(ApplicationPort, ApplicationId);
                this.listenServerThread = new Thread(() => new ServerThread().ServerRun(ref this.listenServer));


                //Start our server
                this.listenServerThread.Start();

                // Reset Menu
                this.menus.StartListenServer = false;
            }
            else if 
                (this.menus.StartDedicatedServer && !this.menus.StartListenServer && this.listenServer == null)
            {
                //Initialize our Listen server and our thread to hold it
                this.listenServer = new GearedServer(ApplicationPort, ApplicationId)
                {
                    IsDedicated = true
                };

                this.listenServerThread = new Thread(() => new ServerThread().ServerRun(ref this.listenServer));


                //Start our server
                this.listenServerThread.Start();

                // Reset Menu
                this.menus.StartDedicatedServer = false;

                this.gameState = GameState.HostingDedicated;

                // opening message
                DeveloperDetails.Details = "Launching Dedicated Server!";
            }

            // Are we attempting to leave the menu to a new game state?
            if (this.menus.CurrentMenu == CurrentMenu.Play)
            {
                // Change our game state to the new one
                this.gameState = GameState.PlayingMultiplayer;

                // Get our text box user input and try to connect to a server with that address.
                // TODO: Use a text box for the server port later too
                this.client.Client.DiscoverKnownPeer(this.menus.GetUserInputString("serverTextBox"), ApplicationPort);

                // Reset our main menu state
                this.menus.CurrentMenu = CurrentMenu.Main;

            }
        }
    }
}
