using System;
using System.ComponentModel.DataAnnotations;

namespace CoffeeLoyaltyApp.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الجوال مطلوب")]
        public string PhoneNumber { get; set; } = string.Empty;

        public int StampCount { get; set; } = 0;

        public int FreeCupsCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}