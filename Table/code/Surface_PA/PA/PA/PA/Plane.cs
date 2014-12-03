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

namespace enib
{
    namespace pa
    {
        public class Plane : Sprite
        {
            /// <summary>
            /// Getter of speed
            /// </summary>
            public float Speed
            {
                get { return _speedCurent; }
            }
            private int _speed = 10;
            private float _speedCurent = 00;
            //private float _speedR = 0.05f;

            /// <summary>
            /// Getter and setter of target
            /// </summary>
            public Vector2 Target
            {
                get { return _target; }
                set
                {
                    _target = new Vector2(value.X, value.Y);
                }
            }
            private Vector2 _target;

            /// <summary>
            /// Getter and setter of target
            /// </summary>
            public override Vector2 Position
            {
                get { return base.Position; }
                set { 
                    _target = value;
                    base.Position = value;
                }
            }

            public Plane(string name): base( name, "plane")
            {
            }

            /// <summary>
            /// Update plane pos
            /// </summary>
            /// <param name="gameTime">frame GameTime</param>
            public override void Update(GameTime gameTime)
            {
                Vector2 direction = _target - _position;
                _speedCurent = Math.Min(direction.Length() / 10, _speed);
                if (_speedCurent != 0)
                    direction.Normalize();

                if (direction.X != 0 && _speedCurent > 0.7)
                {
                    Rotation = (float)Math.Atan(direction.Y / direction.X) + (float)Math.PI / 2;
                    if (direction.X < 0)
                        Rotation += (float)Math.PI;
                    
                    //Rotation += d;
                }

                Vector2 tmp = new Vector2((float)Math.Sin(Rotation), -(float)Math.Cos(Rotation));

                _position = Position + direction * _speedCurent;
            }
        }
    }
}
