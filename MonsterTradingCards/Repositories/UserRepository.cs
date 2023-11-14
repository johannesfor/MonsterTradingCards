using MonsterTradingCards.Contracts;
using MonsterTradingCards.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public void Add(User user)
        {
            // This is not ideal, connection should stay open to allow a faster batch save mode
            // but for now it is ok
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"INSERT INTO users (id, username, password, bio, image, coins, elo, played_games, name)
                                            VALUES (@id, @username, @password, @bio, @image, @coins, @elo, @played_games, @name)"
                    ;
                    command.AddParameterWithValue("id", DbType.Guid, user.Id);
                    command.AddParameterWithValue("username", DbType.String, user.Username);
                    command.AddParameterWithValue("password", DbType.String, user.Password);
                    command.AddParameterWithValue("bio", DbType.String, user.Bio);
                    command.AddParameterWithValue("image", DbType.String, user.Image);
                    command.AddParameterWithValue("name", DbType.String, user.Name);

                    command.AddParameterWithValue("coins", DbType.Int32, user.Coins);
                    command.AddParameterWithValue("elo", DbType.Int32, user.Elo);
                    command.AddParameterWithValue("played_games", DbType.Int32, user.PlayedGames);
                    command.ExecuteNonQuery();
                }
            }

        }

        public void Delete(User user)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"DELETE FROM users WHERE id = @id";

                    command.AddParameterWithValue("id", DbType.Guid, user.Id);

                    command.ExecuteNonQuery();
                }
            }
        }

        public User Get(Guid id)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT id, username, password, bio, image, coins, elo, played_games, name
                                        FROM users
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
                            return new User()
                            {
                                Id = reader.GetGuid(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                Bio = reader.GetNullableString(3),
                                Image = reader.GetNullableString(4),
                                Coins = reader.GetInt32(5),
                                Elo = reader.GetInt32(6),
                                PlayedGames = reader.GetInt32(7),
                                Name = reader.GetNullableString(8)
                            };
                        }
                    }
                }
            }
            return null;

        }

        public IEnumerable<User> GetAll()
        {
            List<User> result = new List<User>();
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"SELECT id, username, password, bio, image, coins, elo, played_games, name
                                        FROM users";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new User()
                            {
                                Id = reader.GetGuid(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                Bio = reader.GetNullableString(3),
                                Image = reader.GetNullableString(4),
                                Coins = reader.GetInt32(5),
                                Elo = reader.GetInt32(6),
                                PlayedGames = reader.GetInt32(7),
                                Name = reader.GetNullableString(8)
                            });
                        }
                    }
                }
            }
            return result;
        }

        public User GetByUsername(string username)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT id, username, password, bio, image, coins, elo, played_games, name
                                        FROM users
                                        WHERE username = @username";

                    connection.Open();

                    var pFID = command.CreateParameter();
                    pFID.DbType = DbType.String;
                    pFID.ParameterName = "username";
                    pFID.Value = username;
                    command.Parameters.Add(pFID);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User()
                            {
                                Id = reader.GetGuid(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                Bio = reader.GetNullableString(3),
                                Image = reader.GetNullableString(4),
                                Coins = reader.GetInt32(5),
                                Elo = reader.GetInt32(6),
                                PlayedGames = reader.GetInt32(7),
                                Name = reader.GetNullableString(8)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void Update(User user, params string[] parameters)
        {
            if (user.Id == null)
                throw new ArgumentNullException("Id cannot be null");

            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"UPDATE users
                                            SET " + this.ConvertAttributesToSetSQL(parameters) +
                                            "WHERE id = @id";

                    command.AddParameterWithValue("id", DbType.Guid, user.Id);
                    command.AddParameterWithValue("username", DbType.String, user.Username);
                    command.AddParameterWithValue("password", DbType.String, user.Password);
                    command.AddParameterWithValue("bio", DbType.String, user.Bio);
                    command.AddParameterWithValue("image", DbType.String, user.Image);
                    command.AddParameterWithValue("name", DbType.String, user.Name);

                    command.AddParameterWithValue("coins", DbType.Int32, user.Coins);
                    command.AddParameterWithValue("elo", DbType.Int32, user.Elo);
                    command.AddParameterWithValue("played_games", DbType.Int32, user.PlayedGames);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
