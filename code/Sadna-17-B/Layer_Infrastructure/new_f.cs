using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;

namespace Sadna_17_B.Layer_Infrastructure
{
    public class c
    {
        [JsonPropertyName("name")]
        public string name { get; set; }
    }


    public class a: c
    {
        [JsonPropertyName("class a")]
        public string type { get; set; }
    }

    public class b : c
    {
        [JsonPropertyName("class b")]
        public string type { get; set; }
    }
}