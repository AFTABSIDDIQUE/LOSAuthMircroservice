using Auth.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Application.Repository
{
    public interface IToken
    {
        public string GenerateToken(Login data);
    }
}
