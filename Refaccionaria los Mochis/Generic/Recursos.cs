using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace Refaccionaria_los_Mochis.Generic
{
    class Recursos
    {
        //Cifrar contraseñas 
        public string ConvertirSha256(string texto)
        {
            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));
                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }
    }
}
