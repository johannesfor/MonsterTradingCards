using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Contracts
{
    public interface ITradingRepository : IRepository<Trading>
    {
        Trading GetByCardId(Guid cardId);
    }
}
