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
    public class UserRepository : IRepository<User>
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
                    command.CommandText = @"INSERT INTO users (id, username, password, bio, image, coins, elo, played_games)
                                            VALUES (@fid, @objectId, @shape, @anlName, @bezirk, @spielplatzDetail, @typDetail, @seAnnoCadData)"
                    ;
                    command.AddParameterWithValue("id", DbType.Guid, user.Id);
                    command.AddParameterWithValue("username", DbType.String, user.Username);
                    command.AddParameterWithValue("password", DbType.String, user.Password);
                    command.AddParameterWithValue("bio", DbType.String, user.Bio);
                    command.AddParameterWithValue("image", DbType.String, user.Image);

                    command.AddParameterWithValue("coins", DbType.Int32, user.Coins);
                    command.AddParameterWithValue("elo", DbType.Int32, user.Elo);
                    command.AddParameterWithValue("played_games", DbType.Int32, user.PlayedGames);
                    command.ExecuteNonQuery();
                }
            }

        }

        public void Delete(User t)
        {
            throw new NotImplementedException();
        }

        public User Get(Guid id)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT id, username, password, bio, image, coins, elo, played_games
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
                                Bio = reader.GetString(3),
                                Image = reader.GetString(4),
                                Coins = reader.GetInt32(5),
                                Elo = reader.GetInt32(6),
                                PlayedGames = reader.GetInt32(7)
                            };
                        }
                    }
                }
            }
            return null;

        }

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(User t, string[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
