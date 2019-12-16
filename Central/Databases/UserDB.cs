using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;

using Central.Models;

namespace Central.Databases
{
    public class UserDB : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Access> Accesses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;database=centralservices;user=centusr");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        
        public string FindIDByMacAddress(string addres)
        {
            var query = from usr in Users where usr.Email == addres.ToUpperInvariant() select usr;
            var user = query.FirstOrDefault<User>();

            if (user != null)
                return user.ID;

            return string.Empty;
        }

        public User GetUserByID(string id)
        {
            var query = from usr in Users where usr.ID == id.ToUpperInvariant() select usr;
            var user = query.FirstOrDefault<User>();

            if (user != null)
                return user;

            return null;
        }

        public bool NameExists(string name)
        {
            var query = from usr in Users where usr.Name == name select usr;
            var user = query.FirstOrDefault<User>();

            return user != null;
        }

        private string[] FirstNames = new string[]{ "Magic",
                                                    "Lemon",
                                                    "Sweetie",
                                                    "Frosty",
                                                    "Jewel",
                                                    "Brilliant",
                                                    "Stormy",
                                                    "Twinkle",
                                                    "Dewdrop",
                                                    "Sky",
                                                    "Pepper",
                                                    "Dusty",
                                                    "Fire",
                                                    "Sparkle",
                                                    "Midnight",
                                                    "Crystal",
                                                    "Cool",
                                                    "Minty",
                                                    "Cloud",
                                                    "Citrus",
                                                    "Peach",
                                                    "Bubble",
                                                    "Berry",
                                                    "Light",
                                                    "Beauty",
                                                    "Rose",
                                                    "Dawn",
                                                    "Cocoa",
                                                    "Summer",
                                                    "Star",
                                                    "Speedy",
                                                    "Whirly",
                                                    "Lucky",
                                                    "Gold"
                                                    };

        private string[] LastNamed = new string[] { "Red",
                                                    "Mint",
                                                    "Flip",
                                                    "Skipper",
                                                    "Dahlia",
                                                    "Tangy",
                                                    "Pink",
                                                    "Berry",
                                                    "Poppy",
                                                    "Bright",
                                                    "Glow",
                                                    "Sugar",
                                                    "Dasher",
                                                    "Muffin",
                                                    "Dash",
                                                    "Comet",
                                                    "Onyx",
                                                    "Glory",
                                                    "Treat",
                                                    "Apple",
                                                    "Lilac",
                                                    "Dreams",
                                                    "Emerald",
                                                    "Jubilee",
                                                    "Joy",
                                                    "Heart",
                                                    "Eyes",
                                                    "Sunset",
                                                    "Pie",
                                                    "Blueberry",
                                                    "Flitter",
                                                    "Magenta"
                                                };


        private string RandomElement(string[] contents)
        {
            return contents[new Random().Next(contents.Length)];
        }

        public string GenerateName()
        {
            return RandomElement(FirstNames) + RandomElement(LastNamed) + new Random().Next(12, 99).ToString();
        }
    }
}
