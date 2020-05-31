using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace crm.Services
{
    public class RecoveryCodeValidator : IRecoveryCodeValidator
    {
        public bool RecoveryCodeValidAndUnused(string code)
        {
            throw new NotImplementedException();
        }

        public static string HashRecoveryCode(string code)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    code,
                    salt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 9500,
                    numBytesRequested: 256 / 8));
            return hashed;
        }
    }
}
