using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lab.Models.Domain
{
    public class Product
    {
        public int ProductId { get; set; }
        [DisplayName("Product Name")]
        [Required(ErrorMessage = "Not null")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        public string ProductName { get; set; }
        [Required]
        [Range(0, 100)]
        [DisplayName("Quantity")]
        public int ProductQuantity { get; set; }
        [DisplayName("Image")]
        public string ProductImage { get; set; }        
        [Required]
        [Range(0, 999.99)]
        [DisplayName("Price")]
        public double ProductPrice { get; set; }
        [DisplayName("Descriptions")]
        public string Descriptions { get; set; }
        [DisplayName("Create on")]
        public DateTime CreateDate { get; set; }
        public Category Category { get; set; }
        [DisplayName("Category")]
        public int? CategoryId { get; set; }
    }
}
