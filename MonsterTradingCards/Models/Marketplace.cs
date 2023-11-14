using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Models
{
    public class Marketplace
    {
        public Guid? Id { get; set; }
        public Guid? CardId { get; set; }
        public int RequirementCardType { get; set; }
        public double RequirementMinDamage { get; set; }
    }
}
