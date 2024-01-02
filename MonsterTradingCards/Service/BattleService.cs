using MonsterTradingCards.Contracts.Repository;
using MonsterTradingCards.Contracts.Service;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCards.Service
{
    public class BattleService : IBattleService
    {
        private ICardRepository cardRepository;
        private IUserRepository userRepository;
        private ILootboxRepository lootboxRepository;
        private Random rd;
        private static Queue<BattleTask> queue = new Queue<BattleTask>();
        public BattleService(ICardRepository cardRepository, IUserRepository userRepository, ILootboxRepository lootboxRepository)
        {
            this.cardRepository = cardRepository;
            this.userRepository = userRepository;
            this.lootboxRepository = lootboxRepository;
            rd = new Random();
        }

        public async Task<IEnumerable<string>> JoinQueueForBattle(Guid userId)
        {
            if (queue.Any())
            {
                BattleTask enemyUser = queue.Dequeue();
                IEnumerable<string> battleLog = Battle(userId, enemyUser.UserId);
                enemyUser.Log = battleLog;
                enemyUser.Task.SetResult(true);
                return battleLog;
            }
            else
            {
                BattleTask battleTask = new BattleTask()
                {
                    Task = new TaskCompletionSource<bool>(),
                    UserId = userId
                };
                queue.Enqueue(battleTask);
                await battleTask.Task.Task;
                return battleTask.Log;
                //Wait and return the same battle log
            }
        }

        private IEnumerable<string> Battle(Guid userAId, Guid userBId)
        {
            List<string> battleLog = new List<string>();

            User userA = userRepository.Get(userAId);
            User userB = userRepository.Get(userBId);
            battleLog.Add(String.Format("{0} fights against {1}", userA.Name ?? userA.Username, userB.Name ?? userB.Username));

            List<Card> cardsOfUserA = cardRepository.GetAllByUserId(userA.Id.Value, true).ToList();
            List<Card> cardsOfUserB = cardRepository.GetAllByUserId(userB.Id.Value, true).ToList();

            int roundCount = 1;

            while (roundCount <= 100 && !(!cardsOfUserA.Any() || !cardsOfUserB.Any()))
            {
                Card cardOfEnemy = cardsOfUserB.RandomElement();
                Card cardOfUser = cardsOfUserA.RandomElement();

                battleLog.Add(String.Format("Round {0}: {1} (from {3}) is fighting against {2} (from {4})", roundCount, cardOfUser.Name, cardOfEnemy.Name, userA.Name ?? userA.Username, userB.Name ?? userB.Username));

                double damageMadeByEnemy = CalculateDamage(cardOfEnemy, cardOfUser);
                double damageMadeByUser = CalculateDamage(cardOfUser, cardOfEnemy);

                battleLog.Add(String.Format("Round {0}: Damage made by {3}: {1}; Damage made by {4}: {2}", roundCount, damageMadeByUser, damageMadeByEnemy, userA.Name ?? userA.Username, userB.Name ?? userB.Username));

                if (damageMadeByEnemy < damageMadeByUser)
                {
                    cardsOfUserB.Remove(cardOfEnemy);
                    cardsOfUserA.Add(cardOfEnemy);

                    battleLog.Add(String.Format("Round {0}: {1} won the round", roundCount, userA.Name ?? userA.Username));
                }
                else if (damageMadeByEnemy > damageMadeByUser)
                {
                    cardsOfUserA.Remove(cardOfUser);
                    cardsOfUserB.Add(cardOfUser);

                    battleLog.Add(String.Format("Round {0}: {1} won the round", roundCount, userB.Name ?? userB.Username));
                }
                else
                {
                    battleLog.Add(String.Format("Round {0}: Its a draw", roundCount));
                }
                roundCount++;
            }

            battleLog.Add("Battle finished");

            if (!cardsOfUserA.Any())
            {
                userB.PlayedGames++;
                userB.Elo += 3;

                userA.PlayedGames++;
                userA.Elo -= 5;

                battleLog.Add(String.Format("{0} won the battle against {1}", userB.Name ?? userB.Username, userA.Name ?? userA.Username));

                //Unique feature
                lootboxRepository.Add(new Lootbox()
                {
                    Id = Guid.NewGuid(),
                    UserId = userB.Id,
                    Rarity = rd.Next(10)
                });
            }
            else if (!cardsOfUserB.Any())
            {
                userB.PlayedGames++;
                userB.Elo -= 5;

                userA.PlayedGames++;
                userA.Elo += 3;

                battleLog.Add(String.Format("{0} won the battle against {1}", userA.Name ?? userA.Username, userB.Name ?? userB.Username));

                //Unique feature
                lootboxRepository.Add(new Lootbox()
                {
                    Id = Guid.NewGuid(),
                    UserId = userA.Id,
                    Rarity = rd.Next(10)
                });
            }
            else
            {
                userB.PlayedGames++;
                userA.PlayedGames++;
                battleLog.Add("Its a draw");
            }

            userRepository.Update(userA, nameof(User.PlayedGames), nameof(User.Elo));
            userRepository.Update(userB, nameof(User.PlayedGames), nameof(User.Elo));

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
