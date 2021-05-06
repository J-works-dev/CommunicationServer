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

        

        public string Id { get => id; set => id = value; }
        public string Password { get => password; set => password = value; }
    }
}
