using MonsterTradingCards.Models.Base;
using System.Data;

namespace MonsterTradingCards.Models
{
    [DbEntity("users")]
    public class User
    {
        [DbColumn("id", DbType.Guid, true)]
        public Guid? Id { get; set; }

        [DbColumn("username", DbType.String)]
        public string Username { get; set; }

        [DbColumn("password", DbType.String)]
        public string Password { get; set; }

        [DbColumn("bio", DbType.String)]
        public string Bio {  get; set; }

        [DbColumn("image", DbType.String)]
        public string Image { get; set; }

        [DbColumn("coins", DbType.Int32)]
        public int Coins { get; set; }

        [DbColumn("elo", DbType.Int32)]
        public int Elo {  get; set; }

        [DbColumn("played_games", DbType.Int32)]
        public int PlayedGames { get; set; }

        [DbColumn("name", DbType.String)]
        public string Name { get; set; }
    }
}
