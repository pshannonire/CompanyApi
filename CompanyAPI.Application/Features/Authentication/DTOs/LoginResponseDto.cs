using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAPI.Application.Features.Authentication.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }
        public string TokenType { get; set; } = "Bearer";
    }
}
