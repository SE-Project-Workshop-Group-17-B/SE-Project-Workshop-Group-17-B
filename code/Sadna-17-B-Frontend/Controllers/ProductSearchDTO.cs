using System.Collections.Generic;

namespace Sadna_17_B_Frontend.Controllers
{
    internal class ProductSearchDTO
    {
        public string AccessToken { get; set; }
        public Dictionary<string, string> SearchCriteria { get; set; }
    }
}