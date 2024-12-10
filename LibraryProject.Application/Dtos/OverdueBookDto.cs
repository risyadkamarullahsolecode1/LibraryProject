using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Dtos
{
    public class OverdueBookDto
    {
        public int? BorrowId { get; set; }
        public string? BookTitle { get; set; }
        public string? UserName { get; set; }
        public DateOnly? DueDate { get; set; }
        public int? Penalty { get; set; }
    }
}
