using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.Contracts;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Handler
{
    public class IsAllowedToAquirePackageRequirementHandler : IAuthorizationHandler<IsAllowedToAquirePackageRequirement>
    {
        private readonly IUserContext userContext;
        private readonly IPackageRepository packageRepository;

        public IsAllowedToAquirePackageRequirementHandler(IUserContext userContext, IPackageRepository packageRepository)
        {
            this.userContext = userContext;
            this.packageRepository = packageRepository;
        }
        public async Task<AuthorizationResult> Handle(IsAllowedToAquirePackageRequirement requirement, CancellationToken cancellationToken = default)
        {
            if (userContext.User == null)
                return AuthorizationResult.Fail("You need to be logged in");

            if (userContext.User.Coins < 5)
                return AuthorizationResult.Fail("Not enough coins");

            IEnumerable<Package> allPackages = packageRepository.GetAll();
            if (!allPackages.Any())
                return AuthorizationResult.Fail("There are no packages available which can be purchased");
            return AuthorizationResult.Succeed();
        }
    }
}
