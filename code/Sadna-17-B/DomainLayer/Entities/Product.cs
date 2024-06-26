// DomainLayer/Entities/Product.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sadna_17_B.DomainLayer.Entities
{
    public class Product
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public int Quantity { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        // Foreign key for Store
        public int StoreID { get; set; }

        // Navigation property
        [ForeignKey("StoreID")]
        public virtual Store Store { get; set; }
    }
}