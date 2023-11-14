using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.CAQ.Deck;
using MonsterTradingCards.CAQ.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Authorizer
{
    public class GetUserProfileQueryAuthorizer : AbstractRequestAuthorizer<GetUserProfileQuery>
    {
        public override void BuildPolicy(GetUserProfileQuery request)
        {
            UseRequirement(new IsAuthenticatedRequirement());
        }
    }
}
