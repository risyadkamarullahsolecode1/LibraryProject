using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Dtos.Account
{
    public class RefreshTokenResponseDto : ResponseModel
    {
        public string? AccessToken { get; set; }

        public DateTime? AccessTokenExpiryTime { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
