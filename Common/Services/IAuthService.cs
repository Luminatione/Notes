using Common.DTO;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public interface IAuthService
    {
        public Task<Response<ClaimsPrincipal>> Login(string username, string password);
        public Task<Response> Register(RegisterDto registerDto);
    }
}
