using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasperVPN.Classes
{
    class Messages
    {
        public static Dictionary<string, string> en = new Dictionary<string, string>();
        public static Dictionary<string, string> fr = new Dictionary<string, string>();
        public static Dictionary<string, string> ar = new Dictionary<string, string>();
        public static Dictionary<string, string> globaldic;

        public Messages()
        {
            en.Add("con_type_required", "Connection type is required!");
            en.Add("phone_nbre_required", "Phone Number is required!");
            en.Add("no_Internet_con", "No Internet connection!");
            en.Add("vpn_is_connected", "VPN Is Connected!");
            en.Add("auth_failed", "Authentication Failed!");
            en.Add("con_time_out", "Connection attempt timed out!");
            en.Add("multi_vpn", "The same vpn exist");


            ar.Add("con_type_required", "Connection type is required!");
            ar.Add("phone_nbre_required", "Phone Number is required!");
            ar.Add("no_Internet_con", "");
            ar.Add("vpn_is_connected", "");
            ar.Add("auth_failed", "");
            ar.Add("con_time_out", "");
            ar.Add("multi_vpn", "");


            fr.Add("con_type_required", "Connection type is required!");
            fr.Add("phone_nbre_required", "Phone Number is required!");
            fr.Add("no_Internet_con", "");
            fr.Add("vpn_is_connected", "");
            fr.Add("auth_failed", "");
            fr.Add("con_time_out", "");
            fr.Add("multi_vpn", "");

            globaldic = en;
        }
    }
}
