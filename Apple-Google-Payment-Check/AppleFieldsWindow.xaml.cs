using Apple_Google_Payment_Check.helper;
using caspervpn_test.helper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
    /// Interaction logic for AppleFieldsWindow.xaml
    /// </summary>
    public partial class AppleFieldsWindow : Window
    {
        private Commun commun = new Commun();
        public AppleFieldsWindow()
        {
            InitializeComponent();
            commun.GoogleAnalyticsTrack("Apple Fields Window Open", "Apple Fields Window Open", "Apple Fields Window Open", "Apple Fields Window Open", "1");
            LoadListBox();
            this.Topmost = true;
        }
         

        private void LoadListBox()
        {
            applefieldsList.Items.Clear();
            applefieldsList.Items.Add("None");
            AppleListFields applefieldlist = LoadData();
            if (applefieldlist != null)
            {
                foreach (AppleFields applefield in applefieldlist.AppleList)
                {
                    Console.WriteLine("applefield.listname: " + applefield.listname);
                    applefieldsList.Items.Add(applefield.listname);

                }
            }
        }

        private void (object sender, RoutedEventArgs e)
        {

            ErrorMessageApple.Text = "Loading...";
            this.Cursor = Cursors.Wait;
            if (checkFields() == false)
            {
                ErrorMessageApple.Text = "*Fields is required!";
                ErrorMessageApple.Visibility = Visibility;
                Dispatcher.Invoke(() => this.Cursor = Cursors.Arrow);
                return;
            } 

            try
            {
                byte[] data = null;

                string url;
                if (SandboxTxt.IsChecked == true)
                    url = "https://sandbox.itunes.apple.com/verifyReceipt";
                else
                    url = "https://buy.itunes.apple.com/verifyReceipt";

                HttpWebRequest HttpWReq = (HttpWebRequest)WebRequest.Create(url);
                ASCIIEncoding encoding = new ASCIIEncoding();

                HttpWReq.Method = "POST";
                HttpWReq.ContentType = "application/json";

                string json = "{\"receipt-data\":\"" + receiptTxt.Text + "\", \"password\": \"" + passwordTxt.Password + "\", \"exclude-old-transactions\":\"" + ExcludeTxt.IsChecked.ToString() + "\"}";

                Console.Write("json >>>>>>>>>>>> " + json + "\n");
                data = encoding.GetBytes(json);
                HttpWReq.ContentLength = data.Length;
                Stream newStream = HttpWReq.GetRequestStream();
                newStream.Write(data, 0, data.Length);
                newStream.Close();

                string response = "";

                var webResponse = (HttpWebResponse)HttpWReq.GetResponse();

                if (webResponse.StatusCode != HttpStatusCode.OK) Console.WriteLine("{0}", webResponse.Headers);
                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    response = reader.ReadToEnd();
                    reader.Close();
                }

                Console.Write("response >>>>>>>>>>>> " + response + "\n");

                if (response != "")
                {

                    string first = response.Substring(response.Length - 1);
                    if (first == "]")
                        response = response.Substring(0, response.Length - 1);

                    dynamic resultdata = JObject.Parse(response);
                    if (resultdata == null)
                    {
                        Console.WriteLine(">>>>>>>>>> error \n");
                        Dispatcher.Invoke(() => this.ErrorMessageApple.Text = "Error, Try Again!");
                        Dispatcher.Invoke(() => this.ErrorMessageApple.Visibility = Visibility);
                    }
                    else if (resultdata.status == 0)
                    {
                        SaveResult(resultdata);
                        AppleViewWindow appleViewWindow = new AppleViewWindow(resultdata);
                        appleViewWindow.Show();
                    }
                    else if (resultdata.status != 0)
                    {
                        Dispatcher.Invoke(() => this.ErrorMessageApple.Text = "Error, Apple check status " + resultdata.status);
                        Dispatcher.Invoke(() => this.ErrorMessageApple.Visibility = Visibility);
                    }
                    else 
                    {
                        Dispatcher.Invoke(() => this.ErrorMessageApple.Text = "Error, Re-check fields");
                        Dispatcher.Invoke(() => this.ErrorMessageApple.Visibility = Visibility);
                    }
                }
            } 
            catch (Exception ex1)
            {
                Console.Write("Error >>>>>>>>>>>> " + ex1.ToString() + "\n");
                Dispatcher.Invoke(() =>
                {
                    this.ErrorMessageApple.Visibility = Visibility;
                    this.ErrorMessageApple.Text = "Error, Try later.";
                });
            }
            Dispatcher.Invoke(() => this.Cursor = Cursors.Arrow);
        }

         
        private void SaveFields_Click(object sender, RoutedEventArgs e)
        {
            // if data are not save - add them to the json  
            SaveData();

        }

        private void LoadField_Click(object sender, RoutedEventArgs e)
        {
            if (applefieldsList.SelectedIndex == 0)
            {
                passwordTxt.Password = "";
                receiptTxt.Text = "";
                return;
            }

            if (applefieldsList.SelectedValue != null)
            {
                AppleListFields applefieldlist = LoadData();
                if (applefieldlist != null)
                {
                    foreach (AppleFields applefield in applefieldlist.AppleList)
                    {
                        Console.WriteLine("applefield.listname: " + applefield.listname);
                        if (applefield.listname == applefieldsList.SelectedValue.ToString())
                        {
                            passwordTxt.Password = applefield.password;
                            receiptTxt.Text = applefield.receipt;
                            fieldnametxt.Text = "";
                            return;
                        }

                    }
                    ErrorMessageApple.Text = "No Fields for this name!";
                    ErrorMessageApple.Visibility = Visibility;
                }
            }
        }

        private void OpenFileLocation_Click(object sender, RoutedEventArgs e)
        {
            string filepath = @commun.folder_path + "\\appleFields.txt";
            if (File.Exists(filepath))
            {
                Process.Start("explorer.exe", filepath);
            }
        }

        protected void SaveData()
        {
            if (checkFields() == false)
            {
                ErrorMessageApple.Text = "*Fields is required!";
                ErrorMessageApple.Visibility = Visibility;
                Dispatcher.Invoke(() => this.Cursor = Cursors.Arrow);
                return;
            }

            if(fieldnametxt.Text.Trim() == "")
            {
                ErrorMessageApple.Text = "*Field Name is required!";
                ErrorMessageApple.Visibility = Visibility;
                Dispatcher.Invoke(() => this.Cursor = Cursors.Arrow);
                return;
            }

            string path = @commun.folder_path;

            try
            {
                // Determine whether the directory exists.
                if (!Directory.Exists(path))
                {
                    // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(path);
                    Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));

                    // Delete the directory.
                    // di.Delete();
                    // Console.WriteLine("The directory was deleted successfully.");
                }

                path = path + "\\appleFields.txt";
                Console.WriteLine("path : " + path);

                AppleListFields applefieldlist = new AppleListFields();
                applefieldlist = LoadData();
                AppleFields applefields = new AppleFields() { password = passwordTxt.Password, receipt = receiptTxt.Text, exclude = ExcludeTxt.IsChecked.Value, sandbox = SandboxTxt.IsChecked.Value, listname = fieldnametxt.Text.Trim() };
                if (File.Exists(path))
                {
                    if (applefieldlist != null)
                    {
                        foreach (AppleFields applefield in applefieldlist.AppleList)
                        {
                            Console.WriteLine("applefield.listname: " + applefield.listname);
                            if (applefield.listname == fieldnametxt.Text.Trim())
                            {
                                ErrorMessageApple.Text = "*Field Name already Exist!";
                                ErrorMessageApple.Visibility = Visibility;
                                return;
                            }

                        }
                    }
                    else
                    {
                        applefieldlist = new AppleListFields();
                    }
                    // File.Delete(path);
                }
                else
                {
                    FileStream fs = File.Create(path);
                    fs.Close();
                }
                 
                applefieldlist.AppleList.Add(applefields);

                SaveDataJson.WriteToJsonFile<AppleListFields>(path, applefieldlist);

                LoadListBox();


            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private AppleListFields LoadData()
        {
            string filepath = @commun.folder_path + "\\appleFields.txt";
            if (File.Exists(filepath))
            {
                AppleListFields applefieldlist = SaveDataJson.ReadFromJsonFile<AppleListFields>(filepath);
                return applefieldlist;
                //List<Person> people = JsonSerialization.ReadFromJsonFile<List<Person>>("C:\people.txt");
            }
            return null;
        }

        private bool checkFields()
        {
            if (receiptTxt.Text.Trim() == "" || passwordTxt.Password.Trim() == "")
            { 
                return false;
            }
            return true;
        }

        private void DeleteField_Click(object sender, RoutedEventArgs e)
        {
            if (applefieldsList.SelectedIndex != 0)
            {
                if (applefieldsList.SelectedValue != null)
                {
                    string path = @commun.folder_path + "\\appleFields.txt";
                    AppleListFields applefieldlist = LoadData();
                    foreach (AppleFields applefield in applefieldlist.AppleList)
                    {
                        Console.WriteLine("applefield.listname: " + applefield.listname);
                        if (applefield.listname == applefieldsList.SelectedValue.ToString())
                        {
                            applefieldlist.AppleList.Remove(applefield);
                            SaveDataJson.WriteToJsonFile<AppleListFields>(path, applefieldlist);
                            LoadListBox();
                            return;
                        }

                    }
                    ErrorMessageApple.Text = "No Fields for this name!";
                    ErrorMessageApple.Visibility = Visibility;
                }
            }
        }

        protected void SaveResult(dynamic resultdata)
        {
            string path = @commun.folder_path;

            try
            {
                // Determine whether the directory exists.
                if (!Directory.Exists(path))
                {
                    // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(path);
                    Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));

                    // Delete the directory.
                    // di.Delete();
                    // Console.WriteLine("The directory was deleted successfully.");
                }

                path = path + "\\appleResults.txt";
                Console.WriteLine("path : " + path);
                if (!File.Exists(path))
                { 
                    FileStream fs = File.Create(path);
                    fs.Close();
                }

                SaveDataJson.WriteToJsonFile<dynamic>(path, resultdata);

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        } 

        private void passwordhelp_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(commun.passwordhelpURL);
        }

        private void receipthelp_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(commun.receipthelpURL);
        }
    }
}
