using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.Contracts.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Handler
{
    public class CreateValidPackageRequirementHandler : IAuthorizationHandler<CreateValidPackageRequirement>
    {
        private readonly IUserContext userContext;

        public CreateValidPackageRequirementHandler(IUserContext userContext)
        {
            this.userContext = userContext;
        }
        public async Task<AuthorizationResult> Handle(CreateValidPackageRequirement requirement, CancellationToken cancellationToken = default)
        {
            if (userContext.User == null)
                return AuthorizationResult.Fail("You need to be logged in");

            if (!userContext.IsAdmin)
                return AuthorizationResult.Fail("You need to be admin");

            if (requirement.Cards.Count() != 5)
                return AuthorizationResult.Fail("A package consists of 5 cards");

            return AuthorizationResult.Succeed();
        }
    }
}
