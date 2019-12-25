using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Configuration;
using MySql.Data;
using MySql.Data.Entity;

using CentralServices.Models;

namespace CentralServices.Databases
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class LocalSettingsDB : DbContext
    {
        public DbSet<Setting> Settings { get; set; }


        public LocalSettingsDB() : base (ConfigurationManager.ConnectionStrings["CentralServices.Databases.LocalSettingsDB"] != null ? ConfigurationManager.ConnectionStrings["CentralServices.Databases.LocalSettingsDB"].ConnectionString : "server=localhost;database=centralsettings;user=centuser")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
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
