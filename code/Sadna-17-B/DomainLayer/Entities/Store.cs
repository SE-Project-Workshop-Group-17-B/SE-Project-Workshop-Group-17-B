// DomainLayer/Entities/Store.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sadna_17_B.DomainLayer.Entities
{
    public class Store
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; }

        // Navigation property
        public virtual ICollection<Product> Products { get; set; }

        public Store()
        {
            Products = new HashSet<Product>();
            CreatedDate = DateTime.Now;
            IsActive = true;
        }
    }
}




