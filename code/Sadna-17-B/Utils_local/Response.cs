using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sadna_17_B.Utils
{
    public class Response
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public object Data { get; set; }

        public Response()
        {
        }

        public Response(string message, bool success, object data)
        {
            Message = message;
            Success = success;
            Data = data;
        }

        public Response(string message, bool success) : this(message, success, null)
        {
        }

        public Response(bool success) : this("", success, "")
        {
        }

        public Response(bool success, object data): this("", success, data)
        {
        }

        public static Response GetErrorResponse(Exception e)
        {
            string cause = "";
            if (e.GetBaseException() != null)
            {
                cause = e.GetBaseException().Message;
            }
            return new Response(e.Message, false, cause);
        }
    }
}