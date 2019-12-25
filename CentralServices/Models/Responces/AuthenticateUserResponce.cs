﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CentralServices.Models.Responces
{
    public class AuthenticateUserResponce
    {
        public string Result { get; set; } = "UNSET";
        public string UserID { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string AuthenticationToken { get; set; } = string.Empty;
        public CosmeticsGroup Cosmetics { get; set; } = new CosmeticsGroup();
    }
}
