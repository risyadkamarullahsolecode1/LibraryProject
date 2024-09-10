using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Entities
{
    public class WorkflowSequence
    {
        [Key]
        public int StepId { get; set; } // Primary Key
        public int WorkflowId { get; set; } // Foreign Key to Workflow
        public string? StepName { get; set; }
        public int StepOrder { get; set; }
        public string? RequiredRoleId { get; set; } // Foreign Key to AspNetRoles

        // Navigation properties
        public Workflow? Workflow { get; set; } // Workflow Reference
        public IdentityRole? RequiredRole { get; set; } // Reference to AspNetRoles
        public ICollection<WorkflowAction>? WorkflowActions { get; set; }
        public ICollection<NextStepRule>? NextStepRules { get; set; }
    }
}
