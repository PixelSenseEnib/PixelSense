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
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace SurfaceApplication3
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {

        public List<int[]> BlackList = new List<int[]>();
        public Rectangle[,] cells;
        public Dictionary<long, int[]> tags = new Dictionary<long , int[]>();
        public TextBlock text;
        public String path;
        public String card;
        public String name;
        public Dictionary<int,int> series = new Dictionary<int,int>();
        public int currentSerie;
        public int nbSerie14;
        public String record;
        public List<String> records = new List<String>();
        public Dictionary<long, int> tagsToCard = new Dictionary<long, int>();
        public int nbCards;

        public void createTagsToCard()
        {
            tagsToCard.Add(2, 1);
            tagsToCard.Add(40, 2);
            tagsToCard.Add(11, 3);
            tagsToCard.Add(7, 4);
            tagsToCard.Add(5, 5);
            tagsToCard.Add(23, 6);
            tagsToCard.Add(31, 7);
            tagsToCard.Add(41, 8);
            tagsToCard.Add(9, 9);
            tagsToCard.Add(6, 10);
            tagsToCard.Add(36, 11);
            tagsToCard.Add(27, 12);
            tagsToCard.Add(22, 13);
            tagsToCard.Add(21, 14);
            tagsToCard.Add(4, 15);
            tagsToCard.Add(1, 16);
            tagsToCard.Add(13, 17);
            tagsToCard.Add(37, 18);
            tagsToCard.Add(34, 19);
            tagsToCard.Add(16, 20);
            tagsToCard.Add(12, 21);
            tagsToCard.Add(28, 22);
            tagsToCard.Add(19, 23);
            tagsToCard.Add(17, 24);
            tagsToCard.Add(10, 25);
            tagsToCard.Add(20, 26);
            tagsToCard.Add(14, 27);
            tagsToCard.Add(38, 28);
            tagsToCard.Add(25, 29);
            tagsToCard.Add(8, 30);
            tagsToCard.Add(32, 31);
            tagsToCard.Add(39, 32);
            tagsToCard.Add(29, 33);
            tagsToCard.Add(18, 34);
            tagsToCard.Add(15, 35);
            tagsToCard.Add(3, 36);
            tagsToCard.Add(35, 37);
            tagsToCard.Add(26, 38);
            tagsToCard.Add(30, 39);
            tagsToCard.Add(33, 40);
            tagsToCard.Add(42, 41);
            tagsToCard.Add(24, 42);
        }


        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            DialogName inputDialog = new DialogName();
            if (inputDialog.ShowDialog() == true)
            {
                name = inputDialog.Answer;
            }

            DialogNumber inputDialog2 = new DialogNumber();
            if (inputDialog2.ShowDialog() == true)
            {

                card = inputDialog2.Answer;
            }
            currentSerie = 1;

            int.TryParse(card, out nbCards);
            int lastSerie = nbCards % 14;
            nbSerie14 = nbCards / 14;
            //Console.WriteLine("nb carte lastserie :" +lastSerie+" nb de series de 14 :" + nbSerie14);
            for(int i=1; i<nbSerie14+1; i++)
                series.Add(i,14);
            if(lastSerie !=0)
                series.Add(nbSerie14 + 1, lastSerie);
            
            for(int i=1; i<series.Count+1; i++)
            {
                Console.WriteLine(series[i]);
            }

            InitializeComponent();

            InitializeTags();
            createTagsToCard();
            CreateGrid(9, 5, series[currentSerie]);
            
            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
        }


        public void CreateGrid(int nbCol, int nbRow, int nbCase)
        {
            cells = new Rectangle[nbCol, nbRow];


            // Create the grid
            //Grid grid = ((Grid)FindName("grid1"));
            //grid = new Grid();
            //grid.HorizontalAlignment = HorizontalAlignment.Left;
            //grid.VerticalAlignment = VerticalAlignment.Top;
            //grid.ShowGridLines = false;
            //grid.Background = new SolidColorBrush(Colors.WhiteSmoke);

            ////Create the columns
            //for (int i = 0; i < nbCol; i++)
            //{
            //    ColumnDefinition col = new ColumnDefinition();
            //    col.Width = new GridLength(213.3);
            //    grid.ColumnDefinitions.Add(col);
            //}


            ////Create the rows
            //for (int j = 0; j < nbRow; j++)
            //{
            //    RowDefinition row = new RowDefinition();
            //    row.Height = new GridLength(216);
            //    grid.RowDefinitions.Add(row);
            //}

            int a = nbCol * nbRow - nbCase;
            int b = 0;
            for (int k = 0; k < a; k++)
            {
                if (!((nbCol - 1 - k == 8) && (nbRow - 1 - b == 4)))
                {
                    if (!((nbCol - 1 - k == 7) && (nbRow - 1 - b == 4)))
                    {
                        if (!((nbCol - 1 - k == 6) && (nbRow - 1 - b == 4)))
                        {
                            //Console.WriteLine("test : " + (nbCol - 1 - k) + ", " + (nbRow - 1 - b));
                            BlackList.Add(new int[] { nbCol - 1 - k, nbRow - 1 - b });
                            if (k / 8 == 1)
                            {
                                k -= 9;
                                b++;
                                a -= 9;
                            }
                        }
                       //Console.WriteLine("k : " + k + ", a :" + a + ", b :" + b);
                    }
                }
            }


            //Create the rectangle and the button
            for (int i = 0; i < nbCol; i++)
                for (int j = 0; j < nbRow; j++)
                {
                    if (i == nbCol- 1 && j == nbRow- 1)
                    {
                        Button button = new Button();
                        button.Content = "Valider (" + currentSerie + "/" + (series.Count) + ")";
                        button.FontSize = 35;
                        button.HorizontalAlignment = HorizontalAlignment.Stretch;
                        button.VerticalAlignment = VerticalAlignment.Top;
                        button.Height = 72;
                        button.Click += OnButtonClick;
                        button.TouchDown += OnButtonTouchDown;
                        Grid.SetRow(button, j);
                        Grid.SetColumn(button, i);
                        ((Grid)FindName("grid1")).Children.Add(button);

                        Button button2 = new Button();
                        button2.Content = "Ouvrir";
                        button2.FontSize = 50;
                        button2.HorizontalAlignment = HorizontalAlignment.Stretch;
                        button2.VerticalAlignment = VerticalAlignment.Center;
                        button2.Height = 72;
                        button2.Click += OnButtonClickOpen;
                        button2.TouchDown += OnButtonTouchDownOpen;
                        Grid.SetRow(button2, j);
                        Grid.SetColumn(button2, i);
                        ((Grid)FindName("grid1")).Children.Add(button2);

                        Button button3 = new Button();
                        button3.Content = "Fermer";
                        button3.FontSize = 50;
                        button3.HorizontalAlignment = HorizontalAlignment.Stretch;
                        button3.VerticalAlignment = VerticalAlignment.Bottom;
                        button3.Height = 72;
                        button3.Click += OnButtonClickClose;
                        button3.TouchDown += OnButtonTouchDownClose;
                        Grid.SetRow(button3, j);
                        Grid.SetColumn(button3, i);
                        ((Grid)FindName("grid1")).Children.Add(button3);
                    }
                    else if (i == nbCol - 2 && j == nbRow - 1)
                    {
                        Rectangle rect = new Rectangle();
                        rect.Fill = new SolidColorBrush(Colors.Black);
                        rect.Stroke = new SolidColorBrush(Colors.Black);
                        Grid.SetRow(rect, j);
                        Grid.SetColumn(rect, i);
                        ((Grid)FindName("grid1")).Children.Add(rect);
                        text = new TextBlock();
                        text.Foreground = new SolidColorBrush(Colors.White);
                        Grid.SetRow(text, j);
                        Grid.SetColumn(text, i);
                        ((Grid)FindName("grid1")).Children.Add(text);
                        cells[i, j] = rect;
                    }
                    else if (i == nbCol - 3 && j == nbRow - 1)
                    {
                        Rectangle rect = new Rectangle();
                        rect.Fill = new SolidColorBrush(Colors.Black);
                        rect.Stroke = new SolidColorBrush(Colors.Black);
                        Grid.SetRow(rect, j);
                        Grid.SetColumn(rect, i);
                        ((Grid)FindName("grid1")).Children.Add(rect);
                        cells[i, j] = rect;
                        //Button button = new Button();
                        //button.Content = "Changer Nom";
                        //button.FontSize = 30;
                        //button.HorizontalAlignment = HorizontalAlignment.Stretch;
                        //button.VerticalAlignment = VerticalAlignment.Top;
                        //button.Height = 108;
                        //button.Click += OnButtonClickName;
                        //button.TouchDown += OnButtonTouchDownName;
                        //Grid.SetRow(button, j);
                        //Grid.SetColumn(button, i);
                        //((Grid)FindName("grid1")).Children.Add(button);

                        //Button button2 = new Button();
                        //button2.Content = "Changer Cartes";
                        //button2.FontSize = 28;
                        //button2.HorizontalAlignment = HorizontalAlignment.Stretch;
                        //button2.VerticalAlignment = VerticalAlignment.Bottom;
                        //button2.Height = 108;
                        //button2.Click += OnButtonClickCard;
                        //button2.TouchDown += OnButtonTouchDownCard;
                        //Grid.SetRow(button2, j);
                        //Grid.SetColumn(button2, i);
                        //((Grid)FindName("grid1")).Children.Add(button2);
                    }
                    else
                    {
                        Rectangle rect = new Rectangle();
                        rect.Fill = new SolidColorBrush(Colors.DarkSlateGray);
                        rect.Stroke = new SolidColorBrush(Colors.Black);
                        Grid.SetRow(rect, j);
                        Grid.SetColumn(rect, i);
                        ((Grid)FindName("grid1")).Children.Add(rect);
                        cells[i, j] = rect;
                    }
                }
            for (int i = 0; i < BlackList.Count; i++)
            {
                Console.WriteLine("BlackList : " + BlackList.ElementAt(i)[0] + "," + BlackList.ElementAt(i)[1]);
                Rectangle rect = new Rectangle();
                rect.Fill = new SolidColorBrush(Colors.Black);
                rect.Stroke = new SolidColorBrush(Colors.Black);
                Grid.SetRow(rect, BlackList.ElementAt(i)[1]);
                Grid.SetColumn(rect, BlackList.ElementAt(i)[0]);
                ((Grid)FindName("grid1")).Children.Add(rect);
                cells[BlackList.ElementAt(i)[0], BlackList.ElementAt(i)[1]] = rect;
            }

          
            //Add grid to window
            //((SurfaceWindow)FindName("surf")).Content = grid;

        }



        private void InitializeTags()
        {
            TagVisualizer myTagVisualizer2 = new TagVisualizer();
            //myTagVisualizer.VerticalAlignment = VerticalAlignment.Stretch;
            //myTagVisualizer.HorizontalAlignment = HorizontalAlignment.Stretch;
            //myTagVisualizer.Background = new SolidColorBrush(Colors.Transparent);
            //myTagVisualizer2.VisualizationAdded += OnVisualizationAdded;
            //myTagVisualizer.AddHandler(myTagVisualizer.VisualizationAdded, OnVisualizationAdded(sender, e), true);
            //myTagVisualizer.AddHandler(MyTagVisualizer.VisualizationAdded, new TagVisualizerEventHandler(OnVisualizationAdded()), true);
            //myTagVisualizer.FindResource("VisualizationAdded");
            //myTagVisualizer.SetResourceReference(,"VisualisationAdded");


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
                //tags[k] = tagDef;
                // Add the definition to the collection.
                MyTagVisualizer.Definitions.Add(tagDef);
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

        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            TagVisualization1 camera = (TagVisualization1)e.TagVisualization;
            Point centreTag = camera.Center;
            int index = 0;
            int[] tab= new int[2];
            tab[0]= (int)Math.Floor(centreTag.X / 213.3);
            tab[1]= (int)Math.Floor(centreTag.Y / 216);

            if (!(tab[0] == 8 && tab[1] == 4))
            {
                if (!(tab[0] == 7 && tab[1] == 4))
                {
                    if (!(tab[0] == 6 && tab[1] == 4))
                    {
                        //Console.WriteLine("i = "+tab[0]+ "j= "+tab[1]);
                        for (int i = 0; i < BlackList.Count; i++)
                        {
                            //Console.WriteLine("BlackList : " + BlackList.ElementAt(i)[0] + "," + BlackList.ElementAt(i)[1]);
                            if (((BlackList.ElementAt(i)[0] != tab[0]) && (BlackList.ElementAt(i)[1] != tab[1])) || ((BlackList.ElementAt(i)[0] == tab[0]) && (BlackList.ElementAt(i)[1] != tab[1])) ||
                                ((BlackList.ElementAt(i)[0] != tab[0]) && (BlackList.ElementAt(i)[1] == tab[1])))
                            {


                                //Console.WriteLine("BlackList : " + BlackList.ElementAt(i)[0] + "," + BlackList.ElementAt(i)[1]+  " et tab :" + tab[0]+ "," +  tab[1]);
                                index++;

                            }
                        }

                        //Console.WriteLine("index : " + index);

                        if (index == BlackList.Count)
                        {

                            cells[tab[0], tab[1]].Fill = new SolidColorBrush(Colors.Green);

                            if (!tags.ContainsKey(camera.VisualizedTag.Value))
                            {
                                tags.Add(camera.VisualizedTag.Value, tab);
                            }
                            else
                                tags[camera.VisualizedTag.Value] = tab;
                        }
                    }
                }
            }
        }

        private void OnVisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
            TagVisualization1 camera = (TagVisualization1)e.TagVisualization;
            Point centreTag = camera.Center;
            int index = 0;
            int[] tab = new int[2];
            tab[0] = (int)Math.Floor(centreTag.X / 213.3);
            tab[1] = (int)Math.Floor(centreTag.Y / 216);
            //Console.WriteLine("i = " + i + "j= " + j);
            if (!(tab[0] == 8 && tab[1] == 4))
            {
                if (!(tab[0] == 7 && tab[1] == 4))
                {
                    if (!(tab[0] == 6 && tab[1] == 4))
                    {
                        for (int i = 0; i < BlackList.Count; i++)
                        {
                            if (((BlackList.ElementAt(i)[0] != tab[0]) && (BlackList.ElementAt(i)[1] != tab[1])) || ((BlackList.ElementAt(i)[0] == tab[0]) && (BlackList.ElementAt(i)[1] != tab[1])) ||
                                 ((BlackList.ElementAt(i)[0] != tab[0]) && (BlackList.ElementAt(i)[1] == tab[1])))
                            {
                                //Console.WriteLine("BlackList : " + BlackList.ElementAt(i)[0] + "," + BlackList.ElementAt(i)[1] + " et tab :" + tab[0] + "," + tab[1]);
                                index++;
                            }
                        }

                        if (index == BlackList.Count)
                        {
                            tags.Remove(camera.VisualizedTag.Value);
                            cells[tab[0], tab[1]].Fill = new SolidColorBrush(Colors.DarkSlateGray);
                        }
                    }
                }
            }

        }

        private void OnVisualizationMoved(object sender, TagVisualizerEventArgs e)
        {
            TagVisualization1 camera = (TagVisualization1)e.TagVisualization;
            Point centreTag = camera.Center;
            int[] tab = new int[2];
            tab[0] = (int)Math.Floor(centreTag.X / 213.3);
            tab[1] = (int)Math.Floor(centreTag.Y / 216);
            int index = 0;


            for (int i = 0; i < BlackList.Count; i++)
            {
                if (((BlackList.ElementAt(i)[0] != tab[0]) && (BlackList.ElementAt(i)[1] != tab[1])) || ((BlackList.ElementAt(i)[0] == tab[0]) && (BlackList.ElementAt(i)[1] != tab[1])) ||
                     ((BlackList.ElementAt(i)[0] != tab[0]) && (BlackList.ElementAt(i)[1] == tab[1])))
                {
                    //Console.WriteLine("BlackList : " + BlackList.ElementAt(i)[0] + "," + BlackList.ElementAt(i)[1] + " et tab :" + tab[0] + "," + tab[1]);
                    index++;
                }
            }


            if (index == BlackList.Count)
            {
                Console.WriteLine("Tab :" + tab[0] + "," + tab[1]);
                if (tags.ContainsKey(camera.VisualizedTag.Value))
                {
                    Console.WriteLine("Tags before : " + tags[camera.VisualizedTag.Value][0] + "," + tags[camera.VisualizedTag.Value][1]);
                    if (tags[camera.VisualizedTag.Value][0] != tab[0] || tags[camera.VisualizedTag.Value][1] != tab[1])
                    {
                        cells[tab[0], tab[1]].Fill = new SolidColorBrush(Colors.Green);

                        cells[tags[camera.VisualizedTag.Value][0], tags[camera.VisualizedTag.Value][1]].Fill = new SolidColorBrush(Colors.DarkSlateGray);

                        tags[camera.VisualizedTag.Value][0] = tab[0];
                        tags[camera.VisualizedTag.Value][1] = tab[1];
                        Console.WriteLine("Tags after : " + tags[camera.VisualizedTag.Value][0] + "," + tags[camera.VisualizedTag.Value][1]);
                    }
                }
                else {
                    //Console.WriteLine("Compteur :"+compteur);
                    //if (!compteur)
                    //{
                    //    Console.WriteLine("Test2 ");
                    //    cells[tab[0], tab[1]].Fill = new SolidColorBrush(Colors.Green);
                    //    compteur = true;
                    //}
                    //else
                    //{
                    //    compteur = false;
                    //}
                }

            }
            else
            {
                Console.WriteLine("Tab :" + tab[0] + "," + tab[1]);
                if (tags.ContainsKey(camera.VisualizedTag.Value))
                {

                    if (tags[camera.VisualizedTag.Value][0] != tab[0] || tags[camera.VisualizedTag.Value][1] != tab[1])
                    {

                        cells[tags[camera.VisualizedTag.Value][0], tags[camera.VisualizedTag.Value][1]].Fill = new SolidColorBrush(Colors.DarkSlateGray);
                        tags.Remove(camera.VisualizedTag.Value);
                    }

                    
                }

            }

        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            
            if (currentSerie == 1)
                {
                    record += name;
                    record += "\r\n";
                    record += DateTime.Now.ToString();
                    record += "\r\n";
                    record += "Nombre de cartes :" + card;
                    record += "\r\n";
                    record += "\r\n";
                    path = name + "_" + DateTime.Now.ToString("d_M_yyyy_hh_mm_ss") + ".txt";
                }
                foreach (KeyValuePair<long, int[]> item in tags.OrderBy(i => i.Key))
                {
                    if (ConvertCell(item.Value[0], item.Value[1]) <= nbCards)
                    {
                        if(ConvertCell(item.Value[0], item.Value[1]) < 10)
                            records.Add("Case 0" + ConvertCell(item.Value[0], item.Value[1]) + " : Carte " + tagsToCard[item.Key] +"\r\n");
                        else
                            records.Add("Case " + ConvertCell(item.Value[0], item.Value[1]) + " : Carte " + tagsToCard[item.Key] + "\r\n");
                    }
                }

            
            text.FontSize = 24;
            text.TextAlignment = TextAlignment.Justify;
            text.Text = " Résultats enregistrés dans "+path ;
            
            currentSerie++;
            if (currentSerie > series.Count())
            {
                var sort = from s in records orderby s select s;
                foreach (String c in sort)
                {
                    record += c;
                }
                FileStream stream = new FileStream("./" + path, FileMode.Append, FileAccess.Write);
                StreamWriter writer = new StreamWriter(stream);
                 using (writer)
                    {
                        writer.Write(record);
                        writer.Flush();
                        writer.Close();
                        }
                OnButtonClickOpen(sender, e);
                OnButtonClickClose(sender, e);
            }
            else
                CreateGrid(9, 5, series[currentSerie]);

        }

        private int ConvertCell(int col, int row)
        {
            return col + 1 + row * 9 + (currentSerie-1)*14;
        }

        private void OnButtonTouchDown(object sender, RoutedEventArgs e)
        {
            OnButtonClick(sender, e);
        }

        private void OnButtonClickOpen(object sender, RoutedEventArgs e)
        {
            if (path != null)
            {
                String directory;
                directory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                path = System.IO.Path.Combine(directory, path);
                Process.Start(path);
            }
            else
            {
                text.FontSize = 24;
                text.TextAlignment = TextAlignment.Justify;
                text.Text = " Il faut valider pour pouvoir ouvrir le fichier de résultats";
            }
           
        }

        private void OnButtonTouchDownOpen(object sender, RoutedEventArgs e)
        {
            OnButtonClickOpen(sender, e);
        }

        private void OnButtonClickClose(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnButtonTouchDownClose(object sender, RoutedEventArgs e)
        {
            OnButtonClickClose(sender, e);
        }

    }
}