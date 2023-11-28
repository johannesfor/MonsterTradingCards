using MonsterTradingCards.Models.Base;
using System.Data;

namespace MonsterTradingCards.Models
{
    [DbEntity("card")]
    public class Card
    {
        [DbColumn("id", DbType.Guid, true)]
        public Guid? Id { get; set; }

        [DbColumn("name", DbType.String)]
        public string Name { get; set; }

        [DbColumn("damage", DbType.Decimal)]
        public double Damage { get; set; }

        [DbColumn("element_type", DbType.Int32)]
        public ElementType ElementType { get; set; }

        [DbColumn("card_type", DbType.Int32)]
        public CardType CardType { get; set; }

        [DbColumn("user_id", DbType.Guid)]
        public Guid? UserId { get; set; }

        [DbColumn("is_in_deck", DbType.Boolean)]
        public bool IsInDeck { get; set; }

        [DbColumn("package_id", DbType.Guid)]
        public Guid? PackageId { get; set; }
    }
}
