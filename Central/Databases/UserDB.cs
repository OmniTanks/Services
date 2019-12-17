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
        
        public string FindIDByAddress(string addres)
        {
            var query = Users.Where(u => u.Email == addres.ToUpperInvariant() && u.Active != 0);
            var user = query.FirstOrDefault<User>();

            if (user != null)
                return user.ID;

            return string.Empty;
        }

        public User FindByName (string name)
        {
            TimeSpan maxTempAge = new TimeSpan(30, 0, 0);
            string days =  new LocalSettingsDB().GetSetting("MaxTempAge");
            if (days != string.Empty)
            {
                int d = 0;
                if (int.TryParse(days,out d))
                    maxTempAge = new TimeSpan(d, 0, 0);
            }

            DateTime cutoff = DateTime.Now - maxTempAge;

            var query = Users.Where(u => u.Name.ToUpperInvariant() == name.ToUpperInvariant() && u.Active != 0 && (u.Temporary == 0 || u.LastUsedDate > cutoff));
            return query.FirstOrDefault<User>();
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

        public User ConvertUser(User user, Models.Requests.RegisterUserRequest request)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();

            user.Email = request.Email;
            user.Temporary = 0;
            user.Hash = hasher.HashPassword(user, request.Password);
            user.Permissions = "User";
            user.State = "Unverified";
            user.LastUsedDate = new DateTime(DateTime.Now.Ticks);
            user.Active = 1;
            user.CosmeticsSettings = new LocalSettingsDB().GetSetting("RegCosmetics");
            if (user.CosmeticsSettings == string.Empty)
                user.CosmeticsSettings = new CosmeticsGroup().Serialize();

            SaveChangesAsync();

            return user;
        }

        public User CreateUser(string userName, string email, string password)
        {
            User newUser = new User();
            while (newUser.Name == string.Empty || NameExists(newUser.Name))
                newUser.Name = NameGenerator.Generate();

            PasswordHasher<User> hasher = new PasswordHasher<User>();

            newUser.ID = NewID();
            newUser.Temporary = 0;
            newUser.Email = email;
            newUser.Hash = hasher.HashPassword(newUser, password);
            newUser.Permissions = "User";
            newUser.State = "Unverified";
            newUser.CreateDate = new DateTime(DateTime.Now.Ticks);
            newUser.LastUsedDate = newUser.CreateDate;
            newUser.Active = 1;
            newUser.CosmeticsSettings = new LocalSettingsDB().GetSetting("RegCosmetics");
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

        public static bool ValidPassword(string password)
        {
            if (password.Length < 7)
                return false;

            bool oneLetter = false;
            bool oneNumber = false;
            bool oneOther = false;
            foreach (var c in password)
            {
                if (char.IsDigit(c))
                    oneNumber = true;

                if (char.IsLetter(c))
                    oneLetter = true;

                if (!char.IsLetterOrDigit(c))
                    oneOther = true;
            }

            return oneLetter && oneNumber && oneOther;
        }


        public static bool ValidName(string name)
        {
            if (name.Length < 3)
                return false;

            foreach (var c in name)
            {
                if (!char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                    return false;
            }

            // TODO check bad names

            return true;
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
