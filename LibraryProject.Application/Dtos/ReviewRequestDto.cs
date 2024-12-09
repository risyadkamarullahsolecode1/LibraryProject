using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Dtos
{
    public class ReviewRequestDto
    {
        public int ProcessId { get; set; }

        public string Action { get; set; } = null!;

        public string Comment { get; set; } = null!;
    }
}
