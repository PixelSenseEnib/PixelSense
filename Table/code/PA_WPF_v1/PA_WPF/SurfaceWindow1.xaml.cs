using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;

namespace PA_WPF
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        public Dictionary<long,Plane> planes = new Dictionary<long,Plane>();
        public Behaviour behaviour;

        private void InitializeTags()
        {

            for (byte k = 0; k <= 42; k++)
            {
                TagVisualizationDefinition tagDef =
                    new TagVisualizationDefinition();
                // The tag value that this definition will respond to.
                tagDef.Value = k;
                // The .xaml file for the UI
                tagDef.Source =
                    new Uri("TagVisualization1.xaml", UriKind.Relative);
                // The maximum number for this tag value.
                tagDef.MaxCount = 2;
                // The visualization stays for 2 seconds.
                tagDef.LostTagTimeout = 100.0;
                // Orientation offset (default).
                tagDef.OrientationOffsetFromTag = 0.0;
                // Physical offset (horizontal inches, vertical inches).
                tagDef.PhysicalCenterOffsetFromTag = new Vector(0.0, 0.0);
                // Tag removal behavior (default).
                tagDef.TagRemovedBehavior = TagRemovedBehavior.Fade;
                // Orient UI to tag? (default).
                tagDef.UsesTagOrientation = true;
                // Add the definition to the collection.
                MyTagVisualizer.Definitions.Add(tagDef);
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();
            string myHost = System.Net.Dns.GetHostName();


            string ip = System.Net.Dns.GetHostEntry(myHost).AddressList[0].ToString();

            Console.WriteLine("myHost : " + myHost + "myIP " + ip);

            //behaviour = new Behaviour(this.Width,this.Height, "192.168.76.222");
            behaviour = new Behaviour(this.Width, this.Height, ip);
            InitializeTags();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            TagVisualization1 camera = (TagVisualization1)e.TagVisualization;
            Plane plane = new Plane("plane" + camera.VisualizedTag.Value, camera.Center, camera.Orientation);
            planes.Add(camera.VisualizedTag.Value,plane);
            behaviour.Update(plane);
        }

        private void OnVisualizationMoved(object sender, TagVisualizerEventArgs e)
        {
            TagVisualization1 camera = (TagVisualization1)e.TagVisualization;
            if (planes.ContainsKey(camera.VisualizedTag.Value))
            {
                planes[camera.VisualizedTag.Value].Position = camera.Center;
                planes[camera.VisualizedTag.Value].Rotation = camera.Orientation;
                behaviour.Update(planes[camera.VisualizedTag.Value]);
                //Console.WriteLine("Avion : " + planes[camera.VisualizedTag.Value].Name+" en : "+planes[camera.VisualizedTag.Value].Position+ " Rotation : " + planes[camera.VisualizedTag.Value].Rotation);
            }

        }

        private void OnVisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
            TagVisualization1 camera = (TagVisualization1)e.TagVisualization;
            Point centreTag = camera.Center;
            if (planes.ContainsKey(camera.VisualizedTag.Value))
            {

                behaviour.Remove(planes[camera.VisualizedTag.Value]);
                planes.Remove(camera.VisualizedTag.Value);
            }
        }
    }
}