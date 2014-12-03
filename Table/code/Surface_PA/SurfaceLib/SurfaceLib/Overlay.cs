using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Microsoft.Surface.Core;

namespace Enib
{
    namespace SurfaceLib
    {
        public abstract class Overlay
        {
            protected Texture2D _dummyTexture;
            protected Rectangle _dummyRectangle;
            protected Color _colori;
            protected Game _game;

            /// <summary>
            /// Setter and getter of the bounding rect 
            /// </summary>
            public virtual Rectangle Rectangle
            {
                get { return _dummyRectangle; }
                set { _dummyRectangle = value; }
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="rect">Bounding Rect</param>
            /// <param name="colori">Overlay color</param>
            /// <param name="game">Game</param>
            public Overlay(Rectangle rect, Color colori, Game game)
            {
                _dummyRectangle = rect;
                _colori = colori;
                _game = game;
            }

            /// <summary>
            /// Load texture
            /// </summary>
            public abstract void LoadContent();

            /// <summary>
            /// Draw overlay
            /// </summary>
            public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);

            /// <summary>
            /// Compute selection
            /// </summary>
            /// <param name="objects">List of all sprites contained in the world</param>
            /// <param name="colori">List of selected sprite</param>
            public abstract void GetSelection(LinkedList<Sprite> objects, LinkedList<Sprite> ioSelection);
        }
    }
}
