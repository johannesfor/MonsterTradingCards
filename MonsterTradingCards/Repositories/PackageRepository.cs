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
    public class PackageRepository : IPackageRepository
    {
        private readonly string _connectionString;

        public PackageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public void Add(Package package)
        {
            // This is not ideal, connection should stay open to allow a faster batch save mode
            // but for now it is ok
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"INSERT INTO package (id)
                                            VALUES (@id)"
                    ;
                    command.AddParameterWithValue("id", DbType.Guid, package.Id);
                    command.ExecuteNonQuery();
                }
            }

        }

        public void Delete(Package package)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"DELETE FROM package WHERE id = @id";

                    command.AddParameterWithValue("id", DbType.Guid, package.Id);

                    command.ExecuteNonQuery();
                }
            }
        }

        public Package Get(Guid id)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT p.id, c.id
                                        FROM package p
                                        LEFT JOIN card c ON p.id = c.package_id
                                        WHERE p.id = @id";

                    connection.Open();

                    var pFID = command.CreateParameter();
                    pFID.DbType = DbType.Guid;
                    pFID.ParameterName = "id";
                    pFID.Value = id;
                    command.Parameters.Add(pFID);

                    Package package = null;
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if(package == null)
                            {
                                package = new Package()
                                {
                                    Id = reader.GetGuid(0),
                                    Cards = new List<Card>()
                                };
                            }

                            Guid cardId = reader.GetGuid(1);
                            if (cardId != Guid.Empty)
                            {
                                package.Cards.Append(new Card() { Id = cardId });
                            }
                        }
                    }
                }
            }
            return null;
        }

        public IEnumerable<Package> GetAll()
        {
            List<Package> result = new List<Package>();
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"SELECT id
                                        FROM package";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Package()
                            {
                                Id = reader.GetGuid(0),
                            });
                        }
                    }
                }
            }
            return result;
        }

        public Package GetRandom()
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT p.id, c.id
                                        FROM package p
                                        LEFT JOIN card c ON p.id = c.package_id
                                        WHERE p.id = (SELECT id FROM package LIMIT 1)";

                    connection.Open();

                    Package package = null;
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (package == null)
                            {
                                package = new Package()
                                {
                                    Id = reader.GetGuid(0),
                                    Cards = new List<Card>()
                                };
                            }

                            Guid cardId = reader.GetGuid(1);
                            if (cardId != Guid.Empty)
                            {
                                package.Cards.Add(new Card() { Id = cardId });
                            }
                        }
                    }
                    return package;
                }
            }
            return null;
        }

        public void Update(Package package, params string[] parameters)
        {
            if (package.Id == null)
                throw new ArgumentNullException("Id cannot be null");

            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"UPDATE package
                                            SET " + this.ConvertAttributesToSetSQL(parameters) +
                                            "WHERE id = @id";

                    command.AddParameterWithValue("id", DbType.Guid, package.Id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
