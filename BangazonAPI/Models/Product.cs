using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BangazonAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int ProductTypeId { get; set; }
        public int CustomerId { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Quantity { get; set; }
        public ProductType ProductType { get; set; }
        public Customer Customer { get; set; }

    }
}