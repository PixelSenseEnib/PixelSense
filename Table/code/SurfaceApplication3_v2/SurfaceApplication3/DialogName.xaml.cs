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

namespace SurfaceApplication3
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class DialogName : Window
    {
                public DialogName()
                {
                        InitializeComponent();
                        lblQuestion.Content = "Entrez votre nom";
                        txtAnswer.Text = "Sebastien Kubicki";
                        lblErreur.Content = "";
                }

                private void btnDialogOk_Click(object sender, RoutedEventArgs e)
                {
                        int n;
                        bool isNumeric = int.TryParse(txtAnswer.Text, out n);
                        if (isNumeric)
                            lblErreur.Content = "Tu t'appelles " + txtAnswer.Text + " ?";
                        else
                            this.DialogResult = true;
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
        }
    }

