using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationServer
{
    class User
    {
        private string id;
        private string password;
        private const int SALT_SIZE = 16;
        private const int HASH_SIZE = 20;

        public User(string id, string password)
        {
            this.id = id;

            // Create Salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SALT_SIZE]);

            // Creat Hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt);
            var hash = pbkdf2.GetBytes(HASH_SIZE);

            // Combine salt and hash
            var hashBytes = new byte[SALT_SIZE + HASH_SIZE];
            Array.Copy(salt, 0, hashBytes, 0, SALT_SIZE);
            Array.Copy(hash, 0, hashBytes, SALT_SIZE, HASH_SIZE);

            // Convert to base64
            var base64Hash = Convert.ToBase64String(hashBytes);

            // Save formatted hash with extra information in password
            this.password = string.Format("$MYHASH$V1${0}", base64Hash);
        }

        public bool Verify(string id, string pw)
        {
            // var splittedHash = password.Replace("$MYHASH$V1$", "").Split('$');
            // var base64Hash = splittedHash[1];
            var base64Hash = password.Replace("$MYHASH$V1$", "");

            // Get hash bytes
            var hashBytes = Convert.FromBase64String(base64Hash);

            // Get Salt
            var salt = new byte[SALT_SIZE];
            Array.Copy(hashBytes, 0, salt, 0, SALT_SIZE);

            // Creat hash with given salt
            var pbkdf2 = new Rfc2898DeriveBytes(pw, salt);
            byte[] hash = pbkdf2.GetBytes(HASH_SIZE);

            for (int i = 0; i < HASH_SIZE; i++)
            {
                if (hashBytes[i + SALT_SIZE] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }

        public string Id { get => id; set => id = value; }
        public string Password { get => password; set => password = value; }
    }
}
