using MonsterTradingCards.Models.Base;
using System.Data;

namespace MonsterTradingCards.Models
{
    [DbEntity("package")]
    public class Package
    {
        [DbColumn("id", DbType.Guid, true)]
        public Guid? Id { get; set; }
        public List<Card> Cards { get; set;}
    }
}
