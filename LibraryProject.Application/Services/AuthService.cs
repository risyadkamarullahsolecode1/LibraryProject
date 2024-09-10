using LibraryProject.Application.Dtos.Account;
using LibraryProject.Application.Interfaces;
using LibraryProject.Domain.Entities;
using LibraryProject.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<IdentityRole> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IUserRepository userRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _userRepository = userRepository;
        }
        //Sign Up The User
        public async Task<ResponseModel> SignUpAsync(RegisterUser model)
        {
            var userExists = await _userManager.FindByNameAsync(model.FirstName + "" + model.LastName);
            if (userExists != null)

                return new ResponseModel { Status = "Error", Message = "User already exists!" };
            AppUser user = new AppUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.FirstName + "" + model.LastName
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return new ResponseModel
            {
                Status = "Error",
                Message = "User creation failed! Please check user details and try again."
            };
            var userId = user.Id;

            User users = new User
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                LibraryCardNumber = model.LibraryCardNumber,
                LibraryCardExpDate = model.LibraryCardExpDate,
                Position = model.Position,
                Previlege = model.Previlege,
                UnpaidPenalty = model.UnpaidPenalty.Value,
                AppUserId = userId
            };
            await _userRepository.AddUser(users);
            await _userRepository.SaveChangesAsync();
            return new ResponseModel { Status = "Success", Message = "User created succesfully!" };
        }

        //Login user
        public async Task<ResponseModel> LoginAsync(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole.ToString()));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                var refreshToken = GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                var refreshTokenExpiryDate = user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(8);

                await _userManager.UpdateAsync(user);

                return new ResponseModel
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    RefreshTokenExpiryTime = refreshTokenExpiryDate,
                    ExpiredOn = token.ValidTo,
                    Message = "User successfully login!",
                    Roles = userRoles.ToList(),
                    Status = "Success"
                };
            }
            return new ResponseModel { Status = "Error", Message = "Password Not valid!" };
        }
        // Create Role
        public async Task<ResponseModel> CreateRoleAsync(string rolename)
        {
            if (!await _roleManager.RoleExistsAsync(rolename))
                await _roleManager.CreateAsync(new IdentityRole(rolename));
            return new ResponseModel { Status = "Success", Message = "Role Created successfully!" };
        }

        // Assign user to role that already created before
        public async Task<ResponseModel> AssignToRoleAsync(string userName, string rolename)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (await _roleManager.RoleExistsAsync($"{rolename}"))
            {
                await _userManager.AddToRoleAsync(user, rolename);
            }
            return new ResponseModel { Status = "Success", Message = "User created succesfully!" };
        }

        // update role for user
        public async Task<ResponseModel> UpdateToRoleAsync(string userName, string rolename)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (await _roleManager.RoleExistsAsync($"{rolename}"))
            {
                await _userManager.AddToRoleAsync(user, rolename);
            }
            return new ResponseModel { Status = "Success", Message = "User roles updated succesfully!" };
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JWT");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"))),
                ValidateLifetime = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"]
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token");
            }
            return principal;
        }
        //Delete roles for user
        public async Task<ResponseModel> DeleteToRoleAsync(string userName, string rolename)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (await _roleManager.RoleExistsAsync($"{rolename}"))
            {
                await _userManager.RemoveFromRoleAsync(user, rolename);
            }
            return new ResponseModel { Status = "Success", Message = "User roles deleted succesfully!" };
        }
        public async Task<ResponseModel> LogoutAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new Exception("There is no username like this in database");
            }
            await _signInManager.SignOutAsync().ConfigureAwait(false);
            return new ResponseModel { Status = "Success", Message = "User successfully logout" };
        }
    }
}
