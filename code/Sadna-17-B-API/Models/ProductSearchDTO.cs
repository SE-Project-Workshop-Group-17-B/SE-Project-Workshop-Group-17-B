using System.Collections.Generic;

namespace Sadna_17_B_API.Models
{
    public class ProductSearchDTO
    {
        public string AccessToken { get; set; }
        public Dictionary<string, string> SearchCriteria { get; set; }
    }
}