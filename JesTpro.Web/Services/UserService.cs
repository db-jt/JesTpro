// This file is part of JesTpro project.
//
// JesTpro is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (if needed) any later version.
//
// JesTpro has NO WARRANTY!! It is distributed for test, study or 
// personal environments. Any commercial distribution
// has no warranty! 
// See the GNU General Public License in root project folder  
// for more details or  see <http://www.gnu.org/licenses/>

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using jt.jestpro.dal.Entities;
using jt.jestpro.Helpers;
using jt.jestpro.Models;
using Microsoft.Extensions.Logging;
using jt.jestpro.dal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using jt.jestpro.Helpers.ExtensionMethods;
using jt.jestpro.Mailer;

namespace jt.jestpro.Services
{
    public interface IUserService
    {
        LoginResultDto Authenticate(string username, string password);
        Task<List<UserDto>> LoadAll();
        Task<UserDto> Add(UserEditDto user);
        Task<bool> ChangePassword(ResetPasswordDto data);
        Task<UserDto> Edit(UserEditDto user);
        Task<bool> ResetPassword(ResetPasswordDto item);
        Task<bool> Delete(Guid id);
        Task<LoginResultDto> RefreshToken();
        Task<string> SaveUserDashboardData(string dashboardData);
        Task<List<UserDto>> GetTeacherList();
    }

    public class UserService : IUserService
    {
        // TODO: Al momento non si sa come viene gestita l'autenticazione (probabilmente AD)
        // per ora si mocka un solo utente lato codice
        //private List<User> _users = new List<User>
        //{ 
        //    new User { Id = 1, FirstName = "Wally", LastName = "E.", Username = "admin", Password = "Pa$$w0rd", Role = "Admin" },
        //    new User { Id = 1, FirstName = "Useraccio", LastName = "Prova", Username = "user", Password = "Pa$$w0rd", Role = "User" }
        //};

        private readonly AppSettings _appSettings;
        
        ILogger<UserService> _logger;
        MyDBContext _dbCtx;
        ClaimsPrincipal _claimPrincipal;

        public UserService(IOptions<AppSettings> appSettings, ClaimsPrincipal claimPrincipal, ILogger<UserService> logger, MyDBContext dbCtx)
        {
            _appSettings = appSettings.Value;
            _logger = logger;
            _dbCtx = dbCtx;
            _claimPrincipal = claimPrincipal;
        }

        private async Task<User> GetInner(Guid id)
        {
           var res = await _dbCtx.Users.FirstOrDefaultAsync(x => x.Id == id);
           return res;
        }

        public LoginResultDto Authenticate(string username, string password)
        {
            _logger.LogDebug($"{username} is trying to login");

            // DISABLED FOR SECURITY ISSUE
            //if (username.Equals("JestAdmin", StringComparison.InvariantCultureIgnoreCase) && password.Equals("12345678.Abcd"))
            //{
            //   return DoFakeLogin();
            //}

            var user = _dbCtx.Users.SingleOrDefault(x => x.UserName.ToLower().Equals(username.ToLower()));

            // return null if user not found
            if (user == null)
            {
                _logger.LogError($"User [{username}] not exists!");
                return null;
            }

            if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                _logger.LogError($"Password mismatch for user [{username}]!");
                return null; //password mismatch
            }

            //user.Token = tokenHandler.WriteToken(token);
            var reslogin = new LoginResultDto();
            reslogin.User = user.ToDto();
            reslogin.User.RoleName = user.Role.Name;
            reslogin.User.IdRole = user.IdRole;
            reslogin.Token = CreateToken(user);
            reslogin.Role = 0; //ADMIN
            reslogin.Id = user.Id;
            reslogin.Authenticated = true;
            var cTypes = _dbCtx.CustomerTypes.Where(x => x.XDeleteDate == null).ToArray();
            reslogin.CustomerTypes = cTypes.Select(x => x.ToDto()).ToArray();
            return reslogin;
        }

