using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Dtos.Account
{
    public class RegisterUser
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? LibraryCardNumber { get; set; }
        public DateTime? LibraryCardExpDate { get; set; }
        public string? Position { get; set; }
        public string? Previlege { get; set; }
        public bool? UnpaidPenalty { get; set; }
        public string? AppUserId { get; set; }
    }
}
