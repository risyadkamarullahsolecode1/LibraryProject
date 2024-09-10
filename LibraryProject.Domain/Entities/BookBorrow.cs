using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Entities
{
    public class BookBorrow
    {
        [Key]
        public int BorrowId { get; set; }
        public int BookId {get; set; }
        public string? AppUserId { get; set; }
        public DateOnly? TanggalPinjam { get; set; }
        public DateOnly? TanggalKembali { get; set; }
        public DateOnly? DueDate { get; set; }
        public int? Penalty { get; set; }
        public Book? Book { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
