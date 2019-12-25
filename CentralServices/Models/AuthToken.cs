using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace CentralServices.Models
{
    public class AuthToken
    {
        [Key]
        public int index { get; set; }
        public string Token { get; set; }
        public string UserID { get; set; }
        public User User { get; set; }
        public string UserAddress { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUsedDate { get; set; }
    }
}
