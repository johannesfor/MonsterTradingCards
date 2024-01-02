using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MonsterTradingCards;
using MonsterTradingCards.CAQ.Admin;
using MonsterTradingCards.CAQ.Users;
using MonsterTradingCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class PackageTest
    {
        private static string _connectionString = "Server=localhost;Port=5432;Username=postgres;Password=mypassword;Database=monster_trading_cards;";
        private static IMediator mediator;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            //Clear database and register & login admin
            ServiceProvider provider = Program.SetupDepencyInjection(null, _connectionString);
            Program.ClearDB(provider);
            mediator = provider.GetService<IMediator>();
            string token = mediator.Send(new RegisterCommand() { Username = "admin", Password = "istrator" }).Result;


            //Create new depency injection with the bearer token
            provider = Program.SetupDepencyInjection("Bearer admin-mtcgToken", _connectionString);
            mediator = provider.GetService<IMediator>();
        }

        [TestMethod]
        public void A_CreatePackage()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card() { Id = Guid.NewGuid(), Name = "WaterGoblin", Damage = 10});
            cards.Add(new Card() { Id = Guid.NewGuid(), Name = "Dragon", Damage = 50});
            cards.Add(new Card() { Id = Guid.NewGuid(), Name = "WaterSpell", Damage = 20});
            cards.Add(new Card() { Id = Guid.NewGuid(), Name = "Ork", Damage = 45});
            cards.Add(new Card() { Id = Guid.NewGuid(), Name = "FireSpell", Damage = 25});
            mediator.Send(new CreatePackageCommand() { Cards = cards }).Wait();
        }

        [TestMethod]
        public void B_CreatePackage_ToLessCards()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card() { Id = Guid.NewGuid(), Name = "WaterGoblin", Damage = 10 });
            try {
                mediator.Send(new CreatePackageCommand() { Cards = cards }).Wait();
                Assert.Fail();
            }
            catch(AggregateException ex)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("A package consists of 5 cards"));
            }
        }

        [TestMethod]
        public void C_CreatePackage_ToMuchCards()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card() { Id = Guid.NewGuid(), Name = "WaterGoblin", Damage = 10 });
            cards.Add(new Card() { Id = Guid.NewGuid(), Name = "Dragon", Damage = 50 });
            cards.Add(new Card() { Id = Guid.NewGuid(), Name = "WaterSpell", Damage = 20 });
            cards.Add(new Card() { Id = Guid.NewGuid(), Name = "Ork", Damage = 45 });
            cards.Add(new Card() { Id = Guid.NewGuid(), Name = "FireSpell", Damage = 25 });
            cards.Add(new Card() { Id = Guid.NewGuid(), Name = "WaterGoblin", Damage = 9 });
            try
            {
                mediator.Send(new CreatePackageCommand() { Cards = cards }).Wait();
                Assert.Fail();
            }
            catch (AggregateException ex)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("A package consists of 5 cards"));
            }
        }

        [TestMethod]
        public void D_CreatePackage_UserIsNotAdmin()
        {
            string token = mediator.Send(new RegisterCommand() { Username = "altenhof", Password = "markus" }).Result;
            ServiceProvider provider = Program.SetupDepencyInjection("Bearer altenhof-mtcgToken", _connectionString);
            var mediatorNormalUser = provider.GetService<IMediator>();

            List<Card> cards = new List<Card>();
            cards.Add(new Card() { Id = Guid.NewGuid(), Name = "WaterGoblin", Damage = 10 });
            cards.Add(new Card() { Id = Guid.NewGuid(), Name = "Dragon", Damage = 50 });
            cards.Add(new Card() { Id = Guid.NewGuid(), Name = "WaterSpell", Damage = 20 });
            cards.Add(new Card() { Id = Guid.NewGuid(), Name = "Ork", Damage = 45 });
            cards.Add(new Card() { Id = Guid.NewGuid(), Name = "FireSpell", Damage = 25 });
            try
            {
                mediatorNormalUser.Send(new CreatePackageCommand() { Cards = cards }).Wait();
                Assert.Fail();
            }
            catch (AggregateException ex)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("You need to be admin"));
            }
        }
    }
}
