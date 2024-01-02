using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Contracts.Factory
{
    public interface ICardFactory
    {
        Card CreateRandomCardByRarity(int rarity);
    }
}
