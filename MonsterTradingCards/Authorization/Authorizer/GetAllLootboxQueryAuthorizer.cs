using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.CAQ.Cards;
using MonsterTradingCards.CAQ.Lootbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Authorizer
{
    public class GetAllLootboxQueryAuthorizer : AbstractRequestAuthorizer<GetAllLootboxQuery>
    {
        public override void BuildPolicy(GetAllLootboxQuery request)
        {
            UseRequirement(new IsAuthenticatedRequirement());
        }
    }
}
