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
        public class CircleOverlay : Overlay
        {
            bool _needRefresh = true;
            int _radius = 0;

            public override Rectangle Rectangle
            {
                get { return _dummyRectangle; }
                set { 
                    _dummyRectangle = value;
                    _needRefresh = true;
                }
            }

            public CircleOverlay(Rectangle rect, Color colori, Game game)
                : base(rect, colori, game)
            {
            }
                        
            public override void LoadContent()
            {
                _dummyTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
                _dummyTexture.SetData(new Color[] { Color.White });
            }

            public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
            {
                if (_needRefresh)
                {
                    _radius = (int)(Math.Sqrt(_dummyRectangle.Height * _dummyRectangle.Height + _dummyRectangle.Width * _dummyRectangle.Width) / 2);
                    int outerRadius = (int)(_radius * 2 + 2); // So circle doesn't go out of bounds
                    _dummyTexture = new Texture2D(_game.GraphicsDevice, outerRadius, outerRadius);

                    Color[] data = new Color[outerRadius * outerRadius];

                    // Colour the entire texture transparent first.
                    for (int i = 0; i < data.Length; i++)
                        data[i] = Color.Transparent;

                    // Work out the minimum step necessary using trigonometry + sine approximation.
                    double angleStep = 1f / _radius;

                    for (double angle = -Math.PI / 2; angle < Math.PI / 2; angle += angleStep)
                    {
                        int x = (int)Math.Round(_radius * Math.Cos(angle));
                        int y = (int)Math.Round(_radius + _radius * Math.Sin(angle));
                        for (int i = -x; i <= x; i++)
                        {
                            data[y * outerRadius + i + _radius] = Color.White;
                        }
                    }

                    _dummyTexture.SetData(data);
                    _needRefresh = false;
                }

                spriteBatch.Draw(_dummyTexture, new Vector2(_dummyRectangle.X + _dummyRectangle.Width / 2 - _radius, _dummyRectangle.Y + _dummyRectangle.Height / 2 - _radius), _colori);
            }

            public override void GetSelection(LinkedList<Sprite> objects, LinkedList<Sprite> ioSelection)
            {
                foreach (Sprite obj in objects)
                {
                    Rectangle rect = obj.BoundingRect;
                    int cx = _dummyRectangle.X + _dummyRectangle.Width / 2;
                    int cy = _dummyRectangle.Y + _dummyRectangle.Height / 2;

                    if (_radius > Math.Sqrt(Math.Pow(cx - rect.X, 2) + Math.Pow(cy - rect.Y, 2)))
                        ioSelection.AddLast(obj);
                    else if (_radius > Math.Sqrt(Math.Pow(cx - (rect.X + rect.Width), 2) + Math.Pow(cy - rect.Y, 2)))
                        ioSelection.AddLast(obj);
                    else if (_radius > Math.Sqrt(Math.Pow(cx - (rect.X + rect.Width), 2) + Math.Pow(cy - (rect.Y + rect.Height), 2)))
                        ioSelection.AddLast(obj);
                    else if (_radius > Math.Sqrt(Math.Pow(cx - rect.X, 2) + Math.Pow(cy - (rect.Y + rect.Height), 2)))
                        ioSelection.AddLast(obj);
                }
            }
        }
    }
}
