using MonsterTradingCards.Contracts.Factory;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Factory
{
    public class CardFactory : ICardFactory
    {

        public Card CreateRandomCardByRarity(int rarity)
        {
            Random rd = new Random();
            List<string> names = new List<string>() { "Dragon", "Ork", "Spell", "Goblin", "Elf", "Knight" };
            Array elementTypes = Enum.GetValues(typeof(ElementType));
            Array cardTypes = Enum.GetValues(typeof(CardType));

            return new Card()
            {
                Id = Guid.NewGuid(),
                Name = names.RandomElement(),
                Damage = rd.Next(10) * (rarity+1),
                ElementType = (ElementType) elementTypes.GetValue(rd.Next(elementTypes.Length)),
                CardType = (CardType)cardTypes.GetValue(rd.Next(cardTypes.Length)),
                IsInDeck = false,
            };
        }
    }
}
