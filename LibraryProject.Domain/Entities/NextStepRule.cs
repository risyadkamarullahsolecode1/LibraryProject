using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Entities
{
    public class NextStepRule
    {
        [Key]
        public int RuleId { get; set; } // Primary Key
        public int CurrentStepId { get; set; } // Foreign Key to WorkflowSequences
        public int NextStepId { get; set; } // Foreign Key to WorkflowSequences
        public string? ConditionType { get; set; }
        public string? ConditionValue { get; set; }

        // Navigation properties
        public WorkflowSequence? CurrentStep { get; set; }
        public WorkflowSequence? NextStep { get; set; }
    }
}
