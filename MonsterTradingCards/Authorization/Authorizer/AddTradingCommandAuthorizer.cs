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
    public class AddTradingCommandAuthorizer : AbstractRequestAuthorizer<AddTradingCommand>
    {
        public override void BuildPolicy(AddTradingCommand request)
        {
            UseRequirement(new IsAddTradeValidRequirement() { Trading = request.Trading });
        }
    }
}
