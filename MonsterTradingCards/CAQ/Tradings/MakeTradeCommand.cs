using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.CAQ.Tradings
{
    public class MakeTradeCommand : IRequest
    {
        public Guid TradeId { get; set; }
        public Guid CardId { get; set; }
    }
}
