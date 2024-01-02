using Npgsql;
using System.Net.Sockets;
using System.Net;
using System.Text;
using MonsterTradingCards.Repositories;
using MonsterTradingCards.Models;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System;
using MonsterTradingCards.CAQ.Admin;
using MediatR.Behaviors.Authorization.Extensions.DependencyInjection;
using MonsterTradingCards.Factory;
using MonsterTradingCards.CAQ.Packages;
using MonsterTradingCards.CAQ.Cards;
using MonsterTradingCards.CAQ.Deck;
using Newtonsoft.Json.Linq;
using MonsterTradingCards.CAQ.Users;
using MonsterTradingCards.CAQ.Tradings;
using MediatR.Behaviors.Authorization.Exceptions;
using MonsterTradingCards.Exceptions;
using MonsterTradingCards.CAQ.Battle;
using MonsterTradingCards.Webserver;
using System.Configuration;
using System.Xml.Linq;
using MonsterTradingCards.Service;
using MonsterTradingCards.Contracts.Service;
using MonsterTradingCards.Contracts.Repository;
using MonsterTradingCards.Contracts.Factory;
using MonsterTradingCards.CAQ.Lootbox;

namespace MonsterTradingCards
{
    public class Program
    {
        private static string _connectionString;

        static void Main(string[] args)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["postgres"];
            if (settings?.ConnectionString == null)
                throw new ArgumentException("The connection string for the postgres database is not set in the App.config");
            _connectionString = settings.ConnectionString;

            bool clearDBOnStartup = Convert.ToBoolean(ConfigurationManager.AppSettings["ClearDBOnStartup"]);
            if (clearDBOnStartup)
            {
                ServiceProvider serviceProvider = SetupDepencyInjection(null, _connectionString);
                ClearDB(serviceProvider);
            }

            HttpSvr svr = new();
            svr.Incoming += _ProcessMesage;

            svr.Run();

            return;
        }

