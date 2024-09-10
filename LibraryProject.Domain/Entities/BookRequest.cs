using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Entities
{
    public class BookRequest
    {
        [Key]
        public int RequestId { get; set; } // Primary Key
        public string? RequestName { get; set; }
        public string? Description { get; set; }
        public string? AppUserId { get; set; }
        public int ProcessId { get; set; } // Foreign Key to Process
        public string? BookTitle { get; set; }
        public string? Author { get; set; }
        public string? Publisher { get; set; }
        public Process? Process { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
