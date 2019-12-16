using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Central.Models
{
    public class Access
    {
        [Key]
        public int index { get; set; }

        public string UserID { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
