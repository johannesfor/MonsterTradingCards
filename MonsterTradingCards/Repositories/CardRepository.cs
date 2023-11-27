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
    public class CardRepository : ICardRepository
    {
        private readonly string _connectionString;

        public CardRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public void Add(Card card)
        {
            // This is not ideal, connection should stay open to allow a faster batch save mode
            // but for now it is ok
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"INSERT INTO card (id, name, damage, element_type, card_type, user_id, is_in_deck, package_id)
                                            VALUES (@id, @name, @damage, @element_type, @card_type, @user_id, @is_in_deck, @package_id)"
                    ;
                    command.AddParameterWithValue("id", DbType.Guid, card.Id);
                    command.AddParameterWithValue("name", DbType.String, card.Name);

                    command.AddParameterWithValue("damage", DbType.Double, card.Damage);
                    command.AddParameterWithValue("element_type", DbType.Int32, (int) card.ElementType);
                    command.AddParameterWithValue("card_type", DbType.Int32, (int) card.CardType);

                    command.AddParameterWithValue("user_id", DbType.Guid, card.UserId);
                    command.AddParameterWithValue("is_in_deck", DbType.Boolean, card.IsInDeck);
                    command.AddParameterWithValue("package_id", DbType.Guid, card.PackageId);
                    command.ExecuteNonQuery();
                }
            }

        }

        public void Delete(Card card)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"DELETE FROM card WHERE id = @id";

                    command.AddParameterWithValue("id", DbType.Guid, card.Id);

                    command.ExecuteNonQuery();
                }
            }
        }

        public Card Get(Guid id)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT id, name, damage, element_type, card_type, user_id, is_in_deck, package_id
                                        FROM card
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
                            return new Card()
                            {
                                Id = reader.GetGuid(0),
                                Name = reader.GetString(1),
                                Damage = reader.GetInt32(2),
                                ElementType = (ElementType) reader.GetInt32(3),
                                CardType = (CardType) reader.GetInt32(4),
                                UserId = reader.GetGuid(5),
                                IsInDeck = reader.GetBoolean(6),
                                PackageId = reader.GetNullableGuid(7),
                            };
                        }
                    }
                }
            }
            return null;

        }

        public IEnumerable<Card> GetAll()
        {
            List<Card> result = new List<Card>();
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"SELECT id, name, damage, element_type, card_type, user_id, is_in_deck, package_id
                                        FROM card";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Card()
                            {
                                Id = reader.GetGuid(0),
                                Name = reader.GetString(1),
                                Damage = reader.GetInt32(2),
                                ElementType = (ElementType) reader.GetInt32(3),
                                CardType = (CardType) reader.GetInt32(4),
                                UserId = reader.GetNullableGuid(5),
                                IsInDeck = reader.GetBoolean(6),
                                PackageId = reader.GetNullableGuid(7),
                            });
                        }
                    }
                }
            }
            return result;
        }

        public IEnumerable<Card> GetAllByUserId(Guid userId, bool filterByIsInDeck)
        {
            List<Card> result = new List<Card>();
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"SELECT id, name, damage, element_type, card_type, user_id, is_in_deck, package_id
                                        FROM card
                                        WHERE user_id = @user_id";

                    if (filterByIsInDeck)
                        command.CommandText += " and is_in_deck = true";

                    var pFID = command.CreateParameter();
                    pFID.DbType = DbType.Guid;
                    pFID.ParameterName = "user_id";
                    pFID.Value = userId;
                    command.Parameters.Add(pFID);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Card()
                            {
                                Id = reader.GetGuid(0),
                                Name = reader.GetString(1),
                                Damage = reader.GetInt32(2),
                                ElementType = (ElementType)reader.GetInt32(3),
                                CardType = (CardType)reader.GetInt32(4),
                                UserId = reader.GetNullableGuid(5),
                                IsInDeck = reader.GetBoolean(6),
                                PackageId = reader.GetNullableGuid(7),
                            });
                        }
                    }
                }
            }
            return result;
        }

        public void Update(Card card, params string[] parameters)
        {
            if (card.Id == null)
                throw new ArgumentNullException("Id cannot be null");

            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"UPDATE card
                                            SET " + this.ConvertAttributesToSetSQL(parameters) +
                                            "WHERE id = @id";

                    command.AddParameterWithValue("id", DbType.Guid, card.Id);
                    command.AddParameterWithValue("name", DbType.String, card.Name);

                    command.AddParameterWithValue("damage", DbType.Double, card.Damage);
                    command.AddParameterWithValue("element_type", DbType.Int32, (int) card.ElementType);
                    command.AddParameterWithValue("card_type", DbType.Int32, (int) card.CardType);

                    command.AddParameterWithValue("user_id", DbType.Guid, card.UserId);
                    command.AddParameterWithValue("is_in_deck", DbType.Boolean, card.IsInDeck);
                    command.AddParameterWithValue("package_id", DbType.Guid, card.PackageId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
