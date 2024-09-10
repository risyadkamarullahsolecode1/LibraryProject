using LibraryProject.Application.Dtos.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseModel> SignUpAsync(RegisterUser model);
        Task<ResponseModel> LoginAsync(LoginModel model);
        Task<ResponseModel> CreateRoleAsync(string rolename);
        Task<ResponseModel> AssignToRoleAsync(string userName, string rolename);
        Task<ResponseModel> UpdateToRoleAsync(string userName, string rolename);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<ResponseModel> DeleteToRoleAsync(string userName, string rolename);
        Task<ResponseModel> LogoutAsync(string userName);
    }
}
