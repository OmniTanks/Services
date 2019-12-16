using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;

using Central.Models;
using Microsoft.AspNetCore.Identity;

namespace Central.Databases
{
    public class UserDB : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Access> Accesses { get; set; }

        public DbSet<AuthToken> AuthTokens { get; set; }

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
            var query = Users.Where(u => u.Email == addres.ToUpperInvariant() && u.Active != 0);
            var user = query.FirstOrDefault<User>();

            if (user != null)
                return user.ID;

            return string.Empty;
        }

        public User GetUserByID(string id)
        {
            var query = Users.Where(u => u.ID == id.ToUpperInvariant() && u.Active != 0);
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

        public User CreateTemporaryUser(string macAddress)
        {
            User newUser = new User();
            while (newUser.Name == string.Empty || NameExists(newUser.Name))
                newUser.Name = NameGenerator.Generate();

            newUser.ID = NewID();
            newUser.Temporary = 1;
            newUser.Email = macAddress;
            newUser.Hash = NewToken();
            newUser.Permissions = "Temp";
            newUser.State = "Temp";
            newUser.CreateDate = new DateTime(DateTime.Now.Ticks);
            newUser.LastUsedDate = newUser.CreateDate;
            newUser.Active = 1;
            newUser.CosmeticsSettings = new LocalSettingsDB().GetSetting("TempCosmetics");
            if (newUser.CosmeticsSettings == string.Empty)
                newUser.CosmeticsSettings = new CosmeticsGroup().Serialize();

            Users.Add(newUser);
            SaveChangesAsync();

            return newUser;
        }

        public static string NewToken()
        {
            Random rng = new Random();
            return rng.Next(10000, 99999).ToString() + rng.Next(10000, 99999).ToString() + DateTime.Now.GetHashCode().ToString();
        }

        public string NewID()
        {
            Random rng = new Random();
            string ID = string.Empty;

            while(ID == string.Empty || GetUserByID(ID) != null)
                ID = rng.Next(100000, 999999).ToString() + rng.Next(100000, 999999).ToString();

            return ID;
        }

        public string AuthenticateUser (string userID, string credentials, string userAddress)
        {
            User user = GetUserByID(userID);
            if (user == null)
                return string.Empty;

            if (user.Temporary != 0)
            {
                if (user.Hash != credentials)
                    return string.Empty;
            }
            else
            {
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                var result = hasher.VerifyHashedPassword(user, user.Hash, credentials);
                if (result == PasswordVerificationResult.Failed)
                    return string.Empty;

                if (result == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    user.Hash = hasher.HashPassword(user, credentials);
                    SaveChanges();
                }
            }

            user.LastUsedDate = new DateTime(DateTime.Now.Ticks);
            Access access = new Access();
            access.UserID = user.ID;
            access.Timestamp = user.LastUsedDate;
            Accesses.Add(access);

            AuthToken token = new AuthToken();
            token.Token = NewToken();
            token.UserID = userID;
            token.UserAddress = userAddress;
            token.CreateDate = user.LastUsedDate;
            token.LastUsedDate = user.LastUsedDate;
            AuthTokens.Add(token);
            SaveChangesAsync();

            return token.Token;
        }
    }
}
