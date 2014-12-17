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
using System.Net;

namespace PA_PixelSense
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindowAircraftCarrier : SurfaceWindow
    {
        // Dictionary containing a tag value and a plane
        public Dictionary<long, Plane> planes = new Dictionary<long, Plane>();
        // Behaviour
        public Behaviour behaviour;
        // Number of tags desired
        private int nbTags = 4;


        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindowAircraftCarrier()
        {
            InitializeComponent();

            // Initialize Behaviour
            behaviour = new Behaviour(this.Width, this.Height, GetServerIP());

            InitializeTags();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Get IP of Server
        /// </summary>
        /// <returns></returns>
        public string GetServerIP()
        {
            // --- Get IP dynamically ---

            //string host = System.Net.Dns.GetHostName();
            //IPAddress[] ip = Dns.GetHostAddresses(host);
            //string serverIP = "";
            //foreach (IPAddress theaddress in ip)
            //{
            //    if (theaddress.ToString().Contains("192"))
            //    {
            //        serverIP = theaddress.ToString();
            //    }
            //    Console.WriteLine(theaddress.ToString());
            //}
            //return serverIP;


            return "192.168.76.222";

        }

        /// <summary>
        /// Initialize the tags
        /// </summary>
        private void InitializeTags()
        {
            //Initialize the tags associated to the planes, nbTags is the number of planes 
            for (byte k = 0; k <= nbTags; k++)
            {
                TagVisualizationDefinition tagDef =
                    new TagVisualizationDefinition();
                // The tag value that this definition will respond to.
                tagDef.Value = k;
                // The .xaml file for the UI
                tagDef.Source =
                    new Uri("PlaneTagVisualization.xaml", UriKind.Relative);
                // The maximum number for this tag value.
                tagDef.MaxCount = 1;
                // The visualization stays for 100 milliseconds.
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
                TagVisualizerPlane.Definitions.Add(tagDef);
            }
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

        /// <summary>
        /// This is called when a tag is added on the SurfaceWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlaneVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            PlaneTagVisualization camera = (PlaneTagVisualization)e.TagVisualization;

            // Create a new plane whose name is planeTagValue (TagValue is the value of the tag), and position and orientation the ones of the tags
            Plane plane = new Plane("plane" + camera.VisualizedTag.Value, camera.Center, camera.Orientation);

            //Add the tag and the plane to the dictionary planes
            planes.Add(camera.VisualizedTag.Value, plane);

            //Update the position of the plane in the 3D Environment 
            behaviour.Update(plane);
        }

        /// <summary>
        /// This is called when a tag is moved on the SurfaceWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlaneVisualizationMoved(object sender, TagVisualizerEventArgs e)
        {
            PlaneTagVisualization camera = (PlaneTagVisualization)e.TagVisualization;

            // Update the position and orientation of the plane 
            if (planes.ContainsKey(camera.VisualizedTag.Value))
            {
                // Update the position of the plane 
                planes[camera.VisualizedTag.Value].Position = camera.Center;
                planes[camera.VisualizedTag.Value].Rotation = camera.Orientation;

                // Update the position of the plane in the 3D Environment
                behaviour.Update(planes[camera.VisualizedTag.Value]);

            }

        }

        /// <summary>
        /// This is called when a tag is removed from the SurfaceWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlaneVisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
            PlaneTagVisualization camera = (PlaneTagVisualization)e.TagVisualization;

            if (planes.ContainsKey(camera.VisualizedTag.Value))
            {
                // Remove the plane from the 3D Environment
                behaviour.Remove(planes[camera.VisualizedTag.Value]);

                // Remove the plane from the dictionary
                planes.Remove(camera.VisualizedTag.Value);
            }
        }
    }
}