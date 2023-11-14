using MediatR;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.CAQ.Deck
{
    public class GetDeckQuery : IRequest<IEnumerable<Card>>
    {
    }
}
