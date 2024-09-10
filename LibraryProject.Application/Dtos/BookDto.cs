using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Dtos
{
    public class BookDto
    {
        public string? Category { get; set; }
        public string? Title { get; set; }
        public string? ISBN { get; set; }
        public string? Author { get; set; }
        public string? Publisher { get; set; }
        public string? Description { get; set; }
        public string? Language { get; set; }
        public string? Location { get; set; }
    }
}
