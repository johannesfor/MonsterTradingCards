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
using MonsterTradingCards.Contracts;
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
using FHTW.Swen1.Swamp;
using MediatR.Behaviors.Authorization.Exceptions;
using MonsterTradingCards.Exceptions;

namespace MonsterTradingCards
{
    public class Program
    {
        public static string strConnString = "Server=localhost;Port=5432;Username=postgres;Password=mypassword;Database=monster_trading_cards";

        static void ClearDB(ServiceProvider serviceCollection)
        {
            //ITradingRepository tradingRepository = serviceCollection.GetRequiredService<ITradingRepository>();
            //tradingRepository.GetAll().ToList().ForEach(trade =>
            //{
            //    tradingRepository.Delete(trade);
            //});

            //ICardRepository cardRepository = serviceCollection.GetRequiredService<ICardRepository>();
            //cardRepository.GetAll().ToList().ForEach(card =>
            //{
            //    cardRepository.Delete(card);
            //});

            //IPackageRepository packageRepository = serviceCollection.GetRequiredService<IPackageRepository>();
            //packageRepository.GetAll().ToList().ForEach(package =>
            //{
            //    packageRepository.Delete(package);
            //});

            //IUserRepository userRepository = serviceCollection.GetRequiredService<IUserRepository>();
            //userRepository.GetAll().ToList().ForEach(user =>
            //{
            //    userRepository.Delete(user);
            //});
        }

        private static void _ProcessMesage(object sender, HttpSvrEventArgs e)
        {
            Console.WriteLine(e.PlainMessage);

            string? authorizationToken = e.Headers.FirstOrDefault(header => header.Name.Equals("Authorization"))?.Value;
            ServiceProvider serviceProvider = SetupDepencyInjection(authorizationToken);
            var mediator = serviceProvider.GetRequiredService<IMediator>();

            string responseBody = null;

            try
            {
                if (e.Method == "POST")
                {
                    if (e.Path == "/users")
                    {
                        RegisterCommand command = JsonConvert.DeserializeObject<RegisterCommand>(e.Payload);
                        string token = mediator.Send(command).Result;
                        responseBody = JsonConvert.SerializeObject(token);
                    }
                    else if (e.Path == "/sessions")
                    {
                        LoginCommand command = JsonConvert.DeserializeObject<LoginCommand>(e.Payload);
                        string token = mediator.Send(command).Result;
                        responseBody = JsonConvert.SerializeObject(token);
                    }
                    else if (e.Path == "/packages")
                    {
                        IEnumerable<Card> cards = JsonConvert.DeserializeObject<IEnumerable<Card>>(e.Payload);
                        mediator.Send(new CreatePackageCommand() { Cards = cards }).Wait();
                    }
                    else if (e.Path == "/transactions/packages")
                    {
                        mediator.Send(new AquirePackageCommand()).Wait();
                    }
                    else if (e.Path == "/tradings")
                    {
                        Trading trade = JsonConvert.DeserializeObject<Trading>(e.Payload);
                        mediator.Send(new AddTradingCommand() { Trading = trade }).Wait();
                    }
                    else if (e.Path.StartsWith("/tradings/"))
                    {
                        Guid cardId = JsonConvert.DeserializeObject<Guid>(e.Payload);
                        mediator.Send(new MakeTradeCommand() { CardId = cardId, TradeId = Guid.Parse(e.Path.Split("/")[2]) }).Wait();
                    }
                    else
                    {
                        throw new PathNotFoundException();
                    }
                }
                else if (e.Method == "GET")
                {
                    if (e.Path == "/cards")
                    {
                        IEnumerable<Card> cards = mediator.Send(new GetAquiredCardsQuery()).Result;
                        responseBody = JsonConvert.SerializeObject(cards);
                    }
                    else if (e.Path == "/deck")
                    {
                        IEnumerable<Card> cards = mediator.Send(new GetDeckQuery()).Result;
                        responseBody = JsonConvert.SerializeObject(cards);
                    }
                    else if (e.Path.StartsWith("/users/"))
                    {
                        string username = e.Path.Split("/")[2];

                        //Eigentlich sollte per Parameter der usename gar nicht angegeben werden, weil der Token reicht aus um den User zu bestimmen im Normalfall
                        //Aufgrund des vorgegeben Curl-Skripts musste ich allerdings diese Überprüfung einbauen, welche den sowieso nicht ganz korrekten/hardcoded Token validiert
                        if (username != authorizationToken.Split("-")[0])
                            throw new ArgumentException("Invalid token for specified username");

                        UserProfile userProfile = mediator.Send(new GetUserProfileQuery()).Result;
                        responseBody = JsonConvert.SerializeObject(userProfile);
                    }
                    else if (e.Path == "/stats")
                    {
                        UserStats userStats = mediator.Send(new GetUserStatsQuery()).Result;
                        responseBody = JsonConvert.SerializeObject(userStats);
                    }
                    else if (e.Path == "/scoreboard")
                    {
                        IEnumerable<Tuple<string, int>> scoreboard = mediator.Send(new GetScoreboardQuery()).Result;
                        responseBody = JsonConvert.SerializeObject(scoreboard);
                    }
                    else if (e.Path == "/tradings")
                    {
                        IEnumerable<Trading> tradings = mediator.Send(new GetTradingsQuery()).Result;
                        responseBody = JsonConvert.SerializeObject(tradings);
                    }
                    else
                    {
                        throw new PathNotFoundException();
                    }
                }
                else if (e.Method == "PUT")
                {
                    if (e.Path == "/deck")
                    {
                        IEnumerable<Guid> cardIds = JsonConvert.DeserializeObject<IEnumerable<Guid>>(e.Payload);
                        mediator.Send(new UpdateDeckCommand() { CardIds = cardIds }).Wait();
                    }
                    else if (e.Path.StartsWith("/users/"))
                    {
                        string username = e.Path.Split("/")[2];

                        //Eigentlich sollte per Parameter der usename gar nicht angegeben werden, weil der Token reicht aus um den User zu bestimmen im Normalfall
                        //Aufgrund des vorgegeben Curl-Skripts musste ich allerdings diese Überprüfung einbauen, welche den sowieso nicht ganz korrekten/hardcoded Token validiert
                        if (username != authorizationToken.Split("-")[0])
                            throw new ArgumentException("Invalid token for specified username");

                        UserProfile userProfile = JsonConvert.DeserializeObject<UserProfile>(e.Payload);
                        mediator.Send(new UpdateUserProfileCommand() { UserProfile = userProfile }).Wait();
                    }
                    else
                    {
                        throw new PathNotFoundException();
                    }
                }
                else if (e.Method == "DELETE")
                {
                    if (e.Path.StartsWith("/tradings/"))
                    {
                        mediator.Send(new RemoveTradingCommand() { TradeId = Guid.Parse(e.Path.Split("/")[2]) }).Wait();
                    }
                    else
                    {
                        throw new PathNotFoundException();
                    }
                }
                else
                {
                    throw new PathNotFoundException();
                }


                if (responseBody != null)
                {
                    string format = e.PathParams.FirstOrDefault(pathParam => pathParam.Equals("format"))?.Value;
                    if (format == "plain")
                    {
                        StringBuilder sb = new StringBuilder();

                        // Deserialisiere das JSON
                        JArray jsonArray = JArray.Parse(responseBody);

                        // Iteriere durch jedes Element im JSON-Array
                        foreach (JObject jsonObject in jsonArray)
                        {
                            // Iteriere durch jedes Property im JSON-Objekt
                            foreach (var property in jsonObject.Properties())
                            {
                                sb.AppendLine($"{property.Name}: {GetValueAsString(property.Value)}");
                            }
                        }

                        responseBody = sb.ToString();
                    }
                }

                e.Reply(200, responseBody);
            }
            catch(PathNotFoundException ex)
            {
                e.Reply(404, "Method and path does not match endpoint");
            }
            catch(AggregateException ex)
            {
                if (ex.InnerException?.GetType() == typeof(UnauthorizedException))
                    e.Reply(401, "Not authorized: " + ex.InnerException.Message);
                else
                    e.Reply(400, "Bad request");
            }
            catch(Exception ex)
            {
                e.Reply(400, "Bad request");
            }
        }

