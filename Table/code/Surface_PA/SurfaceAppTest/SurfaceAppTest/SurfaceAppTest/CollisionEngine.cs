using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Enib.SurfaceLib;

namespace SurfaceAppTest
{
    class CollisionEngine
    {
        private LinkedList<Sprite> _objects = new LinkedList<Sprite>();

        double fFloor = 0.9;

        public void register( Sprite s )
        {
            _objects.AddLast(s);
        }

        public void update()
        {
            LinkedList<Sprite> toCompute = new LinkedList<Sprite>();
            foreach (Sprite obj in _objects) { toCompute.AddLast(obj); }

            while( toCompute.Count > 0 )
            {
                Sprite obj = toCompute.First.Value;
                toCompute.Remove(obj);
                foreach (Sprite objCol in _objects)
                {
                    if (obj == objCol)
                    {
                        continue;
                    }

                    if (obj.Intersect(objCol))
                    {
                        toCompute.Remove(objCol);

                        //if (obj.Speed.X > 0 && objCol.Speed.X >= 0)
                        float vx = obj.Speed.X - objCol.Speed.X;
                        float vy = obj.Speed.Y - objCol.Speed.Y;

                        if (obj.Speed.X < objCol.Speed.X)
                            vx = -vx;
                        /*if (obj.Speed.X > objCol.Speed.X)
                            vx = -vx;
                        if (obj.Speed.Y > objCol.Speed.Y)
                            vy = -vy;

                        if (obj.Weight == 0)
                            obj.Speed = new Vector2(vx, vy);
                        if (objCol.Weight == 0)
                            objCol.Speed = new Vector2(-vx, -vy);*/

                        //if (obj.Speed.X > 0 && objCol.Speed.X >= 0)
                        /*{
                                if( obj.Weight == 0 )
                                    obj.Speed = new Vector2(-vx, obj.Speed.Y);
                                if( objCol.Weight == 0 )
                                    objCol.Speed = new Vector2(vx, objCol.Speed.Y);
                        } */
                        
                        if (obj.Speed.Y > objCol.Speed.Y)
                        {
                            if (obj.Weight == 0)
                                obj.Speed = new Vector2(-vx, -obj.Speed.Y + objCol.Speed.Y);
                            if (objCol.Weight == 0)
                                objCol.Speed = new Vector2(vx, obj.Speed.Y - objCol.Speed.Y);
                        }
                        else
                        {
                            if (objCol.Weight == 0)
                                objCol.Speed = new Vector2(vx, obj.Speed.Y - objCol.Speed.Y);
                            if (obj.Weight == 0)
                                obj.Speed = new Vector2(-vx, objCol.Speed.Y - obj.Speed.Y);
                        }
                    }
                }
            }
        }
    }
}
