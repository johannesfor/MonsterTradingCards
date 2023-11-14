using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.CAQ.Tradings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Authorizer
{
    public class RemoveTradingCommandAuthorizer : AbstractRequestAuthorizer<RemoveTradingCommand>
    {
        public override void BuildPolicy(RemoveTradingCommand request)
        {
            UseRequirement(new IsTradeOwnerRequirement() { TradeId = request.TradeId });
        }
    }
}
