using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.CAQ.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Authorizer
{
    public class CreatePackageCommandAuthorizer : AbstractRequestAuthorizer<CreatePackageCommand>
    {
        public override void BuildPolicy(CreatePackageCommand request)
        {
            UseRequirement(new IsAdminRequirement());
        }
    }
}
