using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Central.Models.Requests
{
    public class AuthenticateUserRequest
    {
        public string MacAddress { get; set; } = string.Empty;
        public string LoginName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Password2 { get; set; } = string.Empty;
    }
}
