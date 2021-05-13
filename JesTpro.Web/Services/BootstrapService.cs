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

using jt.jestpro.dal;
using jt.jestpro.Helpers;
using jt.jestpro.Helpers.ExtensionMethods;
using jt.jestpro.Mailer;
using jt.jestpro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace jt.jestpro.Services
{
    public interface IBootstrapService
    {
        Task CheckAdminUser();
    }
    public class BootstrapService : IBootstrapService { 
        MyDBContext _dbCtx;
        ILogger<BootstrapService> _logger;
        IOptions<AppSettings> _appSettings;
        public BootstrapService(MyDBContext dbCtx, IOptions<AppSettings> appSettings, ILogger<BootstrapService> logger)
        {
            _dbCtx = dbCtx;
            _logger = logger;
            _appSettings = appSettings;
        }
        public async Task CheckAdminUser()
        {
            var adminUser = await _dbCtx.Users.IgnoreQueryFilters().Where(x => x.UserName == "Admin").FirstOrDefaultAsync();
            if (adminUser == null)
            {
                var newAdminUser = new UserEditDto();
                newAdminUser.UserName = "Admin";
                newAdminUser.Password = Utils.CreatePassword(10);
                newAdminUser.FirstName = "Admin";
                newAdminUser.LastName = "Owner";
                newAdminUser.IdRole = 0;
                newAdminUser.Email = _appSettings.Value.SystemEmail;
                await AddAdmin(newAdminUser);
                _logger.LogWarning($"Admin user was created with password \"{newAdminUser.Password}\". Please store this password in a secure place! ");
            }
            else
            {
                if (adminUser.XDeleteDate.HasValue)
                {
                    //recover admin user
                    adminUser.XDeleteDate = null;
                    _dbCtx.Users.Update(adminUser);
                    await _dbCtx.SaveChangesAsync();
                    var res = new ResetPasswordDto();
                    res.IdUser = adminUser.Id;
                    res.Confirm = Utils.CreatePassword(10);
                    res.Password = Utils.CreatePassword(10);
                    await ResetAdminPassword(res);
                    _logger.LogWarning($"Admin was recover; the new password \"{res.Confirm}\". Please store this password in a secure place! ");
                }
            }
        }

        private async Task AddAdmin(UserEditDto userDto)
        {
            var checkUser = _dbCtx.Users.SingleOrDefault(x => x.UserName.ToLower().Equals(userDto.UserName.ToLower()));
            if (checkUser != null)
            {
                _logger.LogError($"Username [{userDto.UserName}] not available!");
                throw new Exception($"Username [{userDto.UserName}] not available!");
            }

            var user = userDto.ToEntity();
            user.Id = Guid.NewGuid();
            byte[] passwordHash, passwordSalt;
            Utils.CreatePasswordHash(userDto.Password, out passwordHash, out passwordSalt);

            user.PasswordHash = Utils.ByteArrayToString(passwordHash);
            user.PasswordSalt = Utils.ByteArrayToString(passwordSalt);

            await _dbCtx.Users.AddAsync(user);
            await _dbCtx.SaveChangesAsync();
        }

        public async Task ResetAdminPassword(ResetPasswordDto data)
        {
            var currentUserId = data.IdUser;
            var cUser = await _dbCtx.Users.FirstOrDefaultAsync(x => x.Id == currentUserId);

            byte[] passwordHash, passwordSalt;
            Utils.CreatePasswordHash(data.Confirm, out passwordHash, out passwordSalt);

            cUser.PasswordHash = Utils.ByteArrayToString(passwordHash);
            cUser.PasswordSalt = Utils.ByteArrayToString(passwordSalt);
            _dbCtx.Users.Update(cUser);
            await _dbCtx.SaveChangesAsync();
        }
    }
}
