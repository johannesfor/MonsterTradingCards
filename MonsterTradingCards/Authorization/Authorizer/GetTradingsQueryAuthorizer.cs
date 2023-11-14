using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.CAQ.Tradings;
using MonsterTradingCards.CAQ.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Authorizer
{
    public class GetTradingsQueryAuthorizer : AbstractRequestAuthorizer<GetTradingsQuery>
    {
        public override void BuildPolicy(GetTradingsQuery request)
        {
            UseRequirement(new IsAuthenticatedRequirement());
        }
    }
}
