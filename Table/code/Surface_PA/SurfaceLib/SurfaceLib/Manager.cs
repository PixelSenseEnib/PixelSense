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

using Enib.SurfaceLib;

namespace Enib
{
    namespace SurfaceLib
    {
        public class MyTouchPoint : IEquatable<MyTouchPoint>
        {
            /// <summary>
            /// Getter of touch point
            /// </summary>
            public TouchPoint Touch
            {
                get { return _touch; }
            }
            private TouchPoint _touch = null;

            /// <summary>
            /// Getter and setter of asMove
            /// </summary>
            public bool AsMove
            {
                get { return _asMove; }
                set { _asMove = value; }
            }
            private bool _asMove = false;

            public MyTouchPoint(TouchPoint touch)
            {
                _touch = touch;
            }

            bool IEquatable<MyTouchPoint>.Equals(MyTouchPoint other)
            {
                return _touch.Id == other._touch.Id;
            }

            public override int GetHashCode() { return base.GetHashCode(); }
        }

        public class Manager
        {
            public enum SelectionMode { NONE, MONO, MULTI, RECTANGLE, CIRCLE };
            private LinkedList<Sprite> _objects = new LinkedList<Sprite>();
            private LinkedList<Sprite> _selectedObjects = new LinkedList<Sprite>();
            private LinkedList<MyTouchPoint> _touchPoints = new LinkedList<MyTouchPoint>();
            private SelectionMode _selectionMode = SelectionMode.MONO;
            private Game _game = null;
            private Behaviour _behaviour = null;

            Overlay _overlay = null;

            /// <summary>
            /// Selection getter
            /// </summary>
            public LinkedList<Sprite> SelectedObjects
            {
                get { return _selectedObjects; }
            }

            /// <summary>
            /// Behaviour setter
            /// </summary>
            public Behaviour Behaviour
            {
                set { _behaviour = value; }
            }

            /// <summary>
            /// Manager initialisation
            /// </summary>
            public virtual void Initialize(Game game, TouchTarget touchTarget, SelectionMode selectionMode = SelectionMode.MONO)
            {
                _game = game;
                _selectionMode = selectionMode;

                if (_selectionMode == SelectionMode.RECTANGLE)
                    _overlay = new RectangleOverlay(new Rectangle(0, 0, 0, 0), new Color(80, 80, 80, 50), game);
                else if (_selectionMode == SelectionMode.CIRCLE)
                    _overlay = new CircleOverlay(new Rectangle(0, 0, 0, 0), new Color(80, 80, 80, 50), game);

                _touchTarget = touchTarget;
                _touchTarget.TouchDown += new EventHandler<TouchEventArgs>(this.TouchedDown);
                _touchTarget.TouchMove += new EventHandler<TouchEventArgs>(this.TouchedMove);
                _touchTarget.TouchUp += new EventHandler<TouchEventArgs>(this.TouchedUp);
            }

            /// <summary>
            /// Register a sprite throw the manager
            /// </summary>
            /// <param name="s">Sprite to register</param>
            public void Register(Sprite s)
            {
                _objects.AddLast(s);
            }

            /// <summary>
            /// Update
            /// </summary>
            /// <param name="gameTime">Frame GameTime</param>
            public void Update(GameTime gameTime)
            {
                foreach (Sprite obj in _objects)
                {
                    obj.Update(gameTime);
                }

                if (_overlay != null && _touchPoints.Count == 2)
                {
                    _selectedObjects.Clear();
                    _overlay.GetSelection(_objects, _selectedObjects);
                }

                if( _behaviour != null )
                {
                    _behaviour.Update(_objects, _selectedObjects, _touchPoints);
                }
            }

            /// <summary>
            /// Load sprite texture
            /// </summary>
            /// <param name="content">ContentManager</param>
            public void LoadContent(ContentManager content)
            {
                foreach (Sprite obj in _objects)
                {
                    obj.LoadContent(content);
                }

                if( _overlay != null )
                    _overlay.LoadContent();
            }

