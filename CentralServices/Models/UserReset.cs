using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CentralServices.Models
{
    public class UserReset
    {
        [Key]
        public int index { get; set; }
        public string UserID { get; set; } = string.Empty;
        public string TempHash { get; set; } = string.Empty;
        public DateTime Requested { get; set; } = DateTime.MinValue;
        public DateTime Used { get; set; } = DateTime.MinValue;
        public int Active { get; set; } = 0;
    }
}
