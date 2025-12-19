using Apple_Google_Payment_Check.helper;
using caspervpn_test.helper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
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
    /// Interaction logic for CheckCodeWindow.xaml
    /// </summary>
    public partial class CheckCodeWindow : Window
    {

        private string TARGETURL = "";
        private Commun commun = new Commun(); 

        public CheckCodeWindow()
        {
            InitializeComponent();
            commun.GoogleAnalyticsTrack("Check Code Window Open", "Check Code Window Open", "Check Code Window Open", "Check Code Window Open", "1");
        }

        public void CheckKeyClick(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = "Loading...";
            this.Cursor = Cursors.Wait;

            if(KeyTxt.Text.Trim() == "")
            {
                ErrorMessage.Text = "*Field is required!";
                ErrorMessage.Visibility = Visibility;
                return;
            }

            KeyCheck keycheck = new KeyCheck();
            {
                keycheck.key = KeyTxt.Text;
                keycheck.taken = false;
            }

            
            try
            {
                TARGETURL = "https://sheetsu.com/apis/v1.0su/cf8068041f54/search?Key=" + KeyTxt.Text + "\n";

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



            //var byteArray = Encoding.ASCII.GetBytes("e6TVmEZnU4CZzaVxzoD6:Svpd8LGfz2qb2zRRGsXasEA3P2Rm553W9wwfnZ2s");
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            Commun commun = new Commun();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{commun.username}:{commun.password}")));

            HttpResponseMessage response = await client.GetAsync(TARGETURL);
            HttpContent content = response.Content;

            // ... Check Status Code                                
            Console.WriteLine("Response StatusCode: " + (int)response.StatusCode + "\n");

            // ... Read the string.
            string result = await content.ReadAsStringAsync();

            Console.WriteLine("result: >>>>>>> " + result + "\n");

            if (result != "" && result != null)
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
                    }else
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
                            MainWindow mainwindow = new MainWindow();
                            mainwindow.Show();
                            this.Close();
                        });
                    }
                    Dispatcher.Invoke(() => this.Cursor = Cursors.Arrow);
                }
                catch (Exception ex)
                {
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
