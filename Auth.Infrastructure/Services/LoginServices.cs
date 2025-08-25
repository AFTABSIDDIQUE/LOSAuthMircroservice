using Auth.Application.Repository;
using Auth.Domain.Model;
using Auth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Infrastructure.Services
{
    public class LoginServices:ILogin
    {
        ApplicationDbContext db;
        IConfiguration configuration;
        IToken tokens;

        public LoginServices(ApplicationDbContext db, IConfiguration configuration, IToken tokens)
        {
            this.db = db;
            this.configuration = configuration;
            this.tokens = tokens;
        }

        public string Logins(Login datas)
        {
            var details = db.User.Where(u=>u.Email == datas.UserName || u.UserName == datas.UserName).Select(p => p.PasswordHash).FirstOrDefault();

            if (details != null && details == datas.PasswordHash)
            {
                return tokens.GenerateToken(datas);
            }
            return null;

        }
    }
}
