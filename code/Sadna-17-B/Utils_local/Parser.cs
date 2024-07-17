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

    public class Parser
    {

        // ------------------- primitive ---------------------------------------------------------------------------------

        public static bool is_none(string s)
        {
            return s == "";
        }

        public static int parse_int(string s)
        {
            if (is_none(s))
                return -1;

            int parsed = int.Parse(s);
            return parsed;
        }

        public static bool parse_boolean(string s)
        {
            if (is_none(s))
                return false;

            bool parsed = bool.Parse(s);
            return parsed;
        }

        public static double parse_double(string s, int range = -1)
        {
            if (is_none(s) | range != -1)
            {
                if (range == 1)
                    return 1000000;

                if (range == 0)
                    return 0;

                return -1;
            }


            double parsed = double.Parse(s, CultureInfo.InvariantCulture);
            return parsed;
        }

        public static char parse_char(string s)
        {

            if (is_none(s))
                return '0';

            if (s.Length != 1)
                throw new ArgumentException("Input string must be exactly one character long.");
            char parsed = char.Parse(s);
            return parsed;
        }

        public static string parse_string(string s)
        { 
            if (is_none(s))
                return "";
            
            string parsed = s;
            return parsed;
        }

        public static DateTime parse_date(string s)
        {
        if (is_none(s))
            return DateTime.Now;

        DateTime parsed = DateTime.ParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return parsed;
        }



        // ------------------- non - primitive ---------------------------------------------------------------------------------

        public static T parse_the_string<T>(string s)
        {
            try
            {
                if (typeof(T) == typeof(int))
                    return (T)(object)parse_int(s);

                else if (typeof(T) == typeof(double))
                    return (T)(object)parse_double(s);

                else if (typeof(T) == typeof(string))
                    return (T)(object)parse_string(s);

                else if (typeof(T) == typeof(bool))
                    return (T)(object)parse_boolean(s);

                else
                    throw new ArgumentException($"Unsupported return type: {typeof(T)}");
            }
            catch (FormatException)
            {
                throw new ArgumentException($"Invalid value for return type {typeof(T)}: {s}");
            }
        }
        
        public static T[] parse_array<T>(string s)
        {
            string[] array = s.Split('|');
            T[] parsed = new T[array.Length];

            for (int i = 0; i < array.Length; i++)
                parsed[i] = parse_the_string<T>(array[i]);

            return parsed;
        }

        public static T[] parse_array<T>(string[] array)
        {
            T[] parsed = new T[array.Length];

            for (int i = 0; i < array.Length; i++)
                parsed[i] = parse_the_string<T>(array[i]);

            return parsed;
        }


    }
}