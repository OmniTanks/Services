using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Central.Models.Requests
{
    public class GenerateAnonUserRequest
    {
        public string MacAddress { get; set; } = string.Empty;
    }
}
