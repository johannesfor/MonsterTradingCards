using MediatR;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.CAQ.Deck
{
    public class UpdateDeckCommand : IRequest
    {
        public IEnumerable<Guid> CardIds { get; set; }
    }
}
