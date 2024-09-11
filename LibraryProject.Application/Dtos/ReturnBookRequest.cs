using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Dtos
{
    public class ReturnBookRequest
    {
        public string? UserId { get; set; }
        public int BookId { get; set; }
        public DateOnly TanggalKembali { get; set; }
    }
}
