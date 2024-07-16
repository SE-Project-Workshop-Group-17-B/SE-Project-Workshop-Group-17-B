namespace Sadna_17_B_API.Models
{
    public class AddProductRatingDTO
    {
        public string AccessToken { get; set; }
        public int StoreID { get; set; }
        public int ProductID { get; set; }
        public int Rating { get; set; }
    }
}
