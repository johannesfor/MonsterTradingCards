using MediatR;
using MonsterTradingCards.CAQ.Deck;
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
    public class GetAllLootboxQueryHandler : IRequestHandler<GetAllLootboxQuery, IEnumerable<Models.Lootbox>>
    {
        private ILootboxRepository lootboxRepository;
        private IUserContext userContext;
        public GetAllLootboxQueryHandler(ILootboxRepository lootboxRepository, IUserContext userContext)
        {
            this.lootboxRepository = lootboxRepository;
            this.userContext = userContext;
        }

        public async Task<IEnumerable<Models.Lootbox>> Handle(GetAllLootboxQuery request, CancellationToken cancellationToken)
        {
            return lootboxRepository.GetAllByUserId(userContext.User.Id.Value);
        }
    }
}