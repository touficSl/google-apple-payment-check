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
    /// Interaction logic for GoogleFieldsWindow.xaml
    /// </summary>
    public partial class GoogleFieldsWindow : Window
    {

        private Commun commun = new Commun();
        public GoogleFieldsWindow()
        {
            InitializeComponent();
            commun.GoogleAnalyticsTrack("Google Fields Window Open", "Google Fields Window Open", "Google Fields Window Open", "Google Fields Window Open", "1");
        }

        private void (object sender, RoutedEventArgs e)
        {

        }
    }
}
