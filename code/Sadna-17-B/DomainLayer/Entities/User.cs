// DomainLayer/Entities/User.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sadna_17_B.DomainLayer.Entities
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        public DateTime RegisterDate { get; set; }

        public bool IsActive { get; set; }

        // Navigation property
        public virtual ICollection<Order> Orders { get; set; }

        public User()
        {
            Orders = new HashSet<Order>();
            RegisterDate = DateTime.Now;
            IsActive = true;
        }
    }
}