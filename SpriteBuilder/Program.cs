using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SpriteBuilder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            var sb = new SpriteBuilderForm();
            sb.Show();

            var rendersurface = new DrawingSurface(); // our control for SFML to draw on
            rendersurface.Size = new System.Drawing.Size(250, 250); // set our SFML surface control size to be 500 width & 500 height
            sb.Controls.Add(rendersurface); // add the SFML surface control to our form
            rendersurface.Location = new System.Drawing.Point(575, 44); // center our control on the form

            // initialize sfml
            var renderwindow = new SFML.Graphics.RenderWindow(rendersurface.Handle); // creates our SFML RenderWindow on our surface control

            // drawing loop
            while (sb.Visible) // loop while the window is open
            {
                System.Windows.Forms.Application.DoEvents(); // handle form events
                renderwindow.DispatchEvents(); // handle SFML events - NOTE this is still required when SFML is hosted in another window
                renderwindow.Clear(SFML.Graphics.Color.Yellow); // clear our SFML RenderWindow
                renderwindow.Display(); // display what SFML has drawn to the screen
            }

        }
    }
}
