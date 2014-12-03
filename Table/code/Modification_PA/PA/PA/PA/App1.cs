using System;
using System.Collections.Generic;
using System.Linq;
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

using Enib.SurfaceLib;
using enib.pa;

namespace PA
{
    /// <summary>
    /// This is the main type for your application.
    /// </summary>
    public class App1 : Microsoft.Xna.Framework.Game
    {

        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private TouchTarget touchTarget;
        private Color backgroundColor = new Color(81, 81, 81);
        private bool applicationLoadCompleteSignalled;

        private UserOrientation currentOrientation = UserOrientation.Bottom;
        private Matrix screenTransform = Matrix.Identity;

        private int screenWidth;
        private int screenHeight;

        private Manager manager;
        private Sprite aircraftCarrier;
        private enib.pa.Plane plane1;
        private enib.pa.Plane plane2;
        private enib.pa.Plane plane3;
        private enib.pa.Plane plane4;

        private Plane_Menu_Elevator menu_pt1;
        private Plane_Menu_Catapulte1 menu_pt2;
        private Plane_Menu_Catapulte2 menu_pt3;
        private Enib.SurfaceLib.Menu menu;

        private string _serverIP;

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
        public App1(string serverIP)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _serverIP = serverIP;
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
            manager.Initialize(this, touchTarget, Manager.SelectionMode.MONO);
            manager.Behaviour = new BehaviourPlane(screenWidth, screenHeight, _serverIP);

            aircraftCarrier = new Sprite("aircraftCarrier", "aircraftCarrier");
            aircraftCarrier.Initialize(touchTarget);
            aircraftCarrier.Touchable = false;
            manager.Register(aircraftCarrier);

            plane1 = new enib.pa.Plane("plane1");
            plane1.Initialize(touchTarget);
            plane1.Scale = 0.13f;
            plane1.Weight = 1;
            plane1.Rotation = -(float)0;
            plane1.ShowBoundingRect = true;
            plane1.Dragable = false;
            manager.Register(plane1);

            plane2 = new enib.pa.Plane("plane2");
            plane2.Initialize(touchTarget);
            plane2.Scale = 0.13f;
            plane2.Weight = 1;
            plane2.Rotation = (float)Math.PI /2;
            plane2.ShowBoundingRect = true;
            plane2.Dragable = false;
            manager.Register(plane2);

            plane3 = new enib.pa.Plane("plane3");
            plane3.Initialize(touchTarget);
            plane3.Scale = 0.13f;
            plane3.Weight = 1;
            plane3.Rotation = (float)Math.PI;
            plane3.ShowBoundingRect = true;
            plane3.Dragable = false;
            manager.Register(plane3);

            plane4 = new enib.pa.Plane("plane4");
            plane4.Initialize(touchTarget);
            plane4.Scale = 0.13f;
            plane4.Weight = 1;
            plane4.Rotation = -(float)Math.PI / 2;
            plane4.ShowBoundingRect = true;
            plane4.Dragable = false;
            manager.Register(plane4);

            menu_pt1 = new Plane_Menu_Elevator("ascenseur1");
            menu_pt1.setPlaneur(plane1);

            menu_pt2 = new Plane_Menu_Catapulte1("catapulte1");
            menu_pt2.setPlaneur(plane1);

            menu_pt3 = new Plane_Menu_Catapulte2("catapulte2");
            menu_pt3.setPlaneur(plane1);

            menu = new Enib.SurfaceLib.Menu(manager, plane1);
            menu.Initialize(touchTarget);
            menu.addMenuEntry(menu_pt1);
            menu.addMenuEntry(menu_pt2);
            menu.addMenuEntry(menu_pt3);
            menu.Hide();

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

            Rectangle tmp = aircraftCarrier.Size;
            float scale = Math.Max(screenWidth / (float)tmp.Width, screenHeight / (float)tmp.Height);
            aircraftCarrier.Scale = scale;
            aircraftCarrier.Position = new Vector2(screenWidth / 2, screenHeight / 2);

            plane1.Position = new Vector2(100 * scale, 550 * scale);
            plane2.Position = new Vector2(300 * scale, 550 * scale);
            plane3.Position = new Vector2(500 * scale, 550 * scale);
            plane4.Position = new Vector2(700 * scale, 550 * scale);

            // TODO: use this.Content to load your application content here
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
            manager.Update(gameTime);
            if (ApplicationServices.WindowAvailability != WindowAvailability.Unavailable)
            {
                if (ApplicationServices.WindowAvailability == WindowAvailability.Interactive)
                {
                    // TODO: Process touches, 
                    // use the following code to get the state of all current touch points.
                    // ReadOnlyTouchPointCollection touches = touchTarget.GetState();
                }

                // TODO: Add your update logic here
            }


            /***
             * KeyBoard
             */
            KeyboardState key = Keyboard.GetState();
            if (key.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                plane1.Rotation = plane1.Rotation + 0.1f;
            }
            if (key.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                plane1.Rotation = plane1.Rotation - 0.1f;
            }

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

            spriteBatch.Begin();
            manager.Draw(spriteBatch, gameTime);
            spriteBatch.End();

            //TODO: Add your drawing code here
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
