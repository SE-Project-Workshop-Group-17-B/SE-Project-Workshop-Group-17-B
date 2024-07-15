namespace Sadna_17_B_API.Models
{
    public class UIStoreDTOAPI
    {
        public string AccessToken { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string StoreDescription { get; set; }
        public string Address { get; set; }
        public double Rating { set; get; }
        public int ID { set; get; }
        public string Complaint { set; get; }
        public Dictionary<string, string> Doc { get; set; }
    }
}
