﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Contracts
{
    public interface IBattleService
    {
        Task<IEnumerable<string>> JoinQueueForBattle(Guid userId);
    }
}
