using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MonsterTradingCards.Models
{
    public class Trading
    {
        public Guid? Id { get; set; }
        public Guid CardToTrade { get; set; }
        public Guid UserId { get; set; }
        public double MinimumDamage { get; set; }
        public CardType Type { get; set; }
    }
}
