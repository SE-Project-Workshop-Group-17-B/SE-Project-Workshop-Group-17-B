using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.Utils
{
    /// <summary>
    /// This custom exception class is used for passing customized messages between the calling layers.
    /// It could be used for differentiating between actual logical errors in the code and customized errors between the layers,
    /// in try-catch blocks, if needed.
    /// </summary>
    public class Sadna17BException : Exception
    {
        public Sadna17BException(string message) : base(message)
        {
        }

        public Sadna17BException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}