using MediatR;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.CAQ.Cards
{
    public class GetAquiredCardsQuery : IRequest<IEnumerable<Card>>
    {
    }
}
