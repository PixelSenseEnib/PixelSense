using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Forms;
using Microsoft.Surface;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SurfaceControls = Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Input;

using Enib.SurfaceLib;




namespace SurfaceAppTest
{
    /// <summary>
    /// This is the main type for your application.
    /// </summary>
    public class App1 : Microsoft.Xna.Framework.Game
    {
       
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public TouchTarget touchTarget; // private
        private Microsoft.Xna.Framework.Color backgroundColor = new Microsoft.Xna.Framework.Color(0,81, 81, 81);
        private bool applicationLoadCompleteSignalled;

        private UserOrientation currentOrientation = UserOrientation.Bottom;
        private Matrix screenTransform = Matrix.Identity;

        private int screenWidth;
        private int screenHeight;

        SoundEffect _ballBounceWall;
        //private Texture2D bignou; // palet
        //private Texture2D raquetteGauche; //raquette1
        //private Texture2D raquetteDroite; //raquette2
        
        //private Vector2 bignou.Position;
        //private Vector2 bignou.Direction;

        private KeyboardState _keyboardState;
        //private float raqetteSpeed;

        private Rectangle hitboxRaquetteGauche;
        private Rectangle hitboxRaquetteDroite;
        private Rectangle hitboxBignou;

        private SpriteFont _font;

        private int scoreA = 0;
        private int scoreB = 0;

        private Manager manager;

        private Sprite batLeft;
        private Sprite batRight;
        private Sprite bignou;

        private CollisionEngine engine = new CollisionEngine();

