using Garlic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace caspervpn_test.helper
{
    public class Commun
    {
        public String username = "e6TVmEZnU4CZzaVxzoD6";
        public String password = "Svpd8LGfz2qb2zRRGsXasEA3P2Rm553W9wwfnZ2s";
        public String folder_path = @System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\appleGoogleCheck";

        public string receipthelpURL = "https://docs.google.com/document/d/1L2cEN-lDuZxKNc6KY-iEGubCgNBAQG4QCW6rsqYWnCw/edit#heading=h.te0kw4qlo3ww";
        public string passwordhelpURL = "https://docs.google.com/document/d/1L2cEN-lDuZxKNc6KY-iEGubCgNBAQG4QCW6rsqYWnCw/edit#heading=h.dzj1xzisbb9f";


        public void GoogleAnalyticsTrack(string page, string category, string action, string label, string value)
        {
            // https://github.com/dustyburwell/garlic
            try
            {
                var session = new AnalyticsSession("Google Apple Check", "UA-122716482-1");
                var analytics = session.CreatePageViewRequest(page, page);
                analytics.SendEvent(category, action, label, value);

            }
            catch(Exception ex)
            {

            }
        }

        public string JSON_To_String(object obj)
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());
            ser.WriteObject(stream, obj);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            return sr.ReadToEnd();

        }

        public object String_To_JSON(string str, Type type) //convert json string to object of type class
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            DataContractJsonSerializer ser = new DataContractJsonSerializer(type);
            return ser.ReadObject(stream);
        }


    }
}
