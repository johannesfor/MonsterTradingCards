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
    public class IsAdminRequirementHandler : IAuthorizationHandler<IsAdminRequirement>
    {
        private readonly IUserContext userContext;

        public IsAdminRequirementHandler(IUserContext userContext)
        {
            this.userContext = userContext;
        }
        public async Task<AuthorizationResult> Handle(IsAdminRequirement requirement, CancellationToken cancellationToken = default)
        {
            if (userContext.User == null)
                return AuthorizationResult.Fail("Nicht authentifiziert");

            if (userContext.IsAdmin)
                return AuthorizationResult.Succeed();
            return AuthorizationResult.Fail();
        }
    }
}
