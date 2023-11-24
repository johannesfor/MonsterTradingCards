using MediatR;
using MonsterTradingCards.CAQ.Admin;
using MonsterTradingCards.CAQ.Users;
using MonsterTradingCards.Contracts;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Handler.Admin
{
    public class CreatePackageCommandHandler : IRequestHandler<CreatePackageCommand>
    {
        private IPackageRepository packageRepository;
        private ICardRepository cardRepository;
        public CreatePackageCommandHandler(IPackageRepository packageRepository, ICardRepository cardRepository)
        {
            this.packageRepository = packageRepository;
            this.cardRepository = cardRepository;
        }
        public async Task Handle(CreatePackageCommand request, CancellationToken cancellationToken)
        {
            Package package = new Package() { Id = Guid.NewGuid() };
            packageRepository.Add(package);

            request.Cards.ToList().ForEach(card =>
            {
                card.PackageId = package.Id;

                if (card.Name.EndsWith("Spell"))
                    card.CardType = CardType.Spell;
                else
                    card.CardType = CardType.Monster;

                if (card.Name.StartsWith("Fire"))
                    card.ElementType = ElementType.Fire;
                else if (card.Name.StartsWith("Water"))
                    card.ElementType = ElementType.Water;
                else
                    card.ElementType = ElementType.Normal;

                cardRepository.Add(card);
            });
        }
    }
}