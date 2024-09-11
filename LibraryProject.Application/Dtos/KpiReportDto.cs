using LibraryProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Dtos
{
    public class KpiReportDto
    {
        public int TotalBook { get; set; }
        public IEnumerable<BookBorrow> OverdueBooks { get; set; }
        public Dictionary<string, int> Category {  get; set; }
    }
}
