using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechBooks.Models
{
    public class Book
    {
        [Display(Name = "Book Id")]
        public int BookId { get; set; }

        [Display(Name = "Category Id")]
        [Required]
        public int CategoryId { get; set; }

        public Category Category { get; set; } = new Category();

        [Required]
        public string Title { get; set; } = string.Empty;
    }

}
