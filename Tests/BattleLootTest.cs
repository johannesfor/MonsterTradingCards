using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MonsterTradingCards;
using MonsterTradingCards.CAQ.Battle;
using MonsterTradingCards.CAQ.Cards;
using MonsterTradingCards.CAQ.Lootbox;
using MonsterTradingCards.CAQ.Users;
using MonsterTradingCards.Contracts.Repository;
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
    public class BattleLootTest
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

            //The damage is so high/low because only one player should win for the testing
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
        public void A_Battle_ShouldHaveNoLootboxes()
        {
            IEnumerable<Lootbox> lootboxesA = mediatorUserA.Send(new GetAllLootboxQuery()).Result;
            IEnumerable<Lootbox> lootboxesB = mediatorUserB.Send(new GetAllLootboxQuery()).Result;

            Assert.IsFalse(lootboxesA.Any());
            Assert.IsFalse(lootboxesB.Any());
        }

        [TestMethod]
        public void B_Battle_StatsShouldChange()
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
        public void C_Battle_LogShouldBeEquals()
        {
            Task<IEnumerable<string>> battleLogATask = mediatorUserA.Send(new JoinBattleQueueCommand());
            Task<IEnumerable<string>> battleLogBTask = mediatorUserB.Send(new JoinBattleQueueCommand());

            battleLogATask.Wait();
            battleLogBTask.Wait();

            IEnumerable<string> battleLogA = battleLogATask.Result;
            IEnumerable<string> battleLogB = battleLogBTask.Result;

            Assert.AreEqual(battleLogA, battleLogB);
        }

        [TestMethod]
        public void D_Battle_OnlyUserAShouldHaveLootboxes()
        {
            IEnumerable<Lootbox> lootboxesA = mediatorUserA.Send(new GetAllLootboxQuery()).Result;
            IEnumerable<Lootbox> lootboxesB = mediatorUserB.Send(new GetAllLootboxQuery()).Result;

            Assert.IsTrue(lootboxesA.Any());
            Assert.IsFalse(lootboxesB.Any());
        }

        [TestMethod]
        public void E_Battle_OpenLootbox()
        {
            IEnumerable<Lootbox> lootboxesBefore = mediatorUserA.Send(new GetAllLootboxQuery()).Result;
            IEnumerable<Card> cardsBefore = mediatorUserA.Send(new GetAquiredCardsQuery()).Result;

            Card drawnCard = mediatorUserA.Send(new OpenLootboxCommand() { LootboxId = lootboxesBefore.First().Id.Value }).Result;

            IEnumerable<Card> cardsAfter = mediatorUserA.Send(new GetAquiredCardsQuery()).Result;
            IEnumerable<Lootbox> lootboxesAfter = mediatorUserA.Send(new GetAllLootboxQuery()).Result;

            Assert.IsTrue(cardsBefore.Count() < cardsAfter.Count());
            Assert.IsTrue(lootboxesBefore.Count() > lootboxesAfter.Count());
        }

        [TestMethod]
        public void F_Battle_UserAJoinsQueueMultipleTimes_ShouldFail()
        {
            mediatorUserA.Send(new JoinBattleQueueCommand());

            try {
                mediatorUserA.Send(new JoinBattleQueueCommand()).Wait();
            }
            catch (AggregateException ex)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("You are already in queue"));
            }
        }
    }
}
