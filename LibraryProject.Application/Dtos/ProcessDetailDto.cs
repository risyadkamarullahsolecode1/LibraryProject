using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Dtos
{
    public class ProcessDetailDto
    {
        public int ProcessId { get; set; }
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string Status { get; set; }
        public List<WorkflowActionDto> WorkflowActions { get; set; }
    }
}
