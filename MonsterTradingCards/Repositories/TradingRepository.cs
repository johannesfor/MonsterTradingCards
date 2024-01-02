using MonsterTradingCards.Contracts.Repository;
using MonsterTradingCards.Models;
using MonsterTradingCards.Repositories.Base;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Repositories
{
    public class TradingRepository : GenericRepository<Trading>, ITradingRepository
    {
        public TradingRepository(string connectionString) : base(connectionString)
        {
        }

        public Trading GetByCardId(Guid cardId)
        {
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();

                    PropertyInfo[] properties = typeof(Trading).GetProperties().Where(property => GetColumnName(property.Name) != null).ToArray();
                    var fieldNames = string.Join(", ", properties.Select(property => GetColumnName(property.Name)));
                    command.CommandText = $"SELECT {fieldNames} FROM {GetEntityName()} WHERE card_id = @card_id";

                    var pFID = command.CreateParameter();
                    pFID.DbType = DbType.Guid;
                    pFID.ParameterName = "card_id";
                    pFID.Value = cardId;
                    command.Parameters.Add(pFID);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapToObject(reader);
                        }
                    }
                }
            }
            return null;
        }
    }
}
