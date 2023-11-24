using MediatR;
using MonsterTradingCards.CAQ.Admin;
using MonsterTradingCards.CAQ.Packages;
using MonsterTradingCards.Contracts;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Handler.Packages
{
    public class AquirePackageCommandHandler : IRequestHandler<AquirePackageCommand>
    {
        private IPackageRepository packageRepository;
        private ICardRepository cardRepository;
        private IUserRepository userRepository;
        private IUserContext userContext;
        public AquirePackageCommandHandler(IPackageRepository packageRepository, ICardRepository cardRepository, IUserRepository userRepository, IUserContext userContext)
        {
            this.packageRepository = packageRepository;
            this.cardRepository = cardRepository;
            this.userRepository = userRepository;
            this.userContext = userContext;
        }
        public async Task Handle(AquirePackageCommand request, CancellationToken cancellationToken)
        {
            Package packageToAquire = packageRepository.GetRandom();

            packageToAquire.Cards.ToList().ForEach(card =>
            {
                card.UserId = userContext.User.Id;
                card.PackageId = null;
                card.IsInDeck = false;
                cardRepository.Update(card, "user_id", "package_id", "is_in_deck");
            });

            packageRepository.Delete(packageToAquire);

            User userToUpdate = userContext.User;
            userToUpdate.Coins -= 5;
            userRepository.Update(userToUpdate, "coins");
        }
    }
}