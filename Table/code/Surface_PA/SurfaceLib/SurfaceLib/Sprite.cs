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
using System.Timers;

namespace Enib
{
    namespace SurfaceLib
    {
        public class Sprite
        {
            private int _touchId = 0;
            private Vector2 _delta = Vector2.Zero;
            private Timer _timer;
            private Menu _menu = null;

            /// <summary>
            /// Getter and setter of touchTarget
            /// </summary>
            public TouchTarget TouchTarget
            {
                get { return _touchTarget; }
                set { _touchTarget = value; }
            }
            private TouchTarget _touchTarget;

            /// <summary>
            /// Getter of menu
            /// </summary>
            public Menu Menu
            {
                set { this._menu = value; }
                get { return _menu; }
            }

            /// <summary>
            /// Getter and setter of scale
            /// </summary>
            public float Scale
            {
                get { return _scale; }
                set { _scale = value; }
            }
            private float _scale = 1;

            /// <summary>
            /// Getter and setter of weight
            /// </summary>
            public float Weight
            {
                get { return _weight; }
                set { _weight = value; }
            }
            private float _weight = 0;

            /// <summary>
            /// Getter and setter of rotation
            /// </summary>
            public float Rotation
            {
                get { return _rotation; }
                set { _rotation = value; }
            }
            protected float _rotation = 0;

            /// <summary>
            /// Getter and setter of touchable
            /// </summary>
            public bool Touchable
            {
                get { return _touchable; }
                set { _touchable = value; }
            }
            private bool _touchable = true;

            /// <summary>
            /// Getter and setter of dragable
            /// </summary>
            public bool Dragable
            {
                get { return _dragable; }
                set { _dragable = value; }
            }
            private bool _dragable = true;

            /// <summary>
            /// Getter of texture
            /// </summary>
            public Texture2D Texture
            {
                get { return _texture; }
            }
            private Texture2D _texture;

            /// <summary>
            /// Getter of size
            /// </summary>
            public Rectangle Size
            {
                get
                {
                    if (_texture == null)
                        throw new Exception("Can not get size before LoadContent");
                    return _texture.Bounds;
                }
            }

            /// <summary>
            /// Getter of the bounding rect
            /// </summary>
            public Rectangle BoundingRect
            {
                get
                {
                    int w = (int)((_texture.Width * Math.Abs(Math.Cos(Rotation)) + _texture.Height * Math.Abs(Math.Sin(Rotation))) * _scale);
                    int h = (int)((_texture.Height * Math.Abs(Math.Cos(Rotation)) + _texture.Width * Math.Abs(Math.Sin(Rotation))) * _scale);
                    return new Rectangle((int)(Position.X) - w / 2, (int)(Position.Y) - h / 2, w, h);
                }
            }

            /// <summary>
            /// Getter and setter of position
            /// </summary>
            public virtual Vector2 Position
            {
                get { return _position; }
                set { _position = value; }
            }
            protected Vector2 _position;

            /// <summary>
            /// Getter of as move
            /// </summary>
            public bool AsMove
            {
                get { return _asMove; }
            }
            private bool _asMove;

            /// <summary>
            /// Getter and setter of show bounding rect
            /// </summary>
            public virtual bool ShowBoundingRect
            {
                get { return _showBoundingRect; }
                set { _showBoundingRect = value; }
            }
            private bool _showBoundingRect = false;

            private string _assetName;

            /// <summary>
            /// Getter of name
            /// </summary>
            public string Name
            {
                get { return _name; }
            }
            private string _name;

            public Sprite(string name, string assetName)
            {
                _name = name;
                _assetName = assetName;
            }

            /// <summary>
            /// Sprite initialisation
            /// </summary>
            public virtual void Initialize(TouchTarget touchTarget)
            {
                _position = Vector2.Zero;
                _asMove = false;

                _touchTarget = touchTarget;
            }

            /// <summary>
            /// Load sprite texture
            /// </summary>
            /// <param name="content">Content Manager</param>
            /// <param name="assetName">L'asset name de l'image à charger pour ce Sprite</param>
            public virtual void LoadContent(ContentManager content)
            {
                _texture = content.Load<Texture2D>(this._assetName);
            }

            /// <summary>
            /// Called at each game update
            /// </summary>
            /// <param name="gameTime">Frame GameTime</param>
            public virtual void Update(GameTime gameTime)
            {

            }

            /// <summary>
            /// Called at each touch up event
            /// </summary>
            /// <param name="sender">Object</param>
            /// <param name="e">EventArgs</param>
            public virtual bool TouchedUp(object sender, EventArgs e)
            {
                TouchEventArgs args = (TouchEventArgs)e;
                TouchPoint touch = args.TouchPoint;

                if (_touchId == touch.Id)
                {
                    _touchId = 0;

                    if (_dragable)
                        _position = new Vector2(touch.CenterX - _delta.X, touch.CenterY - _delta.Y);

                    if (_menu != null)
                    {
                        _timer.Enabled = false;
                    }

                    return true;
                }
                return false;
            }

