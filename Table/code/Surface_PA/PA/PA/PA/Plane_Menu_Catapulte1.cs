using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enib.SurfaceLib;

namespace PA
{
    class Plane_Menu_Catapulte1 : MenuEntry
    {
        private enib.pa.Plane plane;
        public Plane_Menu_Catapulte1(string assetName) : base(assetName) { }

        public void setPlaneur(enib.pa.Plane plane)
        {
            this.plane = plane;
        }

        public override void MenuAction()
        {

            this.plane.Target = new Microsoft.Xna.Framework.Vector2(605,404);  
        }
    }
}


