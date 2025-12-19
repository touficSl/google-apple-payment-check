using caspervpn_test.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Apple_Google_Payment_Check
{
    /// <summary>
    /// Interaction logic for NewVersion.xaml
    /// </summary>
    public partial class NewVersion : Window
    {
        String url="";
        private Commun commun = new Commun();

        public NewVersion(String uri)
        {
            url = uri;
            InitializeComponent();
            commun.GoogleAnalyticsTrack("New Version Window Open", "New Version Window Open", "New Version Window Open", "New Version Window Open", "1");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (url != "")
                System.Diagnostics.Process.Start(url);
            else
            {
                ErrorMessage.Text = "Error, Something went wrong kindly retry.";
                ErrorMessage.Visibility = Visibility;
            }
        }
    }
}
