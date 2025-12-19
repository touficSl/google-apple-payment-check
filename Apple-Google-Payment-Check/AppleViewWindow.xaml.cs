using caspervpn_test.helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for AppleViewWindow.xaml
    /// </summary>
    public partial class AppleViewWindow : Window
    {
        private dynamic resultdata;
        private Commun commun = new Commun();
        public AppleViewWindow(dynamic resultdata)
        {
            Console.Write("resultdata environment >>>>>>>>>>>> " + resultdata.environment + "\n");
            //Console.Write("resultdata receipt_creation_date >>>>>>>>>>>> " + resultdata.receipt_creation_date + "\n");
            this.resultdata = resultdata;
            InitializeComponent();

            envTxt.Text = resultdata.environment;
            ReceiptCreationDateTxt.Text = resultdata.receipt.receipt_type;

            expires_date_pstTxt.Text = resultdata.in_app[0].expires_date_pst;
            product_idTxt.Text = resultdata.in_app[0].product_id;
            auto_renew_statusTxt.Text = resultdata.pending_renewal_info[0].auto_renew_status;
            is_in_billing_retry_periodTxt.Text = resultdata.pending_renewal_info[0].is_in_billing_retry_period;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private void OpenFileLocation_Click(object sender, RoutedEventArgs e)
        {
            string filepath = @commun.folder_path + "\\appleResults.txt";
            if (File.Exists(filepath))
            {
                Process.Start("explorer.exe", filepath);
            }

        }
    }
}
