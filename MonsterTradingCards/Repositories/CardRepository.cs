using MonsterTradingCards.Contracts;
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
    public class CardRepository : GenericRepository<Card>, ICardRepository
    {
        public CardRepository(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<Card> GetAllByUserId(Guid userId, bool filterByIsInDeck)
        {
            List<Card> result = new List<Card>();
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();

                    PropertyInfo[] properties = typeof(Card).GetProperties().Where(property => GetColumnName(property.Name) != null).ToArray();
                    var fieldNames = string.Join(", ", properties.Select(property => GetColumnName(property.Name)));
                    command.CommandText = $"SELECT {fieldNames} FROM {GetEntityName()} WHERE user_id = @user_id";

                    if (filterByIsInDeck)
                        command.CommandText += " AND is_in_deck = true";

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
