using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.CAQ.Users
{
    public class GetScoreboardQuery : IRequest<IEnumerable<Tuple<string, int>>>
    {
    }
}