        private static void _ProcessMesage(object sender, HttpSvrEventArgs e)
        {
            Console.WriteLine(e.PlainMessage);

            string? authorizationToken = e.Headers.FirstOrDefault(header => header.Name.Equals("Authorization"))?.Value;
            ServiceProvider serviceProvider = SetupDepencyInjection(authorizationToken, _connectionString);
            var mediator = serviceProvider.GetRequiredService<IMediator>();

            string responseBody = null;
            ContentType contentType = ContentType.TEXT_PLAIN;

            try
            {
                switch (e.Method)
                {
                    case "GET":
                        switch (e.Path)
                        {
                            case "/cards":
                                IEnumerable<Card> cards = mediator.Send(new GetAquiredCardsQuery()).Result;
                                responseBody = JsonConvert.SerializeObject(cards);
                                contentType = ContentType.APPLICATION_JSON;
                                break;
                            case "/deck":
                                IEnumerable<Card> cardsInDeck = mediator.Send(new GetDeckQuery()).Result;
                                responseBody = JsonConvert.SerializeObject(cardsInDeck);
                                contentType = ContentType.APPLICATION_JSON;
                                break;
                            case var p when p.StartsWith("/users/"):
                                string username = e.Path.Split("/")[2];
                                UserProfile userProfile = mediator.Send(new GetUserProfileQuery() { Username = username }).Result;
                                responseBody = JsonConvert.SerializeObject(userProfile);
                                contentType = ContentType.APPLICATION_JSON;
                                break;
                            case "/stats":
                                UserStats userStats = mediator.Send(new GetUserStatsQuery()).Result;
                                responseBody = JsonConvert.SerializeObject(userStats);
                                contentType = ContentType.APPLICATION_JSON;
                                break;
                            case "/scoreboard":
                                IEnumerable<Tuple<string, int>> scoreboard = mediator.Send(new GetScoreboardQuery()).Result;
                                responseBody = JsonConvert.SerializeObject(scoreboard);
                                contentType = ContentType.APPLICATION_JSON;
                                break;
                            case "/tradings":
                                IEnumerable<Trading> tradings = mediator.Send(new GetTradingsQuery()).Result;
                                responseBody = JsonConvert.SerializeObject(tradings);
                                contentType = ContentType.APPLICATION_JSON;
                                break;
                            case "/lootbox":
                                IEnumerable<Lootbox> lootboxes = mediator.Send(new GetAllLootboxQuery()).Result;
                                responseBody = JsonConvert.SerializeObject(lootboxes);
                                contentType = ContentType.APPLICATION_JSON;
                                break;
                            default:
                                throw new PathNotFoundException();
                        }
                        break;
                    case "POST":
                        switch (e.Path)
                        {
                            case "/users":
                                RegisterCommand registerCommand = JsonConvert.DeserializeObject<RegisterCommand>(e.Payload);
                                string token = mediator.Send(registerCommand).Result;
                                responseBody = JsonConvert.SerializeObject(token);
                                contentType = ContentType.APPLICATION_JSON;
                                break;
                            case "/sessions":
                                LoginCommand loginCommand = JsonConvert.DeserializeObject<LoginCommand>(e.Payload);
                                token = mediator.Send(loginCommand).Result;
                                responseBody = JsonConvert.SerializeObject(token);
                                contentType = ContentType.APPLICATION_JSON;
                                break;
                            case "/packages":
                                IEnumerable<Card> cards = JsonConvert.DeserializeObject<IEnumerable<Card>>(e.Payload);
                                mediator.Send(new CreatePackageCommand() { Cards = cards }).Wait();
                                break;
                            case "/transactions/packages":
                                mediator.Send(new AquirePackageCommand()).Wait();
                                break;
                            case "/tradings":
                                Trading trade = JsonConvert.DeserializeObject<Trading>(e.Payload);
                                mediator.Send(new AddTradingCommand() { Trading = trade }).Wait();
                                break;
                            case var p when p.StartsWith("/tradings/"):
                                Guid cardId = JsonConvert.DeserializeObject<Guid>(e.Payload);
                                mediator.Send(new MakeTradeCommand() { CardId = cardId, TradeId = Guid.Parse(e.Path.Split("/")[2]) }).Wait();
                                break;
                            case "/battles":
                                IEnumerable<string> battleLog = mediator.Send(new JoinBattleQueueCommand()).Result;
                                responseBody = JsonConvert.SerializeObject(battleLog);
                                contentType = ContentType.APPLICATION_JSON;
                                break;
                            case "/lootbox":
                                Guid lootboxId = JsonConvert.DeserializeObject<Guid>(e.Payload);
                                Card drawnCard = mediator.Send(new OpenLootboxCommand() { LootboxId = lootboxId }).Result;
                                responseBody = JsonConvert.SerializeObject(drawnCard);
                                contentType = ContentType.APPLICATION_JSON;
                                break;
                            default:
                                throw new PathNotFoundException();
                        }
                        break;
                    case "PUT":
                        switch (e.Path)
                        {
                            case "/deck":
                                IEnumerable<Guid> cardIds = JsonConvert.DeserializeObject<IEnumerable<Guid>>(e.Payload);
                                mediator.Send(new UpdateDeckCommand() { CardIds = cardIds }).Wait();
                                break;
                            case var p when p.StartsWith("/users/"):
                                string username = e.Path.Split("/")[2];
                                UserProfile userProfile = JsonConvert.DeserializeObject<UserProfile>(e.Payload);
                                mediator.Send(new UpdateUserProfileCommand() { UserProfile = userProfile, Username = username }).Wait();
                                break;
                            default:
                                throw new PathNotFoundException();
                        }
                        break;
                    case "DELETE":
                        switch (e.Path)
                        {
                            case var p when p.StartsWith("/tradings/"):
                                mediator.Send(new RemoveTradingCommand() { TradeId = Guid.Parse(e.Path.Split("/")[2]) }).Wait();
                                break;
                            default:
                                throw new PathNotFoundException();
                        }
                        break;
                    default:
                        throw new PathNotFoundException();
                }

                e.Reply(200, responseBody, contentType);
            }
            catch (PathNotFoundException ex)
            {
                e.Reply(404, "Method and path does not match endpoint");
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException?.GetType() == typeof(UnauthorizedException))
                    e.Reply(401, "Not authorized: " + ex.InnerException.Message);
                else
                    e.Reply(400, "Bad request: " + ex.Message);
            }
            catch (Exception ex)
            {
                e.Reply(400, "Bad request");
            }
        }

        public static ServiceProvider SetupDepencyInjection(string? authorizationToken, string connectionString)
        {
            var serviceCollection = new ServiceCollection()
                .AddScoped<IUserRepository>(_ => new UserRepository(connectionString))
                .AddScoped<IPackageRepository>(_ => new PackageRepository(connectionString))
                .AddScoped<ICardRepository>(_ => new CardRepository(connectionString))
                .AddScoped<ITradingRepository>(_ => new TradingRepository(connectionString))
                .AddScoped<ILootboxRepository>(_ => new LootboxRepository(connectionString))
                .AddScoped<IUserContextFactory, UserContextFactory>()
                .AddScoped<IBattleService, BattleService>()
                .AddScoped<IUserSessionService, UserSessionService>()
                .AddScoped<ICardFactory, CardFactory>()
                .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly))
                .AddMediatorAuthorization(Assembly.GetExecutingAssembly())
                .AddScoped<IUserContext>((serviceProvider) =>
                {
                    IUserContextFactory userContextFactory = serviceProvider.GetRequiredService<IUserContextFactory>();
                    return userContextFactory.Create(authorizationToken);
                });
           serviceCollection.AddAuthorizersFromAssembly(Assembly.GetExecutingAssembly());

           var serviceProvider = serviceCollection.BuildServiceProvider();
           return serviceProvider;
        }

        public static void ClearDB(ServiceProvider serviceCollection)
        {
            ILootboxRepository lootboxRepository = serviceCollection.GetRequiredService<ILootboxRepository>();
            lootboxRepository.GetAll().ToList().ForEach(lootbox =>
            {
                lootboxRepository.Delete(lootbox.Id.Value);
            });

            ITradingRepository tradingRepository = serviceCollection.GetRequiredService<ITradingRepository>();
            tradingRepository.GetAll().ToList().ForEach(trade =>
            {
                tradingRepository.Delete(trade.Id.Value);
            });

            ICardRepository cardRepository = serviceCollection.GetRequiredService<ICardRepository>();
            cardRepository.GetAll().ToList().ForEach(card =>
            {
                cardRepository.Delete(card.Id.Value);
            });

            IPackageRepository packageRepository = serviceCollection.GetRequiredService<IPackageRepository>();
            packageRepository.GetAll().ToList().ForEach(package =>
            {
                packageRepository.Delete(package.Id.Value);
            });

            IUserRepository userRepository = serviceCollection.GetRequiredService<IUserRepository>();
            userRepository.GetAll().ToList().ForEach(user =>
            {
                userRepository.Delete(user.Id.Value);
            });
        }
    }
}