        public async Task<LoginResultDto> RefreshToken()
        {
            var claims = ((ClaimsIdentity)_claimPrincipal.Identity).Claims;
            var userRole = claims.SingleOrDefault(x => x.Type == ClaimTypes.Name);
            if (userRole == null)
            {
                throw new Exception("No role found for current user... aborting request!");
            }

            var currentUserId = Guid.Parse(userRole.Value);
            var user = await _dbCtx.Users.FirstOrDefaultAsync(x => x.Id == currentUserId);
            _logger.LogInformation($"RefreshToken for [{user.UserName}]");
            var reslogin = new LoginResultDto();
            reslogin.User = user.ToDto();
            reslogin.User.RoleName = user.Role.Name;
            reslogin.User.IdRole = user.IdRole;
            reslogin.Token = CreateToken(user);
            reslogin.Role = 0; //ADMIN
            reslogin.Id = user.Id;
            reslogin.Authenticated = true;
            return reslogin;
        }

        private String CreateToken(User user)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName.ToString()));
            claims.Add(new Claim(ClaimTypes.Surname, user.LastName.ToString()));
            claims.Add(new Claim(ClaimTypes.WindowsAccountName, user.UserName.ToString()));
            claims.Add(new Claim(ClaimTypes.Role, user.Role.Name.ToString()));
            claims.Add(new Claim(ClaimTypesExtended.IdRole, user.Role.Id.ToString()));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }

        public async Task<List<UserDto>> LoadAll()
        {
            var users = await _dbCtx.Users.Where(x => !x.XDeleteDate.HasValue).ToArrayAsync();
            return users.Select(x => x.ToDto()).ToList();
        }

        private bool UserIsSuperAdmin()
        {
            var claims = ((ClaimsIdentity)_claimPrincipal.Identity).Claims;
            var userRole = claims.SingleOrDefault(x => x.Type == ClaimTypesExtended.IdRole);
            if (userRole == null || userRole.Value == null)
            {
                return false;
            }

            if (int.Parse(userRole.Value) == (int)UserRoles.SuperAdmin)
            {
                return true;
            }

            return false;
        }

        public async Task<UserDto> Add(UserEditDto userDto)
        {
            var checkUser = _dbCtx.Users.SingleOrDefault(x => x.UserName.ToLower().Equals(userDto.UserName.ToLower()));
            if (checkUser != null)
            {
                _logger.LogError($"Username [{userDto.UserName}] not available!");
                throw new Exception($"Username [{userDto.UserName}] not available!");
            }

            if (userDto.IdRole == 0 && !UserIsSuperAdmin())
            {
                _logger.LogError($"Only administrators can create administrator users!");
                throw new Exception($"Only administrators can create administrator users!");
            }
            var user = userDto.ToEntity();
            user.Id = Guid.NewGuid();
            byte[] passwordHash, passwordSalt;
            Utils.CreatePasswordHash(userDto.Password, out passwordHash, out passwordSalt);

            user.PasswordHash = Utils.ByteArrayToString(passwordHash);
            user.PasswordSalt = Utils.ByteArrayToString(passwordSalt);

            await _dbCtx.Users.AddAsync(user);
            
            await _dbCtx.SaveChangesAsync();
            var res = await GetInner(user.Id); 
            return res.ToDto();
        }

        private bool VerifyPassword(string password, string hexpsw, string hexsalt)
        {
            byte[] passwordHash = Utils.StringToByteArray(hexpsw);
            byte[] passwordSalt = Utils.StringToByteArray(hexsalt);
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); // Create hash using password salt.
                for (int i = 0; i < computedHash.Length; i++)
                { // Loop through the byte array
                    if (computedHash[i] != passwordHash[i]) return false; // if mismatch
                }
            }
            return true; //if no mismatches.
        }

        public async Task<bool> ChangePassword(ResetPasswordDto data)
        {
            var claims = ((ClaimsIdentity)_claimPrincipal.Identity).Claims;
            var userRole = claims.SingleOrDefault(x => x.Type == ClaimTypes.Name);
            if (userRole == null)
            {
                throw new Exception("No role found for current user... aborting request!");
            }

            var currentUserId = Guid.Parse(userRole.Value);
            var cUser = await _dbCtx.Users.FirstOrDefaultAsync(x => x.Id == currentUserId);
            if (VerifyPassword(data.Password, cUser.PasswordHash, cUser.PasswordSalt))
            {
                byte[] passwordHash, passwordSalt;
                Utils.CreatePasswordHash(data.Confirm, out passwordHash, out passwordSalt);

                cUser.PasswordHash = Utils.ByteArrayToString(passwordHash);
                cUser.PasswordSalt = Utils.ByteArrayToString(passwordSalt);
                _dbCtx.Users.Update(cUser);
                await _dbCtx.SaveChangesAsync();
                return true;
            } 
            else
            {
                throw new Exception("Old password does't match!");
            }
        }

        public async Task<UserDto> Edit(UserEditDto user)
        {
            
            var cUser = await _dbCtx.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
            CheckUserPermission(cUser);
            _logger.LogInformation($"Saving user {cUser.UserName} id=[{cUser.Id}]");
            cUser.FirstName = user.FirstName;
            cUser.LastName = user.LastName;
            cUser.Email = user.Email;
            cUser.IdRole = user.IdRole;
            cUser.Disabled = user.Disabled;
            cUser.Lang = user.Lang;
            _dbCtx.Users.Update(cUser);

            await _dbCtx.SaveChangesAsync();
            var res = await GetInner(user.Id);
            return res.ToDto();
        }

        private void CheckUserPermission(User userToCheck)
        {
            var res = true;
            var claims = ((ClaimsIdentity)_claimPrincipal.Identity).Claims;
            var userId = claims.SingleOrDefault(x => x.Type == ClaimTypes.Name);
            if (userId == null)
            {
                throw new Exception("No id found for current user... aborting request!");
            }
            var userRole = claims.SingleOrDefault(x => x.Type == ClaimTypesExtended.IdRole);
            if (userRole == null)
            {
                throw new Exception("No role found for current user... aborting request!");
            }

            if (Guid.Parse(userId.Value) != userToCheck.Id)
            {
                if (int.Parse(userRole.Value) == (int)UserRoles.SuperAdmin || int.Parse(userRole.Value) == (int)UserRoles.PowerUser)
                {
                    return;
                }
                else
                {
                    throw new Exception("No privilege for edit this user");
                }
            }
        }

        public async Task<bool> ResetPassword(ResetPasswordDto data)
        {
           
            var currentUserId = data.IdUser;
            var cUser = await _dbCtx.Users.FirstOrDefaultAsync(x => x.Id == currentUserId);
            CheckUserPermission(cUser);

            byte[] passwordHash, passwordSalt;
            Utils.CreatePasswordHash(data.Confirm, out passwordHash, out passwordSalt);

            cUser.PasswordHash = Utils.ByteArrayToString(passwordHash);
            cUser.PasswordSalt = Utils.ByteArrayToString(passwordSalt);
            _dbCtx.Users.Update(cUser);
            await _dbCtx.SaveChangesAsync();
            return true;
            
        }

        public async Task<bool> Delete(Guid id)
        {
            var cUser = await _dbCtx.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (cUser.IdRole == 0 && !UserIsSuperAdmin())
            {
                _logger.LogError($"Only administrators can delete administrator users!");
                throw new Exception($"Only administrators can delete administrator users!");
            }
            //await RemovePFFromUser(cUser);
            cUser.XDeleteDate = DateTime.UtcNow;
            _dbCtx.Users.Update(cUser);
            await _dbCtx.SaveChangesAsync();
            return true;
        }

        public async Task<string> SaveUserDashboardData(string dashboardData)
        {
            var claimUserId = Guid.Parse(_claimPrincipal.Identity.Name);
            var cUser = await _dbCtx.Users.FirstOrDefaultAsync(x => x.Id == claimUserId);
            if (cUser == null)
            {
                throw new Exception($"No user found with ID {claimUserId}");
            }

            cUser.DashboardData = dashboardData;
            _dbCtx.Users.Update(cUser);
            await _dbCtx.SaveChangesAsync();
            return dashboardData;
        }

        public async Task<List<UserDto>> GetTeacherList()
        {
            var res = await _dbCtx.Users.Where(x => x.IdRole == (int)UserRoles.Teacher && !x.XDeleteDate.HasValue)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToArrayAsync();
            return res.Select(x => x.ToDto()).ToList();
        }
    }

    public enum UserRoles
    {
        SuperAdmin,
        Watcher,
        User,
        PowerUser,
        Teacher
    }

    public class ClaimTypesExtended
    {
        public static string IdRole = "IdRole";
    }
}