using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Entities
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        public string Category { get; set; } = null!;

        public string Title { get; set; } = null!;

        [Column("ISBN")]
        public string Isbn { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string Publisher { get; set; } = null!;

        public string? Description { get; set; }

        public string Language { get; set; } = null!;

        public string Location { get; set; } = null!;

        public DateTime PurchaseDate { get; set; }

        public int Price { get; set; }

        public int TotalBook { get; set; }

        public bool DeleteStamp { get; set; }

        public string? DeleteStatus { get; set; }
    }
}
