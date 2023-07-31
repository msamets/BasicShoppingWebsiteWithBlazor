using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopOnline.Models.Dtos
{
    public class AddProductDto
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ImageURL { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Qty { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public ProductCategoryDto ProductCategory { get; set; }
    }
}
