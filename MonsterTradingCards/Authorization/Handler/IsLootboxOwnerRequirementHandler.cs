using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.Contracts.Factory;
using MonsterTradingCards.Contracts.Repository;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Handler
{
    public class IsLootboxOwnerRequirementHandler : IAuthorizationHandler<IsLootboxOwnerRequirement>
    {
        private readonly IUserContext userContext;
        private readonly ILootboxRepository lootboxRepository;

        public IsLootboxOwnerRequirementHandler(IUserContext userContext, ILootboxRepository lootboxRepository)
        {
            this.userContext = userContext;
            this.lootboxRepository = lootboxRepository;
        }
        public async Task<AuthorizationResult> Handle(IsLootboxOwnerRequirement requirement, CancellationToken cancellationToken = default)
        {
            if (userContext.User == null)
                return AuthorizationResult.Fail("You need to be logged in");

            Lootbox lootbox = lootboxRepository.Get(requirement.LootboxId);
            if (lootbox.UserId != userContext.User.Id)
                return AuthorizationResult.Fail("User is not owner of lootbox");

            return AuthorizationResult.Succeed();
        }
    }
}
