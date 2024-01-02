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
    public class PackageRepository : GenericRepository<Package>, IPackageRepository
    {
        public PackageRepository(string connectionString) : base(connectionString)
        {
        }

        public Package GetRandom()
        {
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();

                    command.CommandText = @"SELECT p.id, c.id
                                        FROM package p
                                        LEFT JOIN card c ON p.id = c.package_id
                                        WHERE p.id = (SELECT id FROM package LIMIT 1)";

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
    }
}
