using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonsterTradingCards;
using MonsterTradingCards.CAQ.Battle;
using MonsterTradingCards.CAQ.Users;
using MonsterTradingCards.Handler.Battle;
using MonsterTradingCards.Service;
using System.Configuration;

namespace Tests
{
    [TestClass]
    public class SessionManagementTest
    {
        private string _connectionString;
        public SessionManagementTest() {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["postgres"];
            if (settings?.ConnectionString == null)
                throw new ArgumentException("The connection string for the postgres database is not set in the App.config");
            _connectionString = settings.ConnectionString;
        }

        [TestMethod]
        [Priority(0)]
        public void RegisterUser()
        {
            ServiceProvider provider = Program.SetupDepencyInjection(null, _connectionString);
            //Program.SetupDepencyInjection("altenhof-mtcgToken");
            var mediator = provider.GetService<IMediator>();

            string token = mediator.Send(new RegisterCommand() { Username = "altenhof", Password = "markus" }).Result;
            Assert.IsNotNull(token);
        }

        [TestMethod]
        [Priority(1)]
        public void LoginUser()
        {
            ServiceProvider provider = Program.SetupDepencyInjection(null, _connectionString);
            //Program.SetupDepencyInjection("altenhof-mtcgToken");
            var mediator = provider.GetService<IMediator>();

            string token = mediator.Send(new LoginCommand() { Username = "altenhof", Password = "markus" }).Result;
            Assert.IsNotNull(token);
        }
    }
}