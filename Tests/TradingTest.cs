using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MonsterTradingCards.CAQ.Users;
using MonsterTradingCards.Contracts.Repository;
using MonsterTradingCards.Models;
using MonsterTradingCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCards.CAQ.Lootbox;
using MonsterTradingCards.CAQ.Cards;
using MonsterTradingCards.CAQ.Tradings;
using MonsterTradingCards.CAQ.Battle;

namespace Tests
{
    [TestClass]
    public class TradingTest
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

            var userA = new User() { Id = Guid.NewGuid(), Username = "kienboec", Password = "daniel".HashPassword(), Elo = 100 };
            userRepository.Add(userA);
            var userB = new User() { Id = Guid.NewGuid(), Username = "altenhof", Password = "markus".HashPassword(), Elo = 100 };
            userRepository.Add(userB);

            //The damage is so high/low because only one player should win for the testing
            cardRepository.Add(new Card() { Id = Guid.NewGuid(), Name = "WaterGoblin", CardType = CardType.Monster, ElementType = ElementType.Water, Damage = 100, IsInDeck = true, UserId = userA.Id });
            cardRepository.Add(new Card() { Id = Guid.NewGuid(), Name = "Dragon", CardType = CardType.Monster, ElementType = ElementType.Normal, Damage = 100, IsInDeck = false, UserId = userA.Id });

            cardRepository.Add(new Card() { Id = Guid.NewGuid(), Name = "FireGoblin", CardType = CardType.Monster, ElementType = ElementType.Fire, Damage = 100, IsInDeck = true, UserId = userB.Id });
            cardRepository.Add(new Card() { Id = Guid.NewGuid(), Name = "Dragon", CardType = CardType.Monster, ElementType = ElementType.Normal, Damage = 100, IsInDeck = false, UserId = userB.Id });

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
        public void A_CreateTrading_CardIsInDeck()
        {
            IEnumerable<Card> cards = mediatorUserA.Send(new GetAquiredCardsQuery()).Result;
            Card card = cards.First(card => card.IsInDeck == true);

            Trading trading = new Trading { CardToTrade = card.Id.Value, MinimumDamage = 0, Type = CardType.Monster };
            try
            {
                mediatorUserA.Send(new AddTradingCommand() { Trading = trading }).Wait();
                Assert.Fail();
            }
            catch (AggregateException ex)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("Is currently in the deck. Please remove first"));
            }
        }

        [TestMethod]
        public void B_CreateTrading_IsNotCardOwner()
        {
            IEnumerable<Card> cards = mediatorUserA.Send(new GetAquiredCardsQuery()).Result;
            Card card = cards.First(card => card.IsInDeck == false);

            Trading trading = new Trading { CardToTrade = card.Id.Value, MinimumDamage = 0, Type = CardType.Monster };
            try
            {
                mediatorUserB.Send(new AddTradingCommand() { Trading = trading }).Wait();
                Assert.Fail();
            }
            catch (AggregateException ex)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("This user is not the owner of this card"));
            }
        }

        [TestMethod]
        public void C_CreateTrading()
        {
            IEnumerable<Card> cards = mediatorUserA.Send(new GetAquiredCardsQuery()).Result;
            Card card = cards.First(card => card.IsInDeck == false);

            Trading trading = new Trading { CardToTrade = card.Id.Value, MinimumDamage = 200, Type = CardType.Monster, Id = Guid.NewGuid() };
            mediatorUserA.Send(new AddTradingCommand() { Trading = trading }).Wait();
        }

        [TestMethod]
        public void D_CreateTrading_CardIsAlreadyOnMarketplace()
        {
            IEnumerable<Card> cards = mediatorUserA.Send(new GetAquiredCardsQuery()).Result;
            Card card = cards.First(card => card.IsInDeck == false);

            Trading trading = new Trading { CardToTrade = card.Id.Value, MinimumDamage = 0, Type = CardType.Monster };
            try
            {
                mediatorUserA.Send(new AddTradingCommand() { Trading = trading }).Wait();
                Assert.Fail();
            }
            catch (AggregateException ex)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("There is already a trade with this card"));
            }
        }

        [TestMethod]
        public void E_MakeTrading_InvalidRequirements()
        {
            IEnumerable<Card> cards = mediatorUserB.Send(new GetAquiredCardsQuery()).Result;
            Card card = cards.First(card => card.IsInDeck == false);

            IEnumerable<Trading> tradings = mediatorUserB.Send(new GetTradingsQuery()).Result;
            try
            {
                mediatorUserB.Send(new MakeTradeCommand() { TradeId = tradings.First().Id.Value, CardId = card.Id.Value }).Wait();
                Assert.Fail();
            }
            catch (AggregateException ex)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("The card you offer does not meet the requirements"));
            }
        }

        [TestMethod]
        public void F_DeleteTrading_NotOwnerOfTrade()
        {
            IEnumerable<Trading> tradings = mediatorUserB.Send(new GetTradingsQuery()).Result;
            try
            {
                mediatorUserB.Send(new RemoveTradingCommand() { TradeId = tradings.First().Id.Value }).Wait();
                Assert.Fail();
            }
            catch (AggregateException ex)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("This user is not the owner of the trade"));
            }
        }

        [TestMethod]
        public void G_DeleteTrading()
        {
            IEnumerable<Trading> tradings = mediatorUserB.Send(new GetTradingsQuery()).Result;
            mediatorUserA.Send(new RemoveTradingCommand() { TradeId = tradings.First().Id.Value }).Wait();
        }

        [TestMethod]
        public void H_MakeTrading()
        {
            IEnumerable<Card> cardsUserA = mediatorUserA.Send(new GetAquiredCardsQuery()).Result;
            Card cardUserA = cardsUserA.First(card => card.IsInDeck == false);

            Trading trading = new Trading { CardToTrade = cardUserA.Id.Value, MinimumDamage = 50, Type = CardType.Monster, Id = Guid.NewGuid() };
            mediatorUserA.Send(new AddTradingCommand() { Trading = trading }).Wait();


            IEnumerable<Card> cardsUserB = mediatorUserB.Send(new GetAquiredCardsQuery()).Result;
            Card cardUserB = cardsUserB.First(card => card.IsInDeck == false);

            IEnumerable<Trading> tradings = mediatorUserB.Send(new GetTradingsQuery()).Result;
            mediatorUserB.Send(new MakeTradeCommand() { TradeId = tradings.First().Id.Value, CardId = cardUserB.Id.Value }).Wait();
        }
    }
}
