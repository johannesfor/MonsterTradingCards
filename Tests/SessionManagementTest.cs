using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonsterTradingCards;
using MonsterTradingCards.CAQ.Battle;
using MonsterTradingCards.CAQ.Users;
using MonsterTradingCards.Contracts.Service;
using MonsterTradingCards.Handler.Battle;
using MonsterTradingCards.Service;
using System.Configuration;

namespace Tests
{
    [TestClass]
    public class SessionManagementTest
    {
        private static string _connectionString = "Server=localhost;Port=5432;Username=postgres;Password=mypassword;Database=monster_trading_cards;";
        private static IMediator mediator;
        private static IUserSessionService userSessionService;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            ServiceProvider provider = Program.SetupDepencyInjection(null, _connectionString);
            Program.ClearDB(provider);
            mediator = provider.GetService<IMediator>();
            userSessionService = provider.GetService<IUserSessionService>();
        }

        [TestMethod]
        public void A_RegisterUser_Success()
        {
            string token = mediator.Send(new RegisterCommand() { Username = "altenhof", Password = "markus" }).Result;
            Assert.IsNotNull(token);
        }

        [TestMethod]
        public void B_LoginUser()
        {
            string token = mediator.Send(new LoginCommand() { Username = "altenhof", Password = "markus" }).Result;
            Assert.IsNotNull(token);
        }

        [TestMethod]
        public void C_RegisterUser_UserNameAlreadyUsed()
        {
            try
            {
                string token = mediator.Send(new RegisterCommand() { Username = "altenhof", Password = "markus" }).Result;
                Assert.Fail();
            }
            catch (AggregateException ex)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("Registration with this username is not possible"));
            }
        }

        [TestMethod]
        public void D_LoginUser_UserDoesNotExist()
        {
            try
            {
                string token = mediator.Send(new LoginCommand() { Username = "altenhof123", Password = "markus" }).Result;
                Assert.Fail();
            }
            catch (AggregateException ex)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("A user with this username is not existing"));
            }
        }

        [TestMethod]
        public void E_LoginUser_WrongPassword()
        {
            try
            {
                string token = mediator.Send(new LoginCommand() { Username = "altenhof", Password = "markus123" }).Result;
                Assert.Fail();
            }
            catch (AggregateException ex)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("Invalid password"));
            }
        }

        [TestMethod]
        public void F_CheckTokenValidity_LoggedInUser()
        {
            bool isTokenValid = userSessionService.IsTokenValid("altenhof-mtcgToken");
            Assert.IsTrue(isTokenValid);
        }

        [TestMethod]
        public void G_CheckTokenValidity_UserIsNotLoggedIn()
        {
            bool isTokenValid = userSessionService.IsTokenValid("altenhof123-mtcgToken");
            Assert.IsFalse(isTokenValid);
        }
    }
}