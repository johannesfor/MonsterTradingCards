using MediatR;
using MonsterTradingCards.CAQ.Battle;
using MonsterTradingCards.CAQ.Cards;
using MonsterTradingCards.Contracts;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Handler.Battle
{
    public class BattleWithRandomPlayerCommandHandler : IRequestHandler<BattleWithRandomPlayerCommand, IEnumerable<string>>
    {
        private ICardRepository cardRepository;
        private IUserContext userContext;
        private IUserRepository userRepository;
        public BattleWithRandomPlayerCommandHandler(ICardRepository cardRepository, IUserContext userContext, IUserRepository userRepository)
        {
            this.cardRepository = cardRepository;
            this.userContext = userContext;
            this.userRepository = userRepository;
        }

        public async Task<IEnumerable<string>> Handle(BattleWithRandomPlayerCommand request, CancellationToken cancellationToken)
        {
            List<string> battleLog = new List<string>();

            User enemy = userRepository.GetRandomUserWithValidDeck();
            if (enemy == null)
            {
                battleLog.Add("No enemy found");
                return battleLog;
            }
            battleLog.Add(String.Format("Your enemy is {0}", enemy.Name));

            List<Card> cardsOfEnemy = cardRepository.GetAllByUserId(enemy.Id.Value, true).ToList();
            List<Card> cardsOfUser = cardRepository.GetAllByUserId(userContext.User.Id.Value, true).ToList();

            int roundCount = 1;

            while (roundCount <= 100 && !(!cardsOfUser.Any() || !cardsOfEnemy.Any()))
            {
                Card cardOfEnemy = cardsOfEnemy.RandomElement();
                Card cardOfUser = cardsOfUser.RandomElement();

                battleLog.Add(String.Format("Round {0}: Your {1} is fighting against {2}", roundCount, cardOfUser.Name, cardOfEnemy.Name));

                double damageMadeByEnemy = CalculateDamage(cardOfEnemy, cardOfUser);
                double damageMadeByUser = CalculateDamage(cardOfUser, cardOfEnemy);

                battleLog.Add(String.Format("Round {0}: Damage made by you: {1}; Damage made by your enemy: {2}", roundCount, damageMadeByUser, damageMadeByEnemy));

                if (damageMadeByEnemy < damageMadeByUser)
                {
                    cardsOfEnemy.Remove(cardOfEnemy);
                    cardsOfUser.Add(cardOfEnemy);

                    battleLog.Add(String.Format("Round {0}: You won the round", roundCount));
                }
                else if(damageMadeByEnemy > damageMadeByUser)
                {
                    cardsOfUser.Remove(cardOfUser);
                    cardsOfEnemy.Add(cardOfUser);

                    battleLog.Add(String.Format("Round {0}: You lost the round", roundCount));
                }
                else
                {
                    battleLog.Add(String.Format("Round {0}: Its a draw", roundCount));
                }
                roundCount++;
            }

            battleLog.Add("Battle finished");

            if(!cardsOfUser.Any())
            {
                enemy.PlayedGames++;
                enemy.Elo += 3;

                userContext.User.PlayedGames++;
                userContext.User.Elo -= 5;

                battleLog.Add(String.Format("You lost the battle against {0}", enemy.Name));
            }
            else if(!cardsOfEnemy.Any())
            {
                enemy.PlayedGames++;
                enemy.Elo -= 5;

                userContext.User.PlayedGames++;
                userContext.User.Elo += 3;

                battleLog.Add(String.Format("You won the battle against {0}", enemy.Name));
            }
            else
            {
                enemy.PlayedGames++;
                userContext.User.PlayedGames++;
                battleLog.Add("Its a draw");
            }

            userRepository.Update(userContext.User, nameof(User.PlayedGames), nameof(User.Elo));
            userRepository.Update(enemy, nameof(User.PlayedGames), nameof(User.Elo));

            return battleLog;
        }

        private double CalculateDamage(Card attacker, Card victim)
        {
            double damageMultiplier = 1;

            if (attacker.Name.Equals("Goblin") && victim.Name.Equals("Dragon"))
                return 0;
            if (attacker.Name.Equals("Ork") && victim.Name.Equals("Wizzard"))
                return 0;
            if (attacker.CardType == CardType.Spell && attacker.ElementType == ElementType.Water && victim.Name.Equals("Knight"))
                return double.MaxValue;
            if (attacker.CardType == CardType.Spell && victim.Name.Equals("Kraken"))
                return 0;
            if (attacker.Name.Equals("Dragon") && victim.Name.Equals("FireElve"))
                return 0;

            if (attacker.CardType == CardType.Spell || victim.CardType == CardType.Spell)
            {
                damageMultiplier = GetDamageMultiplierByEffectiveness(attacker, victim);
            }

            return attacker.Damage * damageMultiplier;
        }

        private double GetDamageMultiplierByEffectiveness(Card attacker, Card victim)
        {
            if (attacker.ElementType == ElementType.Water && victim.ElementType == ElementType.Fire)
            {
                return 2;
            }
            else if (attacker.ElementType == ElementType.Fire && victim.ElementType == ElementType.Water)
            {
                return 0.5;
            }
            else if (attacker.ElementType == ElementType.Water && victim.ElementType == ElementType.Water)
            {
                return 1;
            }

            if (attacker.ElementType == ElementType.Fire && victim.ElementType == ElementType.Normal)
            {
                return 2;
            }
            else if (attacker.ElementType == ElementType.Normal && victim.ElementType == ElementType.Fire)
            {
                return 0.5;
            }
            else if (attacker.ElementType == ElementType.Fire && victim.ElementType == ElementType.Fire)
            {
                return 1;
            }

            if (attacker.ElementType == ElementType.Normal && victim.ElementType == ElementType.Water)
            {
                return 2;
            }
            else if (attacker.ElementType == ElementType.Water && victim.ElementType == ElementType.Normal)
            {
                return 0.5;
            }
            else if (attacker.ElementType == ElementType.Normal && victim.ElementType == ElementType.Normal)
            {
                return 1;
            }

            return 1;
        }
    }
}