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
    public class MakeTradeCommandAuthorizer : AbstractRequestAuthorizer<MakeTradeCommand>
    {
        public override void BuildPolicy(MakeTradeCommand request)
        {
            UseRequirement(new IsMakeTradeValideRequirement()
            {
                CardId = request.CardId,
                TradeId = request.TradeId
            });
        }
    }
}
