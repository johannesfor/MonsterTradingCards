using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MonsterTradingCards;
using MonsterTradingCards.CAQ.Battle;
using MonsterTradingCards.CAQ.Users;
using MonsterTradingCards.Contracts;
using MonsterTradingCards.Models;
using MonsterTradingCards.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class BattleTest
    {
        private static string _connectionString = "Server=localhost;Port=5432;Username=postgres;Password=mypassword;Database=monster_trading_cards;";
        private static IMediator mediatorUserA;
        private static IMediator mediatorUserB;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            //Clear database and register & login the test users, create the test data
            ServiceProvider provider = Program.SetupDepencyInjection(null, _connectionString);
            Program.ClearDB(provider);

            var userRepository = provider.GetService<IUserRepository>();
            var cardRepository = provider.GetService<ICardRepository>();

            var userA = new User() { Id = Guid.NewGuid(), Username = "kienboec", Password = "daniel".HashPassword(), Elo = 100};
            userRepository.Add(userA);
            var userB = new User() { Id = Guid.NewGuid(), Username = "altenhof", Password = "markus".HashPassword(), Elo = 100 };
            userRepository.Add(userB);

            //The damage of water goblin is so high because userA should always win
            cardRepository.Add(new Card() { Id = Guid.NewGuid(), Name = "WaterGoblin", CardType = CardType.Monster, ElementType = ElementType.Water, Damage = 100_000, IsInDeck = true, UserId = userA.Id });
            cardRepository.Add(new Card() { Id = Guid.NewGuid(), Name = "Dragon", CardType = CardType.Monster, ElementType = ElementType.Normal, Damage = 100_000, IsInDeck = true, UserId = userA.Id });
            cardRepository.Add(new Card() { Id = Guid.NewGuid(), Name = "WaterSpell", CardType = CardType.Spell, ElementType = ElementType.Water, Damage = 100_000, IsInDeck = true, UserId = userA.Id });
            cardRepository.Add(new Card() { Id = Guid.NewGuid(), Name = "Firespell", CardType = CardType.Spell, ElementType = ElementType.Fire, Damage = 100_000, IsInDeck = true, UserId = userA.Id });

            cardRepository.Add(new Card() { Id = Guid.NewGuid(), Name = "FireGoblin", CardType = CardType.Monster, ElementType = ElementType.Fire, Damage = 0, IsInDeck = true, UserId = userB.Id });
            cardRepository.Add(new Card() { Id = Guid.NewGuid(), Name = "Dragon", CardType = CardType.Monster, ElementType = ElementType.Normal, Damage = 0, IsInDeck = true, UserId = userB.Id });
            cardRepository.Add(new Card() { Id = Guid.NewGuid(), Name = "WaterSpell", CardType = CardType.Spell, ElementType = ElementType.Water, Damage = 0, IsInDeck = true, UserId = userB.Id });
            cardRepository.Add(new Card() { Id = Guid.NewGuid(), Name = "Firespell", CardType = CardType.Spell, ElementType = ElementType.Fire, Damage = 0, IsInDeck = true, UserId = userB.Id });

            //Login the new users
            var mediator = provider.GetService<IMediator>();
            mediator.Send(new LoginCommand() { Username = "kienboec", Password = "daniel" }).Wait();
            mediator.Send(new LoginCommand() { Username = "altenhof", Password = "markus" }).Wait();

            provider = Program.SetupDepencyInjection("Bearer kienboec-mtcgToken", _connectionString);
            mediatorUserA = provider.GetService<IMediator>();

            provider = Program.SetupDepencyInjection("Bearer altenhof-mtcgToken", _connectionString);
            mediatorUserB = provider.GetService<IMediator>();
        }

        [TestMethod]
        public void A_Battle_StatsShouldChange()
        {
            UserStats resultBefore = mediatorUserA.Send(new GetUserStatsQuery()).Result;

            Task<IEnumerable<string>> battleLogATask = mediatorUserA.Send(new JoinBattleQueueCommand());
            Task<IEnumerable<string>> battleLogBTask = mediatorUserB.Send(new JoinBattleQueueCommand());

            battleLogATask.Wait();
            battleLogBTask.Wait();

            UserStats resultAfter = mediatorUserA.Send(new GetUserStatsQuery()).Result;

            Assert.IsTrue(resultBefore.Elo != resultAfter.Elo);
            Assert.IsTrue(resultBefore.PlayedGames < resultAfter.PlayedGames);
        }

        [TestMethod]
        public void B_Battle_LogShouldBeEquals()
        {
            Task<IEnumerable<string>> battleLogATask = mediatorUserA.Send(new JoinBattleQueueCommand());
            Task<IEnumerable<string>> battleLogBTask = mediatorUserB.Send(new JoinBattleQueueCommand());

            battleLogATask.Wait();
            battleLogBTask.Wait();

            IEnumerable<string> battleLogA = battleLogATask.Result;
            IEnumerable<string> battleLogB = battleLogBTask.Result;

            Assert.AreEqual(battleLogA, battleLogB);
        }
    }
}
