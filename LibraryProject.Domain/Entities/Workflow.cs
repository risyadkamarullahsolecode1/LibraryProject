using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Entities
{
    public class Workflow
    {
        [Key]
        public int WorkflowId { get; set; } // Primary Key
        public string? WorkflowName { get; set; }
        public string? Description { get; set; }

        // Navigation properties
        public ICollection<WorkflowSequence>? WorkflowSequence { get; set; }
        public ICollection<Process>? Processes { get; set; }
    }
}
