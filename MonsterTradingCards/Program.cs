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

namespace MonsterTradingCards
{
    public class Program
    {
        public static string strConnString = "Server=localhost;Port=5432;Username=postgres;Password=mypassword;Database=monster_trading_cards";

        static void ClearDB(ServiceProvider serviceCollection)
        {
            //IUserRepository userRepository = serviceCollection.GetRequiredService<IUserRepository>();
            //userRepository.GetAll().ToList().ForEach(user =>
            //{
            //    userRepository.Delete(user);
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
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Our first simple HTTP-Server: http://localhost:10001/");

            // ===== I. Start the HTTP-Server =====
            var httpServer = new TcpListener(IPAddress.Loopback, 10001);
            httpServer.Start();

            while (true)
            {
                // ----- 0. Accept the TCP-Client and create the reader and writer -----
                var clientSocket = httpServer.AcceptTcpClient();
                using var writer = new StreamWriter(clientSocket.GetStream()) { AutoFlush = true };
                using var reader = new StreamReader(clientSocket.GetStream());

                // ----- 1. Read the HTTP-Request -----
                string? line = null;
                string method = null;
                string path = null;
                string body = null;
                Dictionary<string, string> pathParams = new Dictionary<string, string>();

                // 1.1 first line in HTTP contains the method, path and HTTP version
                line = reader.ReadLine();
                if (line != null)
                {
                    Console.WriteLine(line);
                    string[] splittedLine = line.Split(" ");

                    method = splittedLine[0];

                    string[] fullPathSplitted = splittedLine[1].Split("?");
                    path = fullPathSplitted[0];

                    if (fullPathSplitted.Length > 1)
                    {
                        string[] paramsOfPath = fullPathSplitted[1].Split("&");
                        foreach (string param in paramsOfPath)
                        {
                            string[] keyValueSplit = param.Split("=");
                            pathParams.Add(keyValueSplit[0], keyValueSplit[1]);
                        }
                    }
                }

                // 1.2 read the HTTP-headers (in HTTP after the first line, until the empy line)
                int content_length = 0; // we need the content_length later, to be able to read the HTTP-content
                string authorizationToken = null;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                    if (line.StartsWith("Authorization:"))
                    {
                        string[] splittedAuthorization = line.Split(" ");
                        if (splittedAuthorization.Length != 3)
                            throw new ArgumentException("Invalid format for Bearer token");
                        authorizationToken = splittedAuthorization[2];
                    }

                    if (line == "")
                    {
                        break;  // empty line indicates the end of the HTTP-headers
                    }

                    // Parse the header
                    var parts = line.Split(':');
                    if (parts.Length == 2 && parts[0] == "Content-Length")
                    {
                        content_length = int.Parse(parts[1].Trim());
                    }
                }


                // 1.3 read the body if existing
                if (content_length > 0)
                {
                    var data = new StringBuilder(200);
                    char[] chars = new char[1024];
                    int bytesReadTotal = 0;
                    while (bytesReadTotal < content_length)
                    {
                        var bytesRead = reader.Read(chars, 0, chars.Length);
                        bytesReadTotal += bytesRead;
                        if (bytesRead == 0)
                            break;
                        data.Append(chars, 0, bytesRead);
                    }
                    Console.WriteLine(data.ToString());
                    body = data.ToString();
                }

                // ----- 2. Do the processing -----
                // .... 

                Console.WriteLine("----------------------------------------");
                ServiceProvider serviceProvider = SetupDepencyInjection(authorizationToken);
                var mediator = serviceProvider.GetRequiredService<IMediator>();

                string responseBody = null;
                if (path != null)
                {
                    if(method == "POST")
                    {
                        if (path == "/users")
                        {
                            RegisterCommand command = JsonConvert.DeserializeObject<RegisterCommand>(body);
                            string token = mediator.Send(command).Result;
                            responseBody = JsonConvert.SerializeObject(token);
                        }
                        else if (path == "/sessions")
                        {
                            LoginCommand command = JsonConvert.DeserializeObject<LoginCommand>(body);
                            string token = mediator.Send(command).Result;
                            responseBody = JsonConvert.SerializeObject(token);
                        }
                        else if (path == "/packages")
                        {
                            IEnumerable<Card> cards = JsonConvert.DeserializeObject<IEnumerable<Card>>(body);
                            mediator.Send(new CreatePackageCommand() { Cards = cards }).Wait();
                        }
                        else if(path == "/transactions/packages")
                        {
                            mediator.Send(new AquirePackageCommand()).Wait();
                        }
                        else if(path == "/tradings")
                        {
                            Trading trade = JsonConvert.DeserializeObject<Trading>(body);
                            mediator.Send(new AddTradingCommand() { Trading = trade }).Wait();
                        }
                        else if(path.StartsWith("/tradings/"))
                        {
                            Guid cardId = JsonConvert.DeserializeObject<Guid>(body);
                            mediator.Send(new MakeTradeCommand() { CardId = cardId, TradeId = Guid.Parse(path.Split("/")[2]) }).Wait();
                        }
                        else
                        {
                            //throw Exception because the path is invalid
                        }
                    }
                    else if(method == "GET")
                    {
                        if (path == "/cards")
                        {
                            IEnumerable<Card> cards = mediator.Send(new GetAquiredCardsQuery()).Result;
                            responseBody = JsonConvert.SerializeObject(cards);
                        }
                        else if(path == "/deck")
                        {
                            IEnumerable<Card> cards = mediator.Send(new GetDeckQuery()).Result;
                            responseBody = JsonConvert.SerializeObject(cards);
                        }
                        else if(path.StartsWith("/users/"))
                        {
                            string username = path.Split("/")[2];

                            //Eigentlich sollte per Parameter der usename gar nicht angegeben werden, weil der Token reicht aus um den User zu bestimmen im Normalfall
                            //Aufgrund des vorgegeben Curl-Skripts musste ich allerdings diese Überprüfung einbauen, welche den sowieso nicht ganz korrekten/hardcoded Token validiert
                            if (username != authorizationToken.Split("-")[0])
                                throw new ArgumentException("Invalid token for specified username");

                            UserProfile userProfile = mediator.Send(new GetUserProfileQuery()).Result;
                            responseBody = JsonConvert.SerializeObject(userProfile);
                        }
                        else if(path == "/stats")
                        {
                            UserStats userStats = mediator.Send(new GetUserStatsQuery()).Result;
                            responseBody = JsonConvert.SerializeObject(userStats);
                        }
                        else if (path == "/scoreboard")
                        {
                            IEnumerable<Tuple<string, int>> scoreboard = mediator.Send(new GetScoreboardQuery()).Result;
                            responseBody = JsonConvert.SerializeObject(scoreboard);
                        }
                        else if(path == "/tradings")
                        {
                            IEnumerable<Trading> tradings = mediator.Send(new GetTradingsQuery()).Result;
                            responseBody = JsonConvert.SerializeObject(tradings);
                        }
                        else
                        {
                            //throw Exception because the path is invalid
                        }
                    }
                    else if(method == "PUT")
                    {
                        if (path == "/deck")
                        {
                            IEnumerable<Guid> cardIds = JsonConvert.DeserializeObject<IEnumerable<Guid>>(body);
                            mediator.Send(new UpdateDeckCommand() { CardIds = cardIds }).Wait();
                        }
                        else if (path.StartsWith("/users/"))
                        {
                            string username = path.Split("/")[2];

                            //Eigentlich sollte per Parameter der usename gar nicht angegeben werden, weil der Token reicht aus um den User zu bestimmen im Normalfall
                            //Aufgrund des vorgegeben Curl-Skripts musste ich allerdings diese Überprüfung einbauen, welche den sowieso nicht ganz korrekten/hardcoded Token validiert
                            if (username != authorizationToken.Split("-")[0])
                                throw new ArgumentException("Invalid token for specified username");

                            UserProfile userProfile = JsonConvert.DeserializeObject<UserProfile>(body);
                            mediator.Send(new UpdateUserProfileCommand() { UserProfile = userProfile }).Wait();
                        }
                        else
                        {
                            //throw Exception because the path is invalid
                        }
                    }
                    else if(method == "DELETE")
                    {
                        if (path.StartsWith("/tradings/"))
                        {
                            mediator.Send(new RemoveTradingCommand() { TradeId = Guid.Parse(path.Split("/")[2]) } ).Wait();
                        }
                        else
                        {
                            //throw Exception because the path is invalid
                        }
                    }
                    else
                    {
                        //throw Exception because the path is invalid
                    }
                }

                if (responseBody != null && pathParams.ContainsKey("format") && pathParams["format"] == "plain")
                {
                    //var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);
                    //responseBody = string.Join(',', dict.Select(r => $"{r.Key}={r.Value}"));

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

                // ----- 3. Write the HTTP-Response -----
                var writerAlsoToConsole = new StreamTracer(writer);  // we use a simple helper-class StreamTracer to write the HTTP-Response to the client and to the console

                writerAlsoToConsole.WriteLine("HTTP/1.0 200 OK");    // first line in HTTP-Response contains the HTTP-Version and the status code
                writerAlsoToConsole.WriteLine("Content-Type: application/json");     // the HTTP-headers (in HTTP after the first line, until the empy line)
                writerAlsoToConsole.WriteLine();
                writerAlsoToConsole.WriteLine(responseBody);    // the HTTP-content (here we just return a minimalistic HTML Hello-World)

                Console.WriteLine("========================================");
            }


        }

        static ServiceProvider SetupDepencyInjection(string authorizationToken)
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