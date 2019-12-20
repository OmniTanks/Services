using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Central.Models;

namespace Central.Databases
{
    public class LocalSettingsDB : DbContext
    {
        public DbSet<Setting> Settings { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;database=centralsettings;user=centusr");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public string GetSetting(string name)
        {
            var query = Settings.Where(set => String.Equals(set.Name, name,StringComparison.OrdinalIgnoreCase));
            var item = query.FirstOrDefault<Setting>();

            if (item != null)
                return item.Value;

            return string.Empty;
        }

        public List<string> GetSettings(string name)
        {
            var query = Settings.Where(set => String.Equals(set.Name, name, StringComparison.OrdinalIgnoreCase));

            List<string> vals = new List<string>();
            foreach (var item in query)
                vals.Add(item.Value);

            return vals;
        }
    }
}
