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

namespace WpfApplication2
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
                public MainWindow()
                {
                        InitializeComponent();
                }

                private void btnEnterName_Click(object sender, RoutedEventArgs e)
                {
                        DialogName inputDialog = new DialogName();
                        if(inputDialog.ShowDialog() == true)
                                lblName.Text = inputDialog.Answer;
                }

                private void btnEnterCards_Click(object sender, RoutedEventArgs e)
                {
                    DialogNumber inputDialog = new DialogNumber();
                    if (inputDialog.ShowDialog() == true)
                        lblName.Text = inputDialog.Answer;
                }
    }
}
