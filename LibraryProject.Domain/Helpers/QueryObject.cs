using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Helpers
{
    public class QueryObject
    {
        public string? ISBN { get; set; }
        public string? Category { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? QueryOperators { get; set; }
    }
}
