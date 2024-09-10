using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Dtos.Account
{
    public class ResponseModel
    {
        public string? Status { get; set; }
        public string? Username { get; set; }
        public string? Message { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? ExpiredOn { get; set; }
        public List<string> Roles { get; set; }
    }
}
