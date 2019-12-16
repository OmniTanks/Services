using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Central.Models
{
    public class Setting
    {
        [Key]
        public int index { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

    }
}
