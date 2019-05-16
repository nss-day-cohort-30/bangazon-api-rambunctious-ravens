using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Computer
    {
        //This class stores all of our attributes for Computer

        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime? PurchaseDate { get; set; }
        //The question mark turns datetime into a nullable type
        public DateTime? DecomissionDate { get; set; }
        [Required]
        public string Make { get; set; }
        [Required]
        public string Manufacturer { get; set; }
    }
}
