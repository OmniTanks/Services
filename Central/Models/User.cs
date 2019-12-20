using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Central.Models
{
    public class User
    {
        [Key]
        public int index { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public int Temporary { get; set; }
        public string State { get; set; }
        public string Permissions { get; set; }
        public string CosmeticsSettings { get; set; }
        public int Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUsedDate { get; set; }
        public string VerificationHash { get; set; }
        public List<Access> Accesses { get; set; }
    }
}
