using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Dtos
{
    public class BookRequestDto
    {
        public string? RequestName { get; set; }
        public string? Description { get; set; }
        public string? BookTitle { get; set; }
        public string? Author { get; set; }
        public string? Publisher { get; set; }
        public string? Comments { get; set; }
    }
}
