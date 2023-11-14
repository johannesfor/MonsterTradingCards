using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Models
{
    public class Package
    {
        public Guid? Id { get; set; }
        public List<Card> Cards { get; set;}
    }
}
