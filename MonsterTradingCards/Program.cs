using Npgsql;
using System.Net.Sockets;
using System.Net;
using System.Text;
using MonsterTradingCards.Repositories;
using MonsterTradingCards.Models;

namespace MonsterTradingCards
{
    public class Program
    {
        public static string strConnString = "Server=localhost;Port=5432;Username=postgres;Password=mypassword;Database=monster_trading_cards";

        static void Main(string[] args)
        {
            UserRepository userRepository = new UserRepository(strConnString);
            User user = userRepository.Get(Guid.NewGuid());

            Console.WriteLine("Hello, World!");
            //PostgreSQLConnection();

            string ipAddress = "127.0.0.1";
            int port = 8080;

            TcpListener listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            listener.Start();

            Console.WriteLine($"Listening for requests at {ipAddress}:{port}");

            while (true)
            {
                using (TcpClient client = listener.AcceptTcpClient())
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] requestBuffer = new byte[1024];
                    int bytesRead = stream.Read(requestBuffer, 0, requestBuffer.Length);
                    string request = Encoding.ASCII.GetString(requestBuffer, 0, bytesRead);

                    Console.WriteLine($"Received request:\n{request}");

                    string responseBody = "Response: " + request; // Echo the request

                    string response = $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {responseBody.Length}\r\n\r\n{responseBody}";
                    byte[] responseBuffer = Encoding.ASCII.GetBytes(response);

                    stream.Write(responseBuffer, 0, responseBuffer.Length);
                }
            }
        }

        static void PostgreSQLConnection()
        {
      
            try
            {
                NpgsqlConnection objpostgraceConn = new NpgsqlConnection(strConnString);
                objpostgraceConn.Open();
                string strpostgracecommand = "select * from users";
                NpgsqlDataAdapter objDataAdapter = new NpgsqlDataAdapter(strpostgracecommand, objpostgraceConn);
                objpostgraceConn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}