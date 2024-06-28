
// DomainLayer/Entities/OrderItem.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sadna_17_B.DomainLayer.Entities
{
    public class OrderItem
    {
        [Key]
        public int ID { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        // Foreign keys
        public int OrderID { get; set; }
        public int ProductID { get; set; }

        // Navigation properties
        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
    }
}