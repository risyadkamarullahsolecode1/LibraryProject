using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Entities
{
    public class WorkflowSequence
    {
        [Key]
        public int StepId { get; set; } // Primary Key
        [ForeignKey("Workflow")]
        public int WorkflowId { get; set; } // Foreign Key to Workflow
        public virtual Workflow? Workflow { get; set; } // Workflow Reference
        public string? StepName { get; set; }
        public int StepOrder { get; set; }
        [ForeignKey("RequiredRole")]
        public string? RequiredRoleId { get; set; } // Foreign Key to AspNetRoles
        // Navigation properties
        public virtual IdentityRole? RequiredRole { get; set; } // Reference to AspNetRoles
    }
}
