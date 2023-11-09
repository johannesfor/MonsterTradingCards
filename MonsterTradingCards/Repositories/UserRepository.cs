﻿using FHTW.Swen1.Playground;
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

                    command.AddParameterWithValue("fid", DbType.String, point.FId);
                    command.AddParameterWithValue("objectId", DbType.Int32, point.ObjectId);
                    command.AddParameterWithValue("shape", DbType.String, point.Shape);
                    command.AddParameterWithValue("anlName", DbType.String, point.AnlName);

                    // if (point.Bezirk.HasValue)
                    command.AddParameterWithValue("bezirk", DbType.Int32, point.Bezirk);

                    command.AddParameterWithValue("spielplatzDetail", DbType.String, point.SpielplatzDetail);
                    command.AddParameterWithValue("typDetail", DbType.String, point.TypDetail);
                    command.AddParameterWithValue("seAnnoCadData", DbType.String, point.SeAnnoCadData);
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
