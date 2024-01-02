using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.CAQ.Lootbox;
using MonsterTradingCards.CAQ.Tradings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Authorizer
{
    public class OpenLootboxCommandAuthorizer : AbstractRequestAuthorizer<OpenLootboxCommand>
    {
        public override void BuildPolicy(OpenLootboxCommand request)
        {
            UseRequirement(new IsLootboxOwnerRequirement() { LootboxId = request.LootboxId });
        }
    }
}
