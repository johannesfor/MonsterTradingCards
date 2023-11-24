﻿using MediatR.Behaviors.Authorization;
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
    public class IsAllowedToBattleRequirementHandler : IAuthorizationHandler<IsAllowedToBattleRequirement>
    {
        private readonly IUserContext userContext;
        private readonly ICardRepository cardRepository;

        public IsAllowedToBattleRequirementHandler(IUserContext userContext, ICardRepository cardRepository)
        {
            this.userContext = userContext;
            this.cardRepository = cardRepository;
        }
        public async Task<AuthorizationResult> Handle(IsAllowedToBattleRequirement requirement, CancellationToken cancellationToken = default)
        {
            if (userContext.User == null)
                return AuthorizationResult.Fail();

            IEnumerable<Card> deckOfUser = cardRepository.GetAllByUserId(userContext.User.Id.Value, true);

            if (!deckOfUser.Any())
                return AuthorizationResult.Fail("Der User hat kein konfiguriertes Deck und kann somit nicht batteln");

            return AuthorizationResult.Succeed();
        }
    }
}
