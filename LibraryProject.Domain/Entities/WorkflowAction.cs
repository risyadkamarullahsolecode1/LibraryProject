using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Entities
{
    public class WorkflowAction
    {
        [Key]
        public int ActionId { get; set; } // Primary Key
        public int ProcessId { get; set; } // Foreign Key to RequestBook
        public int StepId { get; set; } // Foreign Key to WorkflowSequences
        public string? ActorId { get; set; } // Foreign Key to AspNetUsers

        public string? Action { get; set; }
        public DateTime? ActionDate { get; set; }
        public string? Comment { get; set; }

        // Navigation properties
        public Process? Process { get; set; }
        public WorkflowSequence? Step { get; set; }
        public AppUser? Actor { get; set; }
    }
}