        /// <summary>
        /// The target receiving all surface input for the application.
        /// </summary>
        protected TouchTarget TouchTarget
        {
            get { return touchTarget; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public App1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        #region Initialization

        /// <summary>
        /// Moves and sizes the window to cover the input surface.
        /// </summary>
        private void SetWindowOnSurface()
        {
            System.Diagnostics.Debug.Assert(Window != null && Window.Handle != IntPtr.Zero,
                "Window initialization must be complete before SetWindowOnSurface is called");
            if (Window == null || Window.Handle == IntPtr.Zero)
                return;

            // Get the window sized right.
            Program.InitializeWindow(Window);
            // Set the graphics device buffers.
            graphics.PreferredBackBufferWidth = Program.WindowSize.Width;
            graphics.PreferredBackBufferHeight = Program.WindowSize.Height;
            graphics.ApplyChanges();
            // Make sure the window is in the right location.
            Program.PositionWindow();
        }

        /// <summary>
        /// Initializes the surface input system. This should be called after any window
        /// initialization is done, and should only be called once.
        /// </summary>
        private void InitializeSurfaceInput()
        {
            System.Diagnostics.Debug.Assert(Window != null && Window.Handle != IntPtr.Zero, "Window initialization must be complete before InitializeSurfaceInput is called");
            if (Window == null || Window.Handle == IntPtr.Zero)
                return;
            System.Diagnostics.Debug.Assert(touchTarget == null, "Surface input already initialized");
            if (touchTarget != null)
                return;

            // Create a target for surface input.
            touchTarget = new TouchTarget(Window.Handle, EventThreadChoice.OnBackgroundThread);
            touchTarget.EnableInput();
        }

        #endregion

        #region Overridden Game Methods

        /// <summary>
        /// Allows the app to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            IsMouseVisible = true; // easier for debugging not to "lose" mouse
            SetWindowOnSurface();
            InitializeSurfaceInput();

            // Set the application's orientation based on the orientation at launch
            currentOrientation = ApplicationServices.InitialOrientation;

            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;

            // Setup the UI to transform if the UI is rotated.
            // Create a rotation matrix to orient the screen so it is viewed correctly
            // when the user orientation is 180 degress different.
            Matrix inverted = Matrix.CreateRotationZ(MathHelper.ToRadians(180)) *
                       Matrix.CreateTranslation(graphics.GraphicsDevice.Viewport.Width,
                                                 graphics.GraphicsDevice.Viewport.Height,
                                                 0);

            if (currentOrientation == UserOrientation.Top)
            {
                screenTransform = inverted;
            }

            screenWidth = Program.WindowSize.Width;
            screenHeight = Program.WindowSize.Height;

            manager = new Manager();
            manager.Behaviour = new Behaviour(screenWidth, screenHeight);
            manager.Initialize(this, touchTarget, Manager.SelectionMode.NONE);

            batLeft = new Sprite("raquette1", "raquette1");
            batLeft.Initialize(touchTarget);
            batLeft.Weight = 1;
            manager.register(batLeft);

            batRight = new Sprite("raquette2", "raquette2");
            batRight.Initialize(touchTarget);
            batRight.Weight = 1;
            batRight.Position = new Vector2(screenWidth - 140, screenHeight - 455);
            manager.register(batRight);

            bignou = new Sprite("bignou", "paletGame");
            bignou.Initialize(touchTarget);
            bignou.Position = new Vector2((screenWidth - 250) / 2, (screenHeight - 250) / 2);
            manager.register(bignou);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per app and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            manager.LoadContent(Content);

            _ballBounceWall = Content.Load<SoundEffect>("bahhhhh");

            _font = Content.Load<SpriteFont>("MaPolice");
        }


        /// <summary>
        /// UnloadContent will be called once per app and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        /// <summary>
        /// Allows the app to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            engine.update();
            manager.Update(gameTime);
            // Hitbox des raquettes
            hitboxRaquetteGauche = new Rectangle((int)batLeft.Position.X, (int)batLeft.Position.Y, batLeft.Size.Width, batLeft.Size.Height);
            hitboxRaquetteDroite = new Rectangle((int)batRight.Position.X, (int)batRight.Position.Y, batRight.Size.Width, batRight.Size.Height);
            hitboxBignou = new Rectangle((int)bignou.Position.X, (int)bignou.Position.Y, bignou.Size.Width, bignou.Size.Height);

            if (ApplicationServices.WindowAvailability != WindowAvailability.Unavailable)
            {
                /*// TODO: Add your update logic here
                bignou.Position = bignou.Position + bignou.Speed;
                
                // logique du bignou : collision
                if ((bignou.Speed.X < 0 && bignou.Position.X <= 0) || (bignou.Speed.X > 0 && bignou.Position.X + bignou.Size.Width >= screenWidth))
                {
                    //bignou.Direction.X = -bignou.Direction.X;
                    Console.WriteLine("GOALLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL!");
                    _ballBounceWall.Play();

                    if ((bignou.Speed.X < 0 && bignou.Position.X <= 0))
                    {
                        scoreB++;
                    }
                    else
                    {
                        scoreA++; 
                    }

                    bignou.Position = new Vector2((screenWidth - bignou.Size.Width) / 2, (screenHeight - bignou.Size.Height) / 2);
                    bignou.Speed *= 0;
                    //bignou.Direction *= 0;
                }
                else if ((bignou.Speed.Y < 0 && bignou.Position.Y <= 0) || (bignou.Speed.Y > 0 && bignou.Position.Y + bignou.Size.Height >= screenHeight))
                {
                    bignou.Speed = new Vector2(bignou.Speed.X, -bignou.Speed.Y);
                    _ballBounceWall.Play();
                }


                /************************************************************************************************************************************************
                *                                                       Logic collision raquette Gauche
                ************************************************************************************************************************************************/

                /*float Y = Math.Min(Math.Min(Math.Max(bignou.Position.Y + bignou.Size.Height - batLeft.Position.Y, 0), Math.Max(batLeft.Position.Y + batLeft.Size.Height - bignou.Position.Y, 0)), bignou.Size.Height);
                float X = Math.Min(Math.Min(Math.Max(bignou.Position.X + bignou.Size.Width - batLeft.Position.X, 0), Math.Max(batLeft.Position.X + batLeft.Size.Width - bignou.Position.X, 0)), bignou.Size.Width);

                //Console.WriteLine(X + " - " + Y);

                if (X < Y)
                {
                    if (hitboxRaquetteGauche.Contains((int)bignou.Position.X, (int)bignou.Position.Y) ||
                        hitboxRaquetteGauche.Contains((int)bignou.Position.X, (int)bignou.Position.Y + bignou.Size.Height))
                    {
                        if (bignou.Direction.X <= 0)
                        {
                            bignou.Direction = new Vector2(Math.Max(-bignou.Direction.X, batLeft.Speed.X), batLeft.Speed.Y + batLeft.Speed.Y * (float)0.5);
                            _ballBounceWall.Play();
                        }
                        else if (batLeft.Direction.X > 0)
                        {
                            bignou.Position = new Vector2(bignou.Position.X + batLeft.Direction.X, bignou.Position.Y);
                        }
                    }
                    else if (   hitboxRaquetteGauche.Contains((int)bignou.Position.X + bignou.Size.Width, (int)bignou.Position.Y) ||
                                hitboxRaquetteGauche.Contains((int)bignou.Position.X + bignou.Size.Width, (int)bignou.Position.Y + bignou.Size.Height))
                    {
                        if (bignou.Direction.X >= 0)
                        {
                            bignou.Direction = new Vector2(Math.Min(-bignou.Direction.X, batLeft.Speed.X), bignou.Direction.Y + batLeft.Speed.Y * (float)0.5);
                            _ballBounceWall.Play();
                        }
                        else if (batLeft.Direction.X < 0)
                        {
                            bignou.Position.X += batLeft.Direction.X;
                        }
                    }
                }

                if (hitboxBignou.Contains((int)batLeft.Position.X, (int)batLeft.Position.Y) ||
                        hitboxBignou.Contains((int)batLeft.Position.X + batLeft.Size.Width, (int)batLeft.Position.Y))
                {
                    if (X > Y)
                    {
                        if (bignou.Direction.Y >= 0)
                        {
                            bignou.Direction.X += batLeft.Speed.X * (float)0.5;
                            bignou.Direction.Y = Math.Min(-bignou.Direction.Y, batLeft.Speed.Y);
                            _ballBounceWall.Play();
                        }
                        else if (batLeft.Direction.Y < 0)
                        {
                            bignou.Position.Y += batLeft.Direction.Y;
                        }
                    }
                }
                else if (hitboxBignou.Contains((int)batLeft.Position.X + batLeft.Size.Width, (int)batLeft.Position.Y + batLeft.Size.Height) ||
                            hitboxBignou.Contains((int)batLeft.Position.X, (int)batLeft.Position.Y + batLeft.Size.Height))
                {
                    if (X > Y)
                    {
                        if (bignou.Direction.Y <= 0)
                        {
                            bignou.Direction.X += batLeft.Speed.X * (float)0.5;
                            bignou.Direction.Y = Math.Max(-bignou.Direction.Y, batLeft.Speed.Y);
                            _ballBounceWall.Play();
                        }
                        else if (batLeft.Direction.Y > 0)
                        {
                            bignou.Position.Y += batLeft.Direction.Y;
                        }
                    }
                }
                

                /************************************************************************************************************************************************
                *                                                       Logic collision raquette droite
                ************************************************************************************************************************************************/

                /*float Y2 = Math.Min(Math.Min(Math.Max(bignou.Position.Y + bignou.Height - batRight.Position.Y, 0), Math.Max(batRight.Position.Y + batRight.Size.Height - bignou.Position.Y, 0)), bignou.Height);
                float X2 = Math.Min(Math.Min(Math.Max(bignou.Position.X + bignou.Width - batRight.Position.X, 0), Math.Max(batRight.Position.X + batRight.Size.Width - bignou.Position.X, 0)), bignou.Width);

                if (hitboxRaquetteDroite.Contains((int)bignou.Position.X + bignou.Width, (int)bignou.Position.Y) ||
                    hitboxRaquetteDroite.Contains((int)bignou.Position.X + bignou.Width, (int)bignou.Position.Y + bignou.Height))
                {
                    if (X2 < Y2)
                    {
                        if (bignou.Direction.X >= 0)
                        {
                            Console.WriteLine(batRight.Speed.X);
                            bignou.Direction.X = Math.Min(-bignou.Direction.X, batRight.Speed.X);
                            bignou.Direction.Y += batRight.Speed.Y * (float)0.5;
                            _ballBounceWall.Play();
                        }
                        else if (batRight.Direction.X < 0)
                        {
                            bignou.Position.X += batRight.Direction.X;// = raquetteDroitePosition.X - bignou.Width;
                        }
                    }
                }
                else if(hitboxRaquetteDroite.Contains((int)bignou.Position.X, (int)bignou.Position.Y) ||
                        hitboxRaquetteDroite.Contains((int)bignou.Position.X, (int)bignou.Position.Y + bignou.Height))
                {
                    if (X2 < Y2)
                    {
                        if (bignou.Direction.X <= 0)
                        {
                            bignou.Direction.X = Math.Max(-bignou.Direction.X, batRight.Speed.X);
                            bignou.Direction.Y += batRight.Speed.Y * (float)0.5;
                            _ballBounceWall.Play();
                        }
                        else if (batRight.Direction.X > 0)
                        {
                            bignou.Position.X += batRight.Direction.X;//  = raquetteDroitePosition.X + raquetteDroite.Width;
                        }
                    }
                }

                if (hitboxBignou.Contains((int)batRight.Position.X, (int)batRight.Position.Y) ||
                         hitboxBignou.Contains((int)batRight.Position.X + batRight.Size.Width, (int)batRight.Position.Y))
                {
                    if (X2 > Y2)
                    {
                        if (bignou.Direction.Y >= 0)
                        {
                            bignou.Direction.X += batRight.Speed.X * (float)0.5;
                            bignou.Direction.Y = Math.Min(-bignou.Direction.Y, batRight.Speed.Y);
                            _ballBounceWall.Play();
                        }
                        else if (batRight.Direction.Y < 0)
                        {
                            bignou.Position.Y += batRight.Direction.Y;//  = raquetteDroitePosition.Y - bignou.Height;
                        }
                    }
                }
                else if (hitboxBignou.Contains((int)batRight.Position.X + batRight.Size.Width, (int)batRight.Position.Y + batRight.Size.Height) ||
                        hitboxBignou.Contains((int)batRight.Position.X, (int)batRight.Position.Y + batRight.Size.Height))
                {
                    if (X2 > Y2)
                    {
                        if (bignou.Direction.Y <= 0)
                        {
                            bignou.Direction.X += batRight.Speed.X * (float)0.5;
                            bignou.Direction.Y = Math.Max(-bignou.Direction.Y, batRight.Speed.Y);
                            _ballBounceWall.Play();
                        }
                        else if (batRight.Position.Y > 0)
                        {
                            bignou.Position.Y += batRight.Position.Y;// raquetteDroitePosition.Y + raquetteDroite.Height;
                        }
                    }
                }

                /**
                 * Bignou slowing
                 */
                /*double speedReductionFactor = 0.997;
                bignou.Direction *= (float)speedReductionFactor;


                /***
                 * KeyBoard
                 */
                _keyboardState = Keyboard.GetState();
                if (_keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up)){
                    if (batRight.Position.Y >= 0)
                    {
                        Vector2 tmp = batRight.Position;
                        tmp.Y -= batRight.SpeedMax;
                        batRight.Position = tmp;
                    }
                    else
                    {
                        // DO NOTHING : POSITION LOCK !
                    }
                }
                else if (_keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                {
                    if (batRight.Position.Y <= screenHeight - batRight.Size.Height)
                    {
                        Vector2 tmp = batRight.Position;
                        tmp.Y += batRight.SpeedMax;
                        batRight.Position = tmp;
                    }
                    else
                    {
                        // DO NOTHING : POSITION LOCK !
                    }
                }

                if (_keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                {
                    if (batRight.Position.X < screenWidth - batRight.Size.Width)
                    {
                        Vector2 tmp = batRight.Position;
                        tmp.X += batRight.SpeedMax;
                        batRight.Position = tmp;
                    }
                    else{
                        // DO NOTHING : POSITION LOCK !
                    }
                }
                else if (_keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                {
                    if (batRight.Position.X > ((screenWidth / 2) + batRight.Size.Width) * 0.9)
                    {
                        Vector2 tmp = batRight.Position;
                        tmp.X -= batRight.SpeedMax;
                        batRight.Position = tmp;
                    }
                    else
                    {
                        // DO NOTHING : POSITION LOCK !
                    }
                }

                if (_keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Z))
                {
                    if (batLeft.Position.Y >= 0)
                    {
                        Vector2 tmp = batLeft.Position;
                        tmp.Y -= batLeft.SpeedMax;
                        batLeft.Position = tmp;
                    }
                    else
                    {
                        // DO NOTHING : POSITION LOCK !
                    }
                }
                else if (_keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
                {
                    if (batLeft.Position.Y <= screenHeight - batLeft.Size.Height)
                    {
                        Vector2 tmp = batLeft.Position;
                        tmp.Y += batLeft.SpeedMax;
                        batLeft.Position = tmp;
                    }
                    else
                    {
                        // DO NOTHING : POSITION LOCK !
                    }
                }

                if (_keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                {
                    if (batLeft.Position.X < ((screenWidth / 2) - batLeft.Size.Width) * 0.9)
                    {
                        Vector2 tmp = batLeft.Position;
                        tmp.X += batLeft.SpeedMax;
                        batLeft.Position = tmp;
                    }
                    else
                    {
                        // DO NOTHING : POSITION LOCK !
                    }
                }
                else if (_keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Q))
                {
                    if (batLeft.Position.X > 0)
                    {
                        Vector2 tmp = batLeft.Position;
                        tmp.X -= batLeft.SpeedMax;
                        batLeft.Position = tmp;
                    }
                    else
                    {
                        // DO NOTHING : POSITION LOCK !
                    }

                }
                
            }

            //this.batLeft.HandleInput(touchTarget);
            base.Update(gameTime);
        }




        /// <summary>
        /// This is called when the app should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (!applicationLoadCompleteSignalled)
            {
                // Dismiss the loading screen now that we are starting to draw
                ApplicationServices.SignalApplicationLoadComplete();
                applicationLoadCompleteSignalled = true;
            }

            //TODO: Rotate the UI based on the value of screenTransform here if desired

            GraphicsDevice.Clear(backgroundColor);

            //TODO: Add your drawing code here

            spriteBatch.Begin();
            //spriteBatch.Draw(raquetteGauche, raquetteGauchePosition, Microsoft.Xna.Framework.Color.White);
            /*batLeft.Draw(spriteBatch, gameTime);
            batRight.Draw(spriteBatch, gameTime);
            bignou.Draw(spriteBatch, gameTime);*/
            manager.Draw(spriteBatch, gameTime);
            //spriteBatch.Draw(raquetteDroite, raquetteDroitePosition, Microsoft.Xna.Framework.Color.White);
            //spriteBatch.Draw(bignou, bignou.Position, Microsoft.Xna.Framework.Color.White);
            //spriteBatch.Draw(raquetteGauche,hitboxRaquetteGauche,Microsoft.Xna.Framework.Color.Yellow);
            //spriteBatch.Draw(batRight.Texture,hitboxRaquetteDroite,Microsoft.Xna.Framework.Color.Orange);
            //spriteBatch.Draw(bignou, hitboxBignou, Microsoft.Xna.Framework.Color.Red);

            Vector2 textSize = _font.MeasureString(scoreA + " - " + scoreB );
            spriteBatch.DrawString(_font, scoreA + " - " + scoreB, new Vector2((screenWidth - textSize.X) / 2, 20), Microsoft.Xna.Framework.Color.White);
            spriteBatch.End();

            //TODO: Avoid any expensive logic if application is neither active nor previewed

            base.Draw(gameTime);
        }

        #endregion

        #region Application Event Handlers

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: Enable audio, animations here

            //TODO: Optionally enable raw image here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: Optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: Disable audio, animations here

            //TODO: Disable raw image if it's enabled
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Release managed resources.
                IDisposable graphicsDispose = graphics as IDisposable;
                if (graphicsDispose != null)
                {
                    graphicsDispose.Dispose();
                }
                if (touchTarget != null)
                {
                    touchTarget.Dispose();
                    touchTarget = null;
                }
            }

            // Release unmanaged Resources.

            // Set large objects to null to facilitate garbage collection.

            base.Dispose(disposing);
        }

        #endregion
    }
}
