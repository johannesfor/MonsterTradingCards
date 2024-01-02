using MediatR;
using MonsterTradingCards.CAQ.Battle;
using MonsterTradingCards.CAQ.Cards;
using MonsterTradingCards.Contracts.Factory;
using MonsterTradingCards.Contracts.Service;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Handler.Battle
{
    public class JoinBattleQueueCommandHandler : IRequestHandler<JoinBattleQueueCommand, IEnumerable<string>>
    {
        private IBattleService battleService;
        private IUserContext userContext;
        public JoinBattleQueueCommandHandler(IBattleService battleService, IUserContext userContext)
        {
            this.battleService = battleService;
            this.userContext = userContext;
        }

        public async Task<IEnumerable<string>> Handle(JoinBattleQueueCommand request, CancellationToken cancellationToken)
        {
            return await battleService.JoinQueueForBattle(userContext.User.Id.Value);
        }
    }
}