using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Handler
{
    public class IsAuthenticatedRequirementHandler : IAuthorizationHandler<IsAuthenticatedRequirement>
    {
        private readonly IUserContext userContext;

        public IsAuthenticatedRequirementHandler(IUserContext userContext)
        {
            this.userContext = userContext;
        }
        public async Task<AuthorizationResult> Handle(IsAuthenticatedRequirement requirement, CancellationToken cancellationToken = default)
        {
            if (userContext.User != null)
                return AuthorizationResult.Succeed();
            return AuthorizationResult.Fail("You need to be logged in");
        }
    }
}
