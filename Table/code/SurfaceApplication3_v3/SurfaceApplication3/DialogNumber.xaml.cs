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
using System.Windows.Shapes;
using System.ComponentModel;

namespace SurfaceApplication3
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class DialogNumber : Window
    {
                public DialogNumber()
                {
                        InitializeComponent();
                        lblQuestion.Content = "Entrez un nombre entre 1 et 42 :";
                        txtAnswer.Text = "42";
                        lblErreur.Content = "";
                        this.WindowStyle = WindowStyle.None;
                    
                        
                }

                private void btnDialogOk_Click(object sender, RoutedEventArgs e)
                {
                        int n;
                        bool isNumeric = int.TryParse(txtAnswer.Text, out n);
                        if (isNumeric)
                        {
                            if (n > 42 || n < 1)
                                lblErreur.Content = "Un nombre ENTRE 1 et 42...";
                            else
                                this.DialogResult = true;
                        }
                        else
                            lblErreur.Content = "Un nombre est demandé...";
                        
                }

                private void Window_ContentRendered(object sender, EventArgs e)
                {
                        txtAnswer.SelectAll();
                        txtAnswer.Focus();
                }

                public string Answer
                {
                        get { return txtAnswer.Text; }
                }

                void DataWindow_Closing(object sender, CancelEventArgs e)
                {
                    Application.Current.Shutdown();
                }
        }
    }

