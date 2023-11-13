using Npgsql;
using System.Net.Sockets;
using System.Net;
using System.Text;
using MonsterTradingCards.Repositories;
using MonsterTradingCards.Models;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using MonsterTradingCards.CAQ.User;
using Microsoft.Extensions.Hosting;
using MonsterTradingCards.Contracts;

namespace MonsterTradingCards
{
    public class Program
    {
        public static string strConnString = "Server=localhost;Port=5432;Username=postgres;Password=mypassword;Database=monster_trading_cards";

        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection()
                .AddScoped<IUserRepository>(_ => new UserRepository(strConnString))
                .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly))
                .BuildServiceProvider();

            var mediator = serviceCollection.GetRequiredService<IMediator>();

            //string token = mediator.Send(new RegisterCommand() { Username = "johannesfor", Password = "Test1234!"}).Result;

            //UserRepository userRepository = new UserRepository(strConnString);
            //List<User> asdf  = userRepository.GetAll().ToList();
            //return;


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
                string? line;

                // 1.1 first line in HTTP contains the method, path and HTTP version
                line = reader.ReadLine();
                if (line != null)
                    Console.WriteLine(line);

                // 1.2 read the HTTP-headers (in HTTP after the first line, until the empy line)
                int content_length = 0; // we need the content_length later, to be able to read the HTTP-content
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
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
                }

                // ----- 2. Do the processing -----
                // .... 

                Console.WriteLine("----------------------------------------");

                // ----- 3. Write the HTTP-Response -----
                var writerAlsoToConsole = new StreamTracer(writer);  // we use a simple helper-class StreamTracer to write the HTTP-Response to the client and to the console

                writerAlsoToConsole.WriteLine("HTTP/1.0 200 OK");    // first line in HTTP-Response contains the HTTP-Version and the status code
                writerAlsoToConsole.WriteLine("Content-Type: text/html; charset=utf-8");     // the HTTP-headers (in HTTP after the first line, until the empy line)
                writerAlsoToConsole.WriteLine();
                writerAlsoToConsole.WriteLine("<html><body><h1>Hello World!</h1></body></html>");    // the HTTP-content (here we just return a minimalistic HTML Hello-World)

                Console.WriteLine("========================================");
            }


        }


    }
}