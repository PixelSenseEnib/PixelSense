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
        public class RectangleOverlay: Overlay
        {
            public RectangleOverlay(Rectangle rect, Color colori, Game game): base(rect, colori, game)
            {
            }

            public override void LoadContent()
            {
                _dummyTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
                _dummyTexture.SetData(new Color[] { Color.White });
            }

            public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
            {
                spriteBatch.Draw(_dummyTexture, _dummyRectangle, _colori);
            }

            public override void GetSelection(LinkedList<Sprite> objects, LinkedList<Sprite> ioSelection)
            {
                foreach (Sprite obj in objects)
                {
                    if (_dummyRectangle.Intersects( obj.BoundingRect ) )
                        ioSelection.AddLast(obj);
                }
            }
        }
    }
}
