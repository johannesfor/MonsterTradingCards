using MonsterTradingCards.Contracts.Repository;
using MonsterTradingCards.Models;
using MonsterTradingCards.Repositories.Base;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(string connectionString) : base(connectionString)
        {
        }

        public User GetRandomUserWithValidDeck()
        {
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();

                    PropertyInfo[] properties = typeof(User).GetProperties().Where(property => GetColumnName(property.Name) != null).ToArray();
                    var fieldNames = string.Join(", ", properties.Select(property => GetColumnName(property.Name)));
                    command.CommandText = $@"SELECT u.{fieldNames} FROM {GetEntityName()} u 
                                        INNER JOIN (
                                            SELECT user_id, COUNT(*) AS card_count
                                            FROM card
                                            WHERE is_in_deck = true
                                            GROUP BY user_id
                                            HAVING COUNT(*) = 4) c ON u.id = c.user_id
                                        ORDER BY random() LIMIT 1";

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

        public User GetByUsername(string username)
        {
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();

                    PropertyInfo[] properties = typeof(User).GetProperties().Where(property => GetColumnName(property.Name) != null).ToArray();
                    var fieldNames = string.Join(", ", properties.Select(property => GetColumnName(property.Name)));
                    command.CommandText = $"SELECT {fieldNames} FROM {GetEntityName()} WHERE username = @username";

                    var pFID = command.CreateParameter();
                    pFID.DbType = DbType.String;
                    pFID.ParameterName = "username";
                    pFID.Value = username;
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
