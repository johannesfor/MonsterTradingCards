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
    public class IsUsernameEqualToUserContextRequirementHandler : IAuthorizationHandler<IsUsernameEqualToUserContextRequirement>
    {
        private readonly IUserContext userContext;

        public IsUsernameEqualToUserContextRequirementHandler(IUserContext userContext)
        {
            this.userContext = userContext;
        }
        public async Task<AuthorizationResult> Handle(IsUsernameEqualToUserContextRequirement requirement, CancellationToken cancellationToken = default)
        {
            if (userContext.User != null && userContext.User.Username == requirement.Username)
                return AuthorizationResult.Succeed();
            return AuthorizationResult.Fail();
        }
    }
}
