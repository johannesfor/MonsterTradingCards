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
    public class LootboxRepository : GenericRepository<Lootbox>, ILootboxRepository
    {
        public LootboxRepository(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<Lootbox> GetAllByUserId(Guid userId)
        {
            List<Lootbox> result = new List<Lootbox>();
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();

                    PropertyInfo[] properties = typeof(Lootbox).GetProperties().Where(property => GetColumnName(property.Name) != null).ToArray();
                    var fieldNames = string.Join(", ", properties.Select(property => GetColumnName(property.Name)));
                    command.CommandText = $"SELECT {fieldNames} FROM {GetEntityName()} WHERE user_id = @user_id";

                    var pFID = command.CreateParameter();
                    pFID.DbType = DbType.Guid;
                    pFID.ParameterName = "user_id";
                    pFID.Value = userId;
                    command.Parameters.Add(pFID);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(MapToObject(reader));
                        }
                    }
                }
            }
            return result;
        }
    }
}
