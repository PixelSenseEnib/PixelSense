using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enib.SurfaceLib;
using enib.pa;
using PA;

namespace PA
{
    public class Plane_Menu_Elevator : MenuEntry
    {
        private enib.pa.Plane plane;
        public Plane_Menu_Elevator(string assetName) : base(assetName) { }

        public void setPlaneur(enib.pa.Plane plane)
        {
            this.plane = plane;
        }

        public override void MenuAction()
        {
            this.plane.Target = new Microsoft.Xna.Framework.Vector2(570,735);  
        }
    }
}

