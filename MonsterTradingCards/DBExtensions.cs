using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MonsterTradingCards.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards
{
    public static class DbExtensions
    {
        public static void AddParameterWithValue(this IDbCommand command, string parameterName, DbType type, object value)
        {
            var parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = parameterName;
            parameter.Value = value ?? DBNull.Value;

            command.Parameters.Add(parameter);
        }

        public static int? GetNullableInt32(this IDataRecord record, int ordinal)
        {
            int? value = null;
            if (!record.IsDBNull(ordinal))
            {
                value = record.GetInt32(ordinal);
            }
            return value;
        }

        public static string GetNullableString(this IDataRecord record, int ordinal)
        {
            string value = null;
            if (!record.IsDBNull(ordinal))
            {
                value = record.GetString(ordinal);
            }
            return value;
        }

        public static Guid GetNullableGuid(this IDataRecord record, int ordinal)
        {
            Guid value = Guid.Empty;
            if (!record.IsDBNull(ordinal))
            {
                value = record.GetGuid(ordinal);
            }
            return value;
        }

        public static string ConvertAttributesToSetSQL<T>(this IRepository<T> repository, string[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            parameters.ToList().ForEach(parameter => sb.AppendFormat("{0} = @{0}, ", parameter));
            sb.Remove(sb.Length-2, 2);
            sb.Append(" ");
            return sb.ToString();
        }

        public static string HashPassword(this string str)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: str,
                    salt: Encoding.ASCII.GetBytes("Jf9!87_44/fh"),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
            return hashed;
        }
    }
}
