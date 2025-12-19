using CasperVPN.Classes;
using caspervpn_test.helper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Apple_Google_Payment_Check
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static String app_version = "1.0";
        private String TARGETURL = "";
        private String type = "";
        private Commun commun = new Commun();

        public MainWindow()
        {
            InitializeComponent();
            commun.GoogleAnalyticsTrack("Main Window Open", "Main Window Open", "Main Window Open", "Main Window Open", "1");
        }


        public void GoogleClick(object sender, RoutedEventArgs e)
        {
            // if new version is ready a popup message should appears 
           type = "Google";
            CheckNewVersion();
        }

        public void AppleClick(object sender, RoutedEventArgs e)
        {
            type = "Apple";
            CheckNewVersion(); 
        }

        private bool CheckList(string type)
        {
            if (!CheckConnection())
                return false;

            // check if the file contains the key data saved
            // if yes call the API to check if these data are still true
            //// if yes let user open the app directly
            //// if no - delete the file data and let him re-enter the key
            // if no - let him enters the key (checkCode)

            if (type == "Google")
            {
                GoogleFieldsWindow googleFieldsWindow = new GoogleFieldsWindow();
                googleFieldsWindow.Show();
            }
            else if (type == "Apple")
            {
                AppleFieldsWindow appleFieldsWindow = new AppleFieldsWindow();
                appleFieldsWindow.Show();

            }
            return true;

        }

        private bool CheckConnection()
        {
            bool con = NetworkInterface.GetIsNetworkAvailable();
            if (con == false)
            {
                MessageBox.Show(Messages.globaldic["no_Internet_con"]);
                return false;
            }
            return true;
        }

        private void CheckNewVersion()
        {
            try
            {
                ErrorMessage.Visibility = Visibility;
                ErrorMessage.Text = "Loading...";
                this.Cursor = Cursors.Wait;

                TARGETURL = "https://sheetsu.com/apis/v1.0su/cf8068041f54/search?Key=version";

                Task t = new Task(HTTP_GET);
                t.Start();
            }
            catch (WebException ex)
            {
                Console.Write(">>>>>>>>>>>>>>>>>>> ERROR  " + ex.ToString() + "  <<<<<<<<<<<<<<<<<<" + " \n");

                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                    Dispatcher.Invoke(() =>
                    {
                        this.ErrorMessage.Visibility = Visibility;
                        this.ErrorMessage.Text = "Error, Invalid Key";
                    });
                else
                    Dispatcher.Invoke(() =>
                    {
                        this.ErrorMessage.Visibility = Visibility;
                        this.ErrorMessage.Text = "Error, Try Again";
                    });
            }
            catch (Exception ex)
            {
                Console.Write(">>>>>>>>>>>>>>>>>>> ERROR  " + ex.ToString() + "  <<<<<<<<<<<<<<<<<<" + " \n");
                Dispatcher.Invoke(() =>
                {
                    this.ErrorMessage.Visibility = Visibility;
                    this.ErrorMessage.Text = "Error, Try Again";
                });
            }
        }

        private async void HTTP_GET()
        {

            /*
            HttpClientHandler handler = new HttpClientHandler()
            {
                Proxy = new WebProxy("http://127.0.0.1:8888"),
                UseProxy = true,
            };
            */

            Console.WriteLine("GET: + " + TARGETURL);

            // ... Use HttpClient.            
            HttpClient client = new HttpClient(); // handler

            Commun commun = new Commun();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{commun.username}:{commun.password}")));

            HttpResponseMessage response = await client.GetAsync(TARGETURL);
            HttpContent content = response.Content;

            // ... Check Status Code                                
            Console.WriteLine("Response StatusCode: " + (int)response.StatusCode + "\n");

            // ... Read the string.
            string result = await content.ReadAsStringAsync();

            Console.WriteLine("result: >>>>>>> " + result + "\n");

            if (result != "")
            {
                string first = result.Substring(result.Length - 1);
                if (first == "]")
                    result = result.Substring(1, result.Length - 2);

                try
                {
                    dynamic data = JObject.Parse(result);

                    if (data.error != null)
                    {
                        Dispatcher.Invoke(() => this.ErrorMessage.Text = data.error);
                        Dispatcher.Invoke(() => this.ErrorMessage.Visibility = Visibility);
                        Dispatcher.Invoke(() => this.Cursor = Cursors.Arrow);
                        return;
                    }
                    else
                    if (data == null)
                    {
                        Console.WriteLine(">>>>>>>>>> error \n");
                        Dispatcher.Invoke(() => this.ErrorMessage.Text = "Error, Try Again!");
                        Dispatcher.Invoke(() => this.ErrorMessage.Visibility = Visibility);
                        Dispatcher.Invoke(() => this.Cursor = Cursors.Arrow);
                        return;
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            Console.WriteLine("Response data: " + data + "\n");
                            if (Convert.ToDouble(data.Taken) > Convert.ToDouble(app_version))
                            {
                                NewVersion newversion = new NewVersion(data.Url.ToString());
                                newversion.Show();
                                this.Close();
                            }
                            else
                                CheckList(type);
                        });
                        Dispatcher.Invoke(() => this.ErrorMessage.Visibility = System.Windows.Visibility.Hidden);
                        Dispatcher.Invoke(() => this.Cursor = Cursors.Arrow);
                    }
                    Dispatcher.Invoke(() => this.Cursor = Cursors.Arrow);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error " + ex.ToString() + "\n");
                    Dispatcher.Invoke(() => this.ErrorMessage.Text = "Error, Try Again!");
                    Dispatcher.Invoke(() => this.ErrorMessage.Visibility = Visibility);
                    Dispatcher.Invoke(() => this.Cursor = Cursors.Arrow);
                }

            }
            else
            {
                Console.WriteLine(">>>>>>>>>> error \n");
                Dispatcher.Invoke(() => this.ErrorMessage.Text = "Error, Try Again!");
                Dispatcher.Invoke(() => this.ErrorMessage.Visibility = Visibility);
            }
        }
    }
}
