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
            
            // http://en.sfml-dev.org/forums/index.php?topic=12466.msg87073#msg87073
            var rendersurface = new DrawingSurface(); // our control for SFML to draw on
            rendersurface.Size = new System.Drawing.Size(250, 250); // set our SFML surface control size to be 500 width & 500 height
            sb.Controls.Add(rendersurface); // add the SFML surface control to our form
            rendersurface.Location = new System.Drawing.Point(545, 44); // center our control on the form

            SFML_Window window = new SFML_Window(sb, rendersurface);
            window.Run();

        }
    }
}
