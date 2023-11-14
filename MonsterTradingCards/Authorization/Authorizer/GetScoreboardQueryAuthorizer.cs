using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.CAQ.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Authorizer
{
    public class GetScoreboardQueryAuthorizer : AbstractRequestAuthorizer<GetScoreboardQuery>
    {
        public override void BuildPolicy(GetScoreboardQuery request)
        {
            UseRequirement(new IsAuthenticatedRequirement());
        }
    }
}
