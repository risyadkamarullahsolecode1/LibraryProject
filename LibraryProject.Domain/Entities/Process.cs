using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Entities
{
    public class Process
    {
        [Key]
        public int ProcessId { get; set; } // Primary Key
        public int WorkflowId { get; set; } // Foreign Key to Workflow
        public string? RequesterId { get; set; } // Foreign Key to AspNetUsers
        public string? RequestType { get; set; }
        public string? Status { get; set; }
        public int? CurrentStepId { get; set; } // Foreign Key to NextStepRules (nullable)

        public DateTime RequestDate { get; set; }

        // Navigation properties
        public Workflow? Workflow { get; set; }
        public AppUser? Requester { get; set; }
        public NextStepRule? CurrentStep { get; set; }
        public ICollection<BookRequest>? BookRequests { get; set; }
        public ICollection<WorkflowAction>? WorkflowActions { get; set; }
    }
}
