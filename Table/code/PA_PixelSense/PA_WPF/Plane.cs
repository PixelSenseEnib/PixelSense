using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PA_PixelSense
{
    public class Plane
    {
        private Point _position = new Point();
        private double _rotation = 0;
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public double Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public Point Position
        {
            get { return _position; }
            set { _position = value; }
        }
        

        public Plane(string name,Point position, double rotation)
        {
            _name = name;
            _position = position;
            _rotation = rotation;
        }


        
    }
}
