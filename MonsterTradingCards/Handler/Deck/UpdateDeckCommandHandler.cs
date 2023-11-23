﻿using MediatR;
using MonsterTradingCards.CAQ.Deck;
using MonsterTradingCards.Contracts;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Handler.Deck
{
    public class UpdateDeckCommandHandler : IRequestHandler<UpdateDeckCommand>
    {
        private ICardRepository cardRepository;
        private IUserContext userContext;
        public UpdateDeckCommandHandler(ICardRepository cardRepository, IUserContext userContext)
        {
            this.cardRepository = cardRepository;
            this.userContext = userContext;
        }

        public async Task Handle(UpdateDeckCommand request, CancellationToken cancellationToken)
        {
            cardRepository.GetAllByUserId(userContext.User.Id.Value, true).ToList().ForEach(card =>
            {
                card.IsInDeck = false;
                cardRepository.Update(card, "is_in_deck");
            });

            request.CardIds.ToList().ForEach(cardId =>
            {
                Card toUpdate = new Card()
                {
                    Id = cardId,
                    IsInDeck = true
                };
                cardRepository.Update(toUpdate, "is_in_deck");
            });
        }
    }
}