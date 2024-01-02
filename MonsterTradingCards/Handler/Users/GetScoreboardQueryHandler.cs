using MediatR;
using MonsterTradingCards.CAQ.Users;
using MonsterTradingCards.Contracts.Repository;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Handler.Users
{
    public class GetScoreboardQueryHandler : IRequestHandler<GetScoreboardQuery, IEnumerable<Tuple<string, int>>>
    {
        private IUserRepository userRepository;
        public GetScoreboardQueryHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<IEnumerable<Tuple<string, int>>> Handle(GetScoreboardQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<User> users = userRepository.GetAll();
            List<Tuple<string, int>> scores = new List<Tuple<string, int>>();

            foreach (User user in users)
            {
                scores.Add(new Tuple<string, int>(user.Username, user.Elo));
            }

            return scores.OrderBy(x => x.Item2);
        }
    }
}