namespace Sadna_17_B_API.Models
{
    public class StoreDTO
    {
        public Dictionary<string,string> doc { get; set; }
        public int storeId { get; set; }
        public Dictionary<int, int> quantities { get; set; }
    }
}
