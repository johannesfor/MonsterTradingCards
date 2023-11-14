using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Models
{
    public class Card
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public double Damage { get; set; }
        public int ElementType { get; set; }
        public int CardType { get; set; }
        public Guid? UserId { get; set; }
        public bool IsInDeck { get; set; }
        public Guid? PackageId { get; set; }
    }
}
