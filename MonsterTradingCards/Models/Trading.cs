using MonsterTradingCards.Models.Base;
using System.Data;

namespace MonsterTradingCards.Models
{
    [DbEntity("trading")]
    public class Trading
    {
        [DbColumn("id", DbType.Guid, true)]
        public Guid? Id { get; set; }

        [DbColumn("card_id", DbType.Guid)]
        public Guid CardToTrade { get; set; }

        [DbColumn("user_id", DbType.Guid)]
        public Guid UserId { get; set; }

        [DbColumn("requirement_min_damage", DbType.Decimal)]
        public double MinimumDamage { get; set; }

        [DbColumn("requirement_card_type", DbType.Int32)]
        public CardType Type { get; set; }
    }
}
