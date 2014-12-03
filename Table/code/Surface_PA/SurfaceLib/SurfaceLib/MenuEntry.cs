using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enib.SurfaceLib;
using Microsoft.Xna.Framework;
using Microsoft.Surface.Core;

namespace Enib
{
    namespace SurfaceLib
    {
        public abstract class MenuEntry : Sprite
        {
            public Sprite _caller;
            public List<Sprite> _sprites = new List<Sprite>();

            private Menu menuCaller;


            /// <summary>
            /// Setter of menuCaller
            /// </summary>
            public Menu MenuCaller
            {
                set { menuCaller = value; }
            }

            public MenuEntry(String assetName) : base("", assetName)
            {
            }

            /// <summary>
            /// Called at each touch down event
            /// </summary>
            /// <param name="sender">Object</param>
            /// <param name="e">EventArgs</param>
            public override bool TouchedDown(object sender, EventArgs e)
            {
                bool ret = base.TouchedDown(sender, e);

                if (ret)
                {
                    this.MenuAction();
                    this.menuCaller.Hide();
                }

                return ret;
            }

            public abstract void MenuAction();
        }
    }
}