using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Contracts.Repository
{
    public interface ICardRepository : IRepository<Card>
    {
        IEnumerable<Card> GetAllByUserId(Guid userId, bool filterByIsInDeck);
    }
}
