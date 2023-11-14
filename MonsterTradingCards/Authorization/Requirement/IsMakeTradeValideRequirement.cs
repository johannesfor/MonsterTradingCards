using MediatR.Behaviors.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Requirement
{
    public class IsMakeTradeValideRequirement : IAuthorizationRequirement
    {
        public Guid TradeId { get; set; }
        public Guid CardId { get; set; }
    }
}
