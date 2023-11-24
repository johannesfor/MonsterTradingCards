using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.CAQ.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Authorizer
{
    public class BattleWithRandomPlayerCommandAuthorizer : AbstractRequestAuthorizer<BattleWithRandomPlayerCommand>
    {
        public override void BuildPolicy(BattleWithRandomPlayerCommand request)
        {
            UseRequirement(new IsAllowedToBattleRequirement());
        }
    }
}
