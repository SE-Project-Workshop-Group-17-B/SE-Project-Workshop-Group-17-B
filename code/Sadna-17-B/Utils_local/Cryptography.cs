using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.HtmlControls;

namespace Sadna_17_B.Utils
{
    public class Cryptography
    {
        public static string HashString(string str)
        {
            SHA256 sha256Hash = SHA256.Create() as SHA256;

            byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(str));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();

        }

    }
}