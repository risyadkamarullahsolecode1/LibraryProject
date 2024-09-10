using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public int? LibraryCardNumber { get; set; }

        public DateTime? LibraryCardExpDate { get; set; }

        public string Position { get; set; } = null!;

        public string Previlege { get; set; } = null!;

        public string? Note { get; set; }
        public bool? UnpaidPenalty { get; set; }
        public string? AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public virtual AppUser? AppUser { get; set; }
    }
}