        static void Main(string[] args)
        {
            HttpSvr svr = new();
            svr.Incoming += _ProcessMesage;

            svr.Run();
            return;

            //TODO:
            //Statt 0 bzw. 1 für die Card-Typen sollten Enums verwendet werden wie das Skript es vorschlägt
            //Die ganzen Try/catches/excpetion http status codes einbauen
            //Das Fight System
        }

        static ServiceProvider SetupDepencyInjection(string? authorizationToken)
        {
            var serviceCollection = new ServiceCollection()
                .AddScoped<IUserRepository>(_ => new UserRepository(strConnString))
                .AddScoped<IPackageRepository>(_ => new PackageRepository(strConnString))
                .AddScoped<ICardRepository>(_ => new CardRepository(strConnString))
                .AddScoped<ITradingRepository>(_ => new TradingRepository(strConnString))
                .AddScoped<IUserContextFactory, UserContextFactory>()
                .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly))
                .AddMediatorAuthorization(Assembly.GetExecutingAssembly())
                .AddScoped<IUserContext>((serviceProvider) =>
                {
                    IUserContextFactory userContextFactory = serviceProvider.GetRequiredService<IUserContextFactory>();
                    return userContextFactory.Create(authorizationToken);
                });
           serviceCollection.AddAuthorizersFromAssembly(Assembly.GetExecutingAssembly());

            var serviceProvider = serviceCollection.BuildServiceProvider();
            ClearDB(serviceProvider);
            return serviceProvider;
        }

        static string GetValueAsString(JToken value)
        {
            if (value.Type == JTokenType.String)
            {
                return $"\"{value.ToString()}\"";
            }

            return value.ToString();
        }
    }
}