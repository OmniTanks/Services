using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentralServices.Models.Requests
{
    public class RegisterUserRequest
    {
        public string MacAddress { get; set; } = string.Empty;
        public string DesiredName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string TemporyID { get; set; } = string.Empty;
        public string TemporaryAccessKey { get; set; } = string.Empty;
    }
}
