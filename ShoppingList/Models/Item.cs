using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingList.Models
{
    public class Item
    {
        public int Id { get; set; }
        
        public string ProductName { get; set; } // Sorted name

        public string ProductDescription { get; set; } // Sorted description

        public string ProductSorted { get; set; } // Важливий середній неважливий

        [Range(1, int.MaxValue, ErrorMessage = "Price must be at least 1")]
        public decimal Price { get; set; } // Sorted price

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }

    public enum SortState
    {
        ProductNameAsc,
        ProductNameDesc,
        ProductDescriptionAsc,
        ProductDescriptionDesc,
        ProductSortedAsc,
        ProductSortedDesc,
        PriceAsc,
        PriceDesc
    }
}
