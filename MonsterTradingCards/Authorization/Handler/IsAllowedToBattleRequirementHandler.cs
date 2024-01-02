using MediatR.Behaviors.Authorization;
using MonsterTradingCards.Authorization.Requirement;
using MonsterTradingCards.Contracts.Factory;
using MonsterTradingCards.Contracts.Repository;
using MonsterTradingCards.Contracts.Service;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Authorization.Handler
{
    public class IsAllowedToBattleRequirementHandler : IAuthorizationHandler<IsAllowedToBattleRequirement>
    {
        private readonly IUserContext userContext;
        private readonly ICardRepository cardRepository;
        private readonly IBattleService battleService;

        public IsAllowedToBattleRequirementHandler(IUserContext userContext, ICardRepository cardRepository, IBattleService battleService)
        {
            this.userContext = userContext;
            this.cardRepository = cardRepository;
            this.battleService = battleService;
        }
        public async Task<AuthorizationResult> Handle(IsAllowedToBattleRequirement requirement, CancellationToken cancellationToken = default)
        {
            if (userContext.User == null)
                return AuthorizationResult.Fail("You need to be logged in");

            IEnumerable<Card> deckOfUser = cardRepository.GetAllByUserId(userContext.User.Id.Value, true);

            if (!deckOfUser.Any())
                return AuthorizationResult.Fail("The user does not have a configured deck and therefore cannot battle");

            if (battleService.CheckIfUserIsAlreadyInQueue(userContext.User.Id.Value))
                return AuthorizationResult.Fail("You are already in queue");

            return AuthorizationResult.Succeed();
        }
    }
}
