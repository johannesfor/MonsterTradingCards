using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.CAQ.Cards;
using MonsterTradingCards.CAQ.Deck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Authorizer
{
    public class GetDeckQueryAuthorizer : AbstractRequestAuthorizer<GetDeckQuery>
    {
        public override void BuildPolicy(GetDeckQuery request)
        {
            UseRequirement(new IsAuthenticatedRequirement());
        }
    }
}
