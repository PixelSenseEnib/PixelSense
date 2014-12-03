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
        public class Menu
        {
            public Sprite _caller;
            public List<MenuEntry> _menuEntries = new List<MenuEntry>();

            private TouchTarget _touchTarget;
            private Manager _manager;
            private bool _displayed = false;
        
            public Menu(Manager manager, Sprite c)
            {
                this._manager = manager;
                this._caller = c;
                this._caller.Menu = this;
            }

            /// <summary>
            /// Add entry to the menu
            /// </summary>
            /// <param name="MenuEntry">MenuEntry to add</param>
            public void addMenuEntry(MenuEntry menuEntry)
            {
                menuEntry.Initialize(_touchTarget);
                menuEntry.Dragable = false;
                menuEntry.Touchable = true;
                menuEntry.MenuCaller = this;
                _manager.Register(menuEntry);
                _menuEntries.Add(menuEntry);
            }

            /// <summary>
            /// Draw menu
            /// </summary>
            public void Dispose()
            {
                int last_position_x = (int)_caller.Position.X +(int)_caller.Texture.Width / 4;
                int last_position = (int)_caller.Position.Y; //+ (int)_caller.Texture.Height/4;
                int last_heigth = (int)_menuEntries.First().Size.Height;

                foreach (MenuEntry e in _menuEntries)
                {
                    e.Position = new Vector2(last_position_x, last_position + last_heigth);
                    last_position = (int)e.Position.Y;
                    last_heigth = e.Size.Height;
                }                
            }

            /// <summary>
            /// Hode  menu
            /// </summary>
            public void Hide()
            {
                foreach (MenuEntry e in _menuEntries)
                {
                    e.Position = new Vector2(-100, -100);
                }
                _displayed = false;
            }

            /// <summary>
            /// Show menu
            /// </summary>
            public void Show()
            {
                this.Dispose();
                _displayed = true;
            }

            /// <summary>
            /// Toggle menu 
            /// </summary>
            public void Toggle()
            {
                if (_displayed)
                    Hide();
                else
                    Show();
            }

            /// <summary>
            /// Initialize menu
            /// </summary>
            public virtual void Initialize(TouchTarget touchTarget)
            {
                _touchTarget = touchTarget;
            }

            /// <summary>
            /// Return true if menu is currently displayed
            /// </summary>
            public bool displayed()
            {
                return _displayed;
            }
        }
    }
}