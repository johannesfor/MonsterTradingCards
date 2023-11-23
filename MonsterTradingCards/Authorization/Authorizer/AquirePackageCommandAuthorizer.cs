using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.CAQ.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Authorizer
{
    public class AquirePackageCommandAuthorizer : AbstractRequestAuthorizer<AquirePackageCommand>
    {
        public override void BuildPolicy(AquirePackageCommand request)
        {
            UseRequirement(new IsAllowedToAquirePackageRequirement());
        }
    }
}
