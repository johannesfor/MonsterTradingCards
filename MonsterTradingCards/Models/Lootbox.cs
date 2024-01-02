using MonsterTradingCards.Models.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Models
{
    [DbEntity("lootbox")]
    public class Lootbox
    {
        [DbColumn("id", DbType.Guid, true)]
        public Guid? Id { get; set; }

        [DbColumn("rarity", DbType.Int32)]
        public int Rarity { get; set; }

        [DbColumn("user_id", DbType.Guid)]
        public Guid? UserId { get; set; }
    }
}
