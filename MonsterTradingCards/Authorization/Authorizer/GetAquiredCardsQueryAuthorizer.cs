using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.CAQ.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Authorizer
{
    public class GetAquiredCardsQueryAuthorizer : AbstractRequestAuthorizer<GetAquiredCardsQuery>
    {
        public override void BuildPolicy(GetAquiredCardsQuery request)
        {
            UseRequirement(new IsAuthenticatedRequirement());
        }
    }
}