            /// <summary>
            /// Draw sprites handled by the manager
            /// </summary>
            /// <param name="spriteBatch">SpriteBatch to use</param>
            /// <param name="gameTime">Frame GameTime</param>
            public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
            {
                foreach (Sprite obj in _objects)
                {
                    obj.Draw(spriteBatch, gameTime);
                }

                foreach (Sprite obj in _selectedObjects)
                {
                    obj.Draw(spriteBatch, gameTime, Color.Orange);
                }

                if (_touchPoints.Count == 2 && (_selectionMode == SelectionMode.CIRCLE || _selectionMode == SelectionMode.RECTANGLE))
                {
                    TouchPoint p1 = _touchPoints.First.Value.Touch;
                    TouchPoint p2 = _touchPoints.Last.Value.Touch;
                    int x = Math.Min((int)p1.CenterX, (int)p2.CenterX);
                    int y = Math.Min((int)p1.CenterY, (int)p2.CenterY);

                    int w = Math.Max((int)p1.CenterX, (int)p2.CenterX) - x;
                    int h = Math.Max((int)p1.CenterY, (int)p2.CenterY) - y;
                    _overlay.Rectangle = new Rectangle(x, y, w, h);
                    _overlay.Draw(spriteBatch, gameTime);
                }
            }

            /// <summary>
            /// Called at each touch up event
            /// </summary>
            /// <param name="sender">Object</param>
            /// <param name="e">EventArgs</param>
            public void TouchedUp(object sender, EventArgs e)
            {
                foreach (Sprite obj in _objects)
                {
                    if (obj.TouchedUp(sender, e) && !obj.AsMove && (_selectionMode == SelectionMode.MONO || _selectionMode == SelectionMode.MULTI))
                    {
                        if (_selectedObjects.Contains(obj))
                            _selectedObjects.Remove(obj);
                        else
                        {
                            if (_selectionMode == SelectionMode.MONO)
                            {
                                _selectedObjects.Clear();
                            }
                            _selectedObjects.AddLast(obj);
                        }
                    }
                }

                TouchEventArgs args = (TouchEventArgs)e;
                TouchPoint touch = args.TouchPoint;

                MyTouchPoint p = new MyTouchPoint(touch);
                if (_touchPoints.Contains(p))
                {
                    _touchPoints.Remove(p);
                }
            }

            /// <summary>
            /// Called at each touch down event
            /// </summary>
            /// <param name="sender">Object</param>
            /// <param name="e">EventArgs</param>
            public void TouchedDown(object sender, EventArgs e)
            {
                bool handled = false;
                foreach (Sprite obj in _objects)
                {
                    handled |= obj.TouchedDown(sender, e);
                }

                if (!handled)
                {
                    TouchEventArgs args = (TouchEventArgs)e;
                    TouchPoint touch = args.TouchPoint;

                    MyTouchPoint p = new MyTouchPoint(touch);
                    _touchPoints.AddLast(p);
                }
            }

            /// <summary>
            /// Called at each touch move event
            /// </summary>
            /// <param name="sender">Object</param>
            /// <param name="e">EventArgs</param>
            public void TouchedMove(object sender, EventArgs e)
            {
                foreach (Sprite obj in _objects)
                {
                    obj.TouchedMove(sender, e);
                }

                TouchEventArgs args = (TouchEventArgs)e;
                TouchPoint touch = args.TouchPoint;

                MyTouchPoint p = new MyTouchPoint(touch);
                if (_touchPoints.Contains(p))
                {
                    _touchPoints.Remove(p);
                    MyTouchPoint pTmp = new MyTouchPoint(touch);
                    p.AsMove = true;
                    _touchPoints.AddLast(pTmp);
                }
            }

            /// <summary>
            /// Getter and setter of touchTarget
            /// </summary>
            public TouchTarget TouchTarget
            {
                get { return _touchTarget; }
                set { _touchTarget = value; }
            }
            private TouchTarget _touchTarget;
        }
    }
}
