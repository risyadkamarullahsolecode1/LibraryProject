using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Entities
{
    public class WorkflowAction
    {
        [Key]
        public int ActionId { get; set; } // Primary Key
        [ForeignKey("Process")]
        public int ProcessId { get; set; } // Foreign Key to RequestBook
        public virtual Process? Process { get; set; }
        [ForeignKey("WorkflowSequence")]
        public int? StepId { get; set; } // Foreign Key to WorkflowSequences
        public virtual WorkflowSequence? Step { get; set; }
        [ForeignKey("AspNetUsers")]
        public string? ActorId { get; set; } // Foreign Key to AspNetUsers
        public virtual AppUser? Actor { get; set; }
        public string? Action { get; set; }
        public DateTime? ActionDate { get; set; }
        public string? Comment { get; set; }
    }
}
