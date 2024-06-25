using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Globalization;
using Sadna_17_B.DomainLayer.StoreDom;

namespace Sadna_17_B.Utils
{


    /*
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     */








    public class Parser
    {

        // ------------------- primitive ---------------------------------------------------------------------------------

        public static int parse_int(string s)
        {
            int parsed = int.Parse(s);
            return parsed;
        }

        public static bool parse_boolean(string s)
        {
            bool parsed = bool.Parse(s);
            return parsed;
        }

        public static double parse_double(string s)
        {
            double parsed = double.Parse(s, CultureInfo.InvariantCulture);
            return parsed;
        }

        public static char parse_char(string s)
        {
            if (s.Length != 1)
                throw new ArgumentException("Input string must be exactly one character long.");
            char parsed = char.Parse(s);
            return parsed;
        }

        public static string parse_string(string s)
        {
            string parsed = s;
            return parsed;
        }

        public static DateTime parse_date(string s)
        {
            DateTime parsed = DateTime.ParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return parsed;
        }



        // ------------------- non - primitive ---------------------------------------------------------------------------------

        public static List<int> parse_int_list(string[] a)
        {
            List<int> parsed = new List<int>();

            foreach (string s in a)
            {
                if (s == "]")
                    break;

                parsed.Add(parse_int(s));
            }

            return parsed;
        }

        public static List<string> parse_string_list(string[] a)
        {
            List<string> parsed = new List<string>();

            foreach (string s in a)
            {
                if (s == "]")
                    break;

                parsed.Add(s);
            }

            return parsed;
        }


    }
}