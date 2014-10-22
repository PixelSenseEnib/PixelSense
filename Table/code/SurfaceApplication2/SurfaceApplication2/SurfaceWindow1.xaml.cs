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

namespace SurfaceApplication2
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        /// <summary>
        /// Default constructor.
        /// 
        /// </summary>
        /// 
        public bool echec = true;

        public List<List<Rectangle>> squares = new List<List<Rectangle>>();

        public SurfaceWindow1()
        {
            
            InitializeComponent();
            InitializeDefinitions();
            FillArrayOfSquare();
            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
            echec = true;


            
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
        private void InitializeDefinitions()
        {
           
            for (byte k = 1; k <= 5; k++)
            {
                TagVisualizationDefinition tagDef =
                    new TagVisualizationDefinition();
                // The tag value that this definition will respond to.
                tagDef.Value = k;
                // The .xaml file for the UI
                tagDef.Source =
                    new Uri("CameraVisualization.xaml", UriKind.Relative);
                // The maximum number for this tag value.
                tagDef.MaxCount = 2;
                // The visualization stays for 2 seconds.
                tagDef.LostTagTimeout = 2000.0;
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

        private void FillArrayOfSquare()
        {
           
            for (int i = 0; i < 10; i++)
            {
                //
                // Put some integers in the inner lists.
                //
                List<Rectangle> sublist = new List<Rectangle>();
                
                for (int v = 0; v < 10; v++)
                {
                    string name ="rectangle"+ i.ToString() + "_" + v.ToString();
                    sublist.Add((Rectangle)FindName(name));
                }
                //
                // Add the sublist to the top-level List reference.
                //
                squares.Add(sublist);
                
            }

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
        private void OnVisualizationMoved(object sender, TagVisualizerEventArgs e) {
            
                CameraVisualization camera = (CameraVisualization)e.TagVisualization;
                Point centreTag = camera.Center;
                Boolean cont2 = true;
                for (int i = 0; i < 10; i++)
                {
                    if (!cont2)
                        break;
                    for (int j = 0; j < 10; j++)
                    {
                        Rectangle sq = squares[i][j];
                        double left = Canvas.GetLeft(sq);
                        double top = Canvas.GetTop(sq);
                        if (left <= (centreTag.X) && (centreTag.X) <= (left + 90) && top <= centreTag.Y && (top + 90) >= centreTag.Y)
                        {
                            if ((left+20) <= (centreTag.X) && (centreTag.X) <= (left + 70) && (top+20) <= centreTag.Y && (top + 70) >= centreTag.Y)
                            { OnVisualizationAdded(sender, e); } 
                            else
                            { OnVisualizationRemoved(sender, e); }
                        }
                    }
                }
                
           
        }
        private void OnVisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
            CameraVisualization camera = (CameraVisualization)e.TagVisualization;
            Point centreTag = camera.Center;
            if (!echec)
            {
                switch (camera.VisualizedTag.Value)
                {
                    case 1:
                            Boolean cont2 = true;
                            for (int i = 0; i < 10; i++)
                            {
                                if (!cont2)
                                    break;
                                for (int j = 0; j < 10; j++)
                                {
                                    Rectangle sq = squares[i][j];
                                    double left = Canvas.GetLeft(sq);
                                    double top = Canvas.GetTop(sq);
                                    Color color;
                                    if ((i + j) % 2 == 0)
                                    {
                                        color = System.Windows.Media.Colors.Black;
                                    }
                                    else {
                                        color = System.Windows.Media.Colors.White;
                                    }
                                    if (left <= (centreTag.X) && (centreTag.X) <= (left + 90) && top <= centreTag.Y && (top + 90) >= centreTag.Y)
                                    {
                                        if (i != 9 && j != 0)
                                            squares[i + 1][j - 1].Fill = new SolidColorBrush(color);
                                        if (i < 8 && j > 1)
                                            squares[i + 2][j - 2].Fill = new SolidColorBrush(color);
                                        if (i < 7 && j > 2)
                                            squares[i + 3][j - 3].Fill = new SolidColorBrush(color);
                                        if (i < 6 && j > 3)
                                            squares[i + 4][j - 4].Fill = new SolidColorBrush(color);
                                        if (i < 5 && j > 4)
                                            squares[i + 5][j - 5].Fill = new SolidColorBrush(color);
                                        if (i < 4 && j > 5)
                                            squares[i + 6][j - 6].Fill = new SolidColorBrush(color);
                                        if (i < 3 && j > 6)
                                            squares[i + 7][j - 7].Fill = new SolidColorBrush(color);
                                        if (i < 2 && j > 7)
                                            squares[i + 8][j - 8].Fill = new SolidColorBrush(color);
                                        if (i < 1 && j > 8)
                                            squares[i + 9][j - 9].Fill = new SolidColorBrush(color);

                                        if (i != 9 && j != 9)
                                            squares[i + 1][j + 1].Fill = new SolidColorBrush(color);
                                        if (i < 8 && j < 8)
                                            squares[i + 2][j + 2].Fill = new SolidColorBrush(color);
                                        if (i < 7 && j < 7)
                                            squares[i + 3][j + 3].Fill = new SolidColorBrush(color);
                                        if (i < 6 && j < 6)
                                            squares[i + 4][j + 4].Fill = new SolidColorBrush(color);
                                        if (i < 5 && j < 5)
                                            squares[i + 5][j + 5].Fill = new SolidColorBrush(color);
                                        if (i < 4 && j < 4)
                                            squares[i + 6][j + 6].Fill = new SolidColorBrush(color);
                                        if (i < 3 && j < 3)
                                            squares[i + 7][j + 7].Fill = new SolidColorBrush(color);
                                        if (i < 2 && j < 2)
                                            squares[i + 8][j + 8].Fill = new SolidColorBrush(color);
                                        if (i < 1 && j < 1)
                                            squares[i + 9][j + 9].Fill = new SolidColorBrush(color);

                                        if (i != 0 && j != 0)
                                            squares[i - 1][j - 1].Fill = new SolidColorBrush(color);
                                        if (i > 1 && j > 1)
                                            squares[i - 2][j - 2].Fill = new SolidColorBrush(color);
                                        if (i > 2 && j > 2)
                                            squares[i - 3][j - 3].Fill = new SolidColorBrush(color);
                                        if (i > 3 && j > 3)
                                            squares[i - 4][j - 4].Fill = new SolidColorBrush(color);
                                        if (i > 4 && j > 4)
                                            squares[i - 5][j - 5].Fill = new SolidColorBrush(color);
                                        if (i > 5 && j > 5)
                                            squares[i - 6][j - 6].Fill = new SolidColorBrush(color);
                                        if (i > 6 && j > 6)
                                            squares[i - 7][j - 7].Fill = new SolidColorBrush(color);
                                        if (i > 7 && j > 7)
                                            squares[i - 8][j - 8].Fill = new SolidColorBrush(color);
                                        if (i > 8 && j > 8)
                                            squares[i - 9][j - 9].Fill = new SolidColorBrush(color);

                                        if (i != 0 && j != 9)
                                            squares[i - 1][j + 1].Fill = new SolidColorBrush(color);
                                        if (i > 1 && j < 8)
                                            squares[i - 2][j + 2].Fill = new SolidColorBrush(color);
                                        if (i > 2 && j < 7)
                                            squares[i - 3][j + 3].Fill = new SolidColorBrush(color);
                                        if (i > 3 && j < 6)
                                            squares[i - 4][j + 4].Fill = new SolidColorBrush(color);
                                        if (i > 4 && j < 5)
                                            squares[i - 5][j + 5].Fill = new SolidColorBrush(color);
                                        if (i > 5 && j < 4)
                                            squares[i - 6][j + 6].Fill = new SolidColorBrush(color);
                                        if (i > 6 && j < 3)
                                            squares[i - 7][j + 7].Fill = new SolidColorBrush(color);
                                        if (i > 7 && j < 2)
                                            squares[i - 8][j + 8].Fill = new SolidColorBrush(color);
                                        if (i > 8 && j < 1)
                                            squares[i - 9][j + 9].Fill = new SolidColorBrush(color);
                                        cont2 = false;
                                        break;
                                    }
                                }
                            }
                        break;
                    default:
                        if (camera.Orientation >= 0.0f && camera.Orientation < 180.0f)
                        {
                            camera.CameraModel.Content = "Pion noir";
                            camera.myEllipse.Fill = SurfaceColors.Accent2Brush;
                            Boolean cont = true;
                            for (int i = 0; i < 10; i++)
                            {
                                if (!cont)
                                    break;
                                for (int j = 0; j < 10; j++)
                                {
                                    Rectangle sq = squares[i][j];
                                    double left = Canvas.GetLeft(sq);
                                    double top = Canvas.GetTop(sq);
                                    Color color;
                                    if ((i + j) % 2 == 0)
                                    {
                                        color = System.Windows.Media.Colors.Black;
                                    }
                                    else
                                    {
                                        color = System.Windows.Media.Colors.White;
                                    }
                                    if (left <= (centreTag.X) && (centreTag.X) <= (left + 90) && top <= centreTag.Y && (top + 90) >= centreTag.Y)
                                    {
                                        Console.WriteLine(left.ToString() + "-" + top.ToString() + "-" + centreTag.X + "-" + centreTag.Y + "i" + i + "j" + j);
                                        if (i != 9 && j != 0)
                                            squares[i + 1][j - 1].Fill = new SolidColorBrush(color);
                                        if (i != 9 && j != 9)
                                            squares[i + 1][j + 1].Fill = new SolidColorBrush(color);
                                        cont = false;
                                        break;
                                    }
                                }
                            }

                        }
                        else
                        {
                            camera.CameraModel.Content = "Pion blanc";
                            camera.myEllipse.Fill = SurfaceColors.Accent4Brush;
                            Boolean cont = true;
                            for (int i = 0; i < 10; i++)
                            {
                                if (!cont)
                                    break;
                                for (int j = 0; j < 10; j++)
                                {
                                    Rectangle sq = squares[i][j];
                                    double left = Canvas.GetLeft(sq);
                                    double top = Canvas.GetTop(sq);
                                    Color color;
                                    if ((i + j) % 2 == 0)
                                    {
                                        color = System.Windows.Media.Colors.Black;
                                    }
                                    else
                                    {
                                        color = System.Windows.Media.Colors.White;
                                    }
                                    if (left <= (centreTag.X) && (centreTag.X) <= (left + 90) && top <= centreTag.Y && (top + 90) >= centreTag.Y)
                                    {
                                        Console.WriteLine(left.ToString() + "-" + top.ToString() + "-" + centreTag.X + "-" + centreTag.Y + "i" + i + "j" + j);
                                        if (i != 0 && j != 0)
                                            squares[i - 1][j - 1].Fill = new SolidColorBrush(color);
                                        if (i != 0 && j != 9)
                                            squares[i - 1][j + 1].Fill = new SolidColorBrush(color);
                                        cont = false;
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                }

                return;

            }
        }
        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            //object gestion on the checker board
            CameraVisualization camera = (CameraVisualization)e.TagVisualization;
            Point centreTag = camera.Center;
            if (!echec) {
                switch (camera.VisualizedTag.Value)
                {   
                    
                    case 1 :
                        Color color;
                        if (camera.Orientation >= 0.0f && camera.Orientation < 180.0f)
                        {
                            camera.CameraModel.Content = "Dame blanche";
                            camera.myEllipse.Fill = SurfaceColors.Accent4Brush;
                            color = System.Windows.Media.Colors.DarkMagenta;
                        }
                        else
                        {   
                            camera.CameraModel.Content = "Dame noire";
                            camera.myEllipse.Fill = SurfaceColors.Accent2Brush;
                            color = System.Windows.Media.Colors.Brown;
                        }
                        Boolean cont3 = true;
                        for (int i = 0; i < 10; i++)
                            {
                                if (!cont3)
                                    break;
                                for (int j = 0; j < 10; j++)
                                {
                                    Rectangle sq = squares[i][j];
                                    double left = Canvas.GetLeft(sq);
                                    double top = Canvas.GetTop(sq);
                                    if (left <= (centreTag.X) && (centreTag.X) <= (left + 90) && top <= centreTag.Y && (top + 90) >= centreTag.Y)
                                    {
                                        if (i != 9 && j != 0)
                                            squares[i + 1][j - 1].Fill = new SolidColorBrush(color);
                                        if (i < 8 && j > 1)
                                            squares[i + 2][j - 2].Fill = new SolidColorBrush(color);
                                        if (i < 7 && j > 2)
                                            squares[i + 3][j - 3].Fill = new SolidColorBrush(color);
                                        if (i < 6 && j > 3)
                                            squares[i + 4][j - 4].Fill = new SolidColorBrush(color);
                                        if (i < 5 && j > 4)
                                            squares[i + 5][j - 5].Fill = new SolidColorBrush(color);
                                        if (i < 4 && j > 5)
                                            squares[i + 6][j - 6].Fill = new SolidColorBrush(color);
                                        if (i < 3 && j > 6)
                                            squares[i + 7][j - 7].Fill = new SolidColorBrush(color);
                                        if (i < 2 && j > 7)
                                            squares[i + 8][j - 8].Fill = new SolidColorBrush(color);
                                        if (i < 1 && j > 8)
                                            squares[i + 9][j - 9].Fill = new SolidColorBrush(color);

                                        if (i != 9 && j != 9)
                                            squares[i + 1][j + 1].Fill = new SolidColorBrush(color);
                                        if (i < 8 && j < 8)
                                            squares[i + 2][j + 2].Fill = new SolidColorBrush(color);
                                        if (i < 7 && j < 7)
                                            squares[i + 3][j + 3].Fill = new SolidColorBrush(color);
                                        if (i < 6 && j < 6)
                                            squares[i + 4][j + 4].Fill = new SolidColorBrush(color);
                                        if (i < 5 && j < 5)
                                            squares[i + 5][j + 5].Fill = new SolidColorBrush(color);
                                        if (i < 4 && j < 4)
                                            squares[i + 6][j + 6].Fill = new SolidColorBrush(color);
                                        if (i < 3 && j < 3)
                                            squares[i + 7][j + 7].Fill = new SolidColorBrush(color);
                                        if (i < 2 && j < 2)
                                            squares[i + 8][j + 8].Fill = new SolidColorBrush(color);
                                        if (i < 1 && j < 1)
                                            squares[i + 9][j + 9].Fill = new SolidColorBrush(color);

                                        if (i != 0 && j != 0)
                                            squares[i - 1][j - 1].Fill = new SolidColorBrush(color);
                                        if (i > 1 && j > 1)
                                            squares[i - 2][j - 2].Fill = new SolidColorBrush(color);
                                        if (i > 2 && j > 2)
                                            squares[i - 3][j - 3].Fill = new SolidColorBrush(color);
                                        if (i > 3 && j > 3)
                                            squares[i - 4][j - 4].Fill = new SolidColorBrush(color);
                                        if (i > 4 && j > 4)
                                            squares[i - 5][j - 5].Fill = new SolidColorBrush(color);
                                        if (i > 5 && j > 5)
                                            squares[i - 6][j - 6].Fill = new SolidColorBrush(color);
                                        if (i > 6 && j > 6)
                                            squares[i - 7][j - 7].Fill = new SolidColorBrush(color);
                                        if (i > 7 && j > 7)
                                            squares[i - 8][j - 8].Fill = new SolidColorBrush(color);
                                        if (i > 8 && j > 8)
                                            squares[i - 9][j - 9].Fill = new SolidColorBrush(color);

                                        if (i != 0 && j != 9)
                                            squares[i - 1][j + 1].Fill = new SolidColorBrush(color);
                                        if (i > 1 && j < 8)
                                            squares[i - 2][j + 2].Fill = new SolidColorBrush(color);
                                        if (i > 2 && j < 7)
                                            squares[i - 3][j + 3].Fill = new SolidColorBrush(color);
                                        if (i > 3 && j < 6)
                                            squares[i - 4][j + 4].Fill = new SolidColorBrush(color);
                                        if (i > 4 && j < 5)
                                            squares[i - 5][j + 5].Fill = new SolidColorBrush(color);
                                        if (i > 5 && j < 4)
                                            squares[i - 6][j + 6].Fill = new SolidColorBrush(color);
                                        if (i > 6 && j < 3)
                                            squares[i - 7][j + 7].Fill = new SolidColorBrush(color);
                                        if (i > 7 && j < 2)
                                            squares[i - 8][j + 8].Fill = new SolidColorBrush(color);
                                        if (i > 8 && j < 1)
                                            squares[i - 9][j + 9].Fill = new SolidColorBrush(color);
                                        cont3 = false;
                                        break;
                                    }
                                }
                            }
                        break;
                    default:
                        if (camera.Orientation >= 0.0f && camera.Orientation < 180.0f)
                        {
                            camera.CameraModel.Content = "Pion noir";
                            camera.myEllipse.Fill = SurfaceColors.Accent2Brush;
                            Boolean cont = true;
                            for (int i = 0; i < 10; i++)
                            {
                                if (!cont)
                                    break;
                                for (int j = 0; j < 10; j++)
                                {
                                    Rectangle sq = squares[i][j];
                                    double left = Canvas.GetLeft(sq);
                                    double top = Canvas.GetTop(sq);
                                    if (left <= (centreTag.X) && (centreTag.X) <= (left + 90) && top <= centreTag.Y && (top + 90) >= centreTag.Y)
                                    {
                                        Console.WriteLine(left.ToString() + "-" + top.ToString() + "-" + centreTag.X + "-" + centreTag.Y + "i" + i + "j" + j);
                                        if (i != 9 && j != 0)
                                            squares[i + 1][j - 1].Fill = new SolidColorBrush(System.Windows.Media.Colors.DarkMagenta);
                                        if (i != 9 && j != 9)
                                            squares[i + 1][j + 1].Fill = new SolidColorBrush(System.Windows.Media.Colors.DarkMagenta);
                                        cont = false;
                                        break;
                                    }
                                }
                            }
                           
                        }
                        else
                        {
                            camera.CameraModel.Content = "Pion blanc";
                            camera.myEllipse.Fill = SurfaceColors.Accent4Brush;
                            Boolean cont = true;
                            for (int i = 0; i < 10; i++)
                            {
                                if (!cont)
                                    break;
                                for (int j = 0; j < 10; j++)
                                {
                                    Rectangle sq = squares[i][j];
                                    double left = Canvas.GetLeft(sq);
                                    double top = Canvas.GetTop(sq);
                                    
                                    if (left <= (centreTag.X) && (centreTag.X) <= (left + 90) && top <= centreTag.Y && (top + 90) >= centreTag.Y)
                                    {
                                        Console.WriteLine(left.ToString() + "-" + top.ToString() + "-" + centreTag.X + "-" + centreTag.Y + "i" + i + "j" + j);
                                        if ( i != 0 && j != 0)
                                            squares[i - 1][j - 1].Fill = new SolidColorBrush(System.Windows.Media.Colors.Brown);
                                        if ( i !=0 && j != 9)
                                            squares[i - 1][j + 1].Fill = new SolidColorBrush(System.Windows.Media.Colors.Brown);
                                        cont = false;
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                }
                
                return; 
            }
            switch (camera.VisualizedTag.Value)
            {
                case 1:
                    if (camera.Orientation >= 0.0f && camera.Orientation < 180.0f)
                    {
                        camera.CameraModel.Content = "Tour noire";
                        camera.myEllipse.Fill = SurfaceColors.Accent2Brush;
                    }
                    else
                    {
                        camera.CameraModel.Content = "Tour blanche";
                        camera.myEllipse.Fill = SurfaceColors.Accent4Brush;
                    }
                    break;
                case 2:
                    if (camera.Orientation >= 0.0f && camera.Orientation < 180.0f)
                    {
                        camera.CameraModel.Content = "Fou noir";
                        camera.myEllipse.Fill = SurfaceColors.Accent2Brush;
                    }
                    else
                    {
                        camera.CameraModel.Content = "Fou blanc";
                        camera.myEllipse.Fill = SurfaceColors.Accent4Brush;
                    }
                    break;
                case 3:
                    if (camera.Orientation >= 0.0f && camera.Orientation < 180.0f)
                    {
                        camera.CameraModel.Content = "Cavalier noir";
                        camera.myEllipse.Fill = SurfaceColors.Accent2Brush;
                    }
                    else
                    {
                        camera.CameraModel.Content = "Cavalier blanc";
                        camera.myEllipse.Fill = SurfaceColors.Accent4Brush;
                    }
                    camera.myEllipse.Fill = SurfaceColors.Accent2Brush;
                    break;
                case 4:
                    if (camera.Orientation >= 0.0f && camera.Orientation < 180.0f)
                    {
                        camera.CameraModel.Content = "Roi noir";
                        camera.myEllipse.Fill = SurfaceColors.Accent2Brush;
                    }
                    else
                    {
                        camera.CameraModel.Content = "Roi blanc";
                        camera.myEllipse.Fill = SurfaceColors.Accent4Brush;
                    }

                    break;
                case 5:
                    if (camera.Orientation >= 0.0f && camera.Orientation < 180.0f)
                    {
                        camera.CameraModel.Content = "Renne noire";
                        camera.myEllipse.Fill = SurfaceColors.Accent2Brush;
                    }
                    else
                    {
                        camera.CameraModel.Content = "Renne blanche";
                        camera.myEllipse.Fill = SurfaceColors.Accent4Brush;
                    }
                    break;
                case 6:
                    if (camera.Orientation >= 0.0f && camera.Orientation < 180.0f)
                    {
                        camera.CameraModel.Content = "Pion noir";
                        camera.myEllipse.Fill = SurfaceColors.Accent2Brush;
                    }
                    else
                    {
                        camera.CameraModel.Content = "Pion blanche";
                        camera.myEllipse.Fill = SurfaceColors.Accent4Brush;
                    }
                    
                    break;
                default:
                    camera.CameraModel.Content = "rien";
                    camera.myEllipse.Fill = SurfaceColors.Accent2Brush;
                    break;
            }
        }

        private void OnVisualizationAddedd(object sender, TagVisualizerEventArgs e)
        {
            //object gestion fonction to change the game checks <=> dame
            CameraVisualization camera = (CameraVisualization)e.TagVisualization;
            Rectangle rect = (Rectangle)FindName("rectangleDroite");
            Rectangle rect1 = (Rectangle)FindName("rectangleBas");
            Microsoft.Surface.Presentation.Controls.TagVisualizer tagV = (Microsoft.Surface.Presentation.Controls.TagVisualizer)FindName("MyTagVisualizer");

            switch (camera.VisualizedTag.Value)
            {
                default:
                    if (echec) {
                        rect.Visibility = Visibility.Hidden;
                        rect1.Visibility = Visibility.Hidden;
                        tagV.Width = 900;
                        tagV.Height = 900;
                        echec = !echec;
                    } 
                    else {
                        rect.Visibility = Visibility.Visible;
                        rect1.Visibility = Visibility.Visible;
                        tagV.Width = 720;
                        tagV.Height = 720;
                        echec = !echec;
                    }

                    break;
            }
        }
    }
}