namespace Sadna_17_B_API.Models
{
    public class ItemDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Amount { get; set; }
        public int StoreId { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public ItemDTO() { }
    }
}