            /// <summary>
            /// Called at each touch down event
            /// </summary>
            /// <param name="sender">Object</param>
            /// <param name="e">EventArgs</param>
            public virtual bool TouchedDown(object sender, EventArgs e)
            {
                if (!_touchable) return false;
                TouchEventArgs args = (TouchEventArgs)e;
                TouchPoint touch = args.TouchPoint;

                if (BoundingRect.Contains((int)touch.CenterX, (int)touch.CenterY))
                {
                    _touchId = touch.Id;
                    _delta = new Vector2(touch.CenterX - Position.X, touch.CenterY - Position.Y);
                    _asMove = false;

                    if (_menu != null)
                    {
                        if (! _menu.displayed())
                        {
                            _timer = new System.Timers.Timer(1000);
                            _timer.Enabled = true;
                            _timer.Elapsed += OnTimedEvent;
                        }
                        else
                        {
                            this._menu.Hide();
                        }
                    }
                    return true;
                }
                return false;
            }


            /// <summary>
            /// Called at each touch move event
            /// </summary>
            /// <param name="sender">Object</param>
            /// <param name="e">EventArgs</param>
            public virtual bool TouchedMove(object sender, EventArgs e)
            {
                TouchEventArgs args = (TouchEventArgs)e;
                TouchPoint touch = args.TouchPoint;
                
                if (_dragable && _touchId == touch.Id)
                {
                    _position = new Vector2(touch.CenterX - _delta.X, touch.CenterY - _delta.Y);
                    _asMove = true;
                    return true;
                }
                return false;
            }

            public void OnTimedEvent(object sender, ElapsedEventArgs e)
            {
                if (! _menu.displayed())
                    this._menu.Show();
            }

            /// <summary>
            /// Draw sprite
            /// </summary>
            /// <param name="spriteBatch">SpriteBatch to use</param>
            /// <param name="gameTime">frame gametime</param>
            /// <param name="color">color</param>
            public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime, Color? color = null)
            {
                Color tmp = color.HasValue ? color.Value : Color.White;
                spriteBatch.Draw(_texture, new Vector2(computedX(), computedY()), null, tmp, _rotation, Vector2.Zero, _scale, SpriteEffects.None, 0f);

                if (_showBoundingRect)
                {
                    Texture2D tx = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                    tx.SetData(new Color[] { Color.White });
                    spriteBatch.Draw(tx, BoundingRect, new Color(80, 80, 80, 50));
                    spriteBatch.Draw(tx, new Rectangle((int)computedX() - 5, (int)computedY() - 5, 10, 10), new Color(80, 80, 120, 50));
                }
            }

            protected float computedX()
            {
                return _position.X - (Size.Width * Scale / 2) * (float)Math.Cos(Rotation) + (Size.Height * Scale / 2) * (float)Math.Sin(Rotation);
            }

            protected float computedY()
            {
                return _position.Y - (Size.Height * Scale / 2) * (float)Math.Cos(Rotation) - (Size.Width * Scale / 2) * (float)Math.Sin(Rotation);
            }

            /// <summary>
            /// Compute intersection with an other Sprite
            /// </summary>
            /// <param name="s">Sprite to compute</param>
            public bool Intersect(Sprite s)
            {
                Rectangle rect = new Rectangle((int)s.Position.X, (int)s.Position.Y, s.Size.Width, s.Size.Height);
                return this.Intersect(rect);
            }

            /// <summary>
            /// Compute intersection with a rectangle
            /// </summary>
            /// <param name="s">Rectangle to compute</param>
            public bool Intersect(Rectangle r)
            {
                if (r.X > _position.X && r.X < _position.X + _texture.Width &&
                    r.Y > _position.Y && r.Y < _position.Y + _texture.Height)
                {
                    return true;
                }
                if (r.X + r.Width > _position.X && r.X + r.Width < _position.X + _texture.Width &&
                     r.Y > _position.Y && r.Y < _position.Y + _texture.Height)
                {
                    return true;
                }
                if (r.X + r.Width > _position.X && r.X + r.Width < _position.X + _texture.Width &&
                     r.Y + r.Height > _position.Y && r.Y + r.Height < _position.Y + _texture.Height)
                {
                    return true;
                }
                if (r.X > _position.X && r.X < _position.X + _texture.Width &&
                     r.Y + r.Height > _position.Y && r.Y + r.Height < _position.Y + _texture.Height)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
