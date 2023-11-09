using MonsterTradingCards.Repositories;
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

        public static string ConvertAttributesToSetSQL<T>(this IRepository<T> repository, string[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            parameters.ToList().ForEach(parameter => sb.AppendFormat("%s = @%s", parameter));
            return sb.ToString();
        }
    }
}
