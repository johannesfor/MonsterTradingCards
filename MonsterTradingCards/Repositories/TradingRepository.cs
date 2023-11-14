using MonsterTradingCards.Contracts;
using MonsterTradingCards.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Repositories
{
    public class TradingRepository : ITradingRepository
    {
        private readonly string _connectionString;

        public TradingRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public void Add(Trading trading)
        {
            // This is not ideal, connection should stay open to allow a faster batch save mode
            // but for now it is ok
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"INSERT INTO trading (id, card_id, user_id, requirement_card_type, requirement_min_damage)
                                            VALUES (@id, @card_id, @user_id, @requirement_card_type, @requirement_min_damage)"
                    ;
                    command.AddParameterWithValue("id", DbType.Guid, trading.Id);
                    command.AddParameterWithValue("card_id", DbType.Guid, trading.CardToTrade);
                    command.AddParameterWithValue("user_id", DbType.Guid, trading.UserId);
                    command.AddParameterWithValue("requirement_card_type", DbType.Int32, trading.Type);
                    command.AddParameterWithValue("requirement_min_damage", DbType.Int32, trading.MinimumDamage);
                    command.ExecuteNonQuery();
                }
            }

        }

        public void Delete(Trading trading)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"DELETE FROM trading WHERE id = @id";

                    command.AddParameterWithValue("id", DbType.Guid, trading.Id);

                    command.ExecuteNonQuery();
                }
            }
        }

        public Trading Get(Guid id)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT id, card_id, user_id, requirement_card_type, requirement_min_damage
                                        FROM trading
                                        WHERE id = @id";

                    connection.Open();

                    var pFID = command.CreateParameter();
                    pFID.DbType = DbType.Guid;
                    pFID.ParameterName = "id";
                    pFID.Value = id;
                    command.Parameters.Add(pFID);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Trading()
                            {
                                Id = reader.GetGuid(0),
                                CardToTrade = reader.GetGuid(1),
                                UserId = reader.GetGuid(2),
                                Type = reader.GetInt32(3),
                                MinimumDamage = reader.GetInt32(4)
                            };
                        }
                    }
                }
            }
            return null;

        }

        public IEnumerable<Trading> GetAll()
        {
            List<Trading> result = new List<Trading>();
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"SELECT id, card_id, user_id, requirement_card_type, requirement_min_damage
                                        FROM trading";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Trading()
                            {
                                Id = reader.GetGuid(0),
                                CardToTrade = reader.GetGuid(1),
                                UserId = reader.GetGuid(2),
                                Type = reader.GetInt32(3),
                                MinimumDamage = reader.GetInt32(4)
                            });
                        }
                    }
                }
            }
            return result;
        }

        public Trading GetByCardId(Guid cardId)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT id, card_id, user_id, requirement_card_type, requirement_min_damage
                                        FROM trading
                                        WHERE card_id = @card_id";

                    connection.Open();

                    var pFID = command.CreateParameter();
                    pFID.DbType = DbType.Guid;
                    pFID.ParameterName = "card_id";
                    pFID.Value = cardId;
                    command.Parameters.Add(pFID);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Trading()
                            {
                                Id = reader.GetGuid(0),
                                CardToTrade = reader.GetGuid(1),
                                UserId = reader.GetGuid(2),
                                Type = reader.GetInt32(3),
                                MinimumDamage = reader.GetInt32(4)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void Update(Trading trading, params string[] parameters)
        {
            if (trading.Id == null)
                throw new ArgumentNullException("Id cannot be null");

            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"UPDATE trading
                                            SET " + this.ConvertAttributesToSetSQL(parameters) +
                                            "WHERE id = @id";

                    command.AddParameterWithValue("id", DbType.Guid, trading.Id);
                    command.AddParameterWithValue("card_id", DbType.Guid, trading.CardToTrade);
                    command.AddParameterWithValue("user_id", DbType.Guid, trading.UserId);
                    command.AddParameterWithValue("requirement_card_type", DbType.Int32, trading.Type);
                    command.AddParameterWithValue("requirement_min_damage", DbType.Int32, trading.MinimumDamage);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
