using MediatR;
using MonsterTradingCards.CAQ.Lootbox;
using MonsterTradingCards.Contracts.Factory;
using MonsterTradingCards.Contracts.Repository;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Handler.Lootbox
{
    public class OpenLootboxCommandHandler : IRequestHandler<OpenLootboxCommand, Card>
    {
        private ILootboxRepository lootboxRepository;
        private ICardRepository cardRepository;
        private ICardFactory cardFactory;
        private IUserContext userContext;
        public OpenLootboxCommandHandler(ILootboxRepository lootboxRepository, IUserContext userContext, ICardRepository cardRepository, ICardFactory cardFactory)
        {
            this.lootboxRepository = lootboxRepository;
            this.userContext = userContext;
            this.cardRepository = cardRepository;
            this.cardFactory = cardFactory;
        }

        public async Task<Card> Handle(OpenLootboxCommand request, CancellationToken cancellationToken)
        {
            Models.Lootbox lootbox = lootboxRepository.Get(request.LootboxId);
            Card card = cardFactory.CreateRandomCardByRarity(lootbox.Rarity);
            card.UserId = userContext.User.Id;
            cardRepository.Add(card);
            lootboxRepository.Delete(request.LootboxId);
            return card;
        }
    }
}