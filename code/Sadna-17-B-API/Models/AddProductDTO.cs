namespace Sadna_17_B_API.Models
{
    public class AddProductDTO
    {
        public string Token { get; set; }
        public int Sid { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
    }
}
