using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MonsterTradingCards.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards
{
    public static class Extensions
    {
        private static string salt = ConfigurationManager.AppSettings["HashSalt"];
        public static void AddParameterWithValue(this IDbCommand command, string parameterName, DbType type, object value)
        {
            var parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;

            command.Parameters.Add(parameter);
        }
        public static string HashPassword(this string str)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: str,
                    salt: Encoding.ASCII.GetBytes(salt ?? string.Empty),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
            return hashed;
        }

        public static T RandomElement<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.RandomElementUsing<T>(new Random());
        }

        public static T RandomElementUsing<T>(this IEnumerable<T> enumerable, Random rand)
        {
            int index = rand.Next(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }
    }
}
