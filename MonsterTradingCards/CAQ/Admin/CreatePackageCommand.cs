using MediatR;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.CAQ.Admin
{
    public class CreatePackageCommand : IRequest
    {
        public IEnumerable<Card> Cards { get; set; }
    }
}
