using MonsterTradingCards.Webserver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;



namespace FHTW.Swen1.Swamp
{
    /// <summary>This class provides HTTP server event arguments.</summary>
    public class HttpSvrEventArgs: EventArgs
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // protected members                                                                                                //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>TCP client.</summary>
        protected TcpClient _Client;



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // constructors                                                                                                     //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="client">TCP client object.</param>
        /// <param name="plainMessage">HTTP plain message.</param>
        public HttpSvrEventArgs(TcpClient client, string plainMessage) 
        {
            _Client = client;
            PlainMessage = plainMessage;
            Payload = string.Empty;
            
            string[] lines = plainMessage.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
            bool inheaders = true;
            List<HttpHeader> headers = new();
            List<HttpPathParams> pathParams = new();

            for(int i = 0; i < lines.Length; i++) 
            {
                if(i == 0)
                {
                    string[] inc = lines[0].Split(' ');
                    Method = inc[0];
                    string[] fullPathSplitted = inc[1].Split("?");
                    Path = fullPathSplitted[0];

                    if (fullPathSplitted.Length > 1)
                    {
                        string[] paramsOfPath = fullPathSplitted[1].Split("&");
                        foreach (string param in paramsOfPath)
                        {
                            string[] keyValueSplit = param.Split("=");
                            pathParams.Add(new HttpPathParams(keyValueSplit[0], keyValueSplit[1]));

                        }
                    }
                }
                else if(inheaders)
                {
                    if(string.IsNullOrWhiteSpace(lines[i]))
                    {
                        inheaders = false;
                    }
                    else { headers.Add(new HttpHeader(lines[i])); }
                }
                else
                {
                    if(!string.IsNullOrWhiteSpace(Payload)) { Payload += "\r\n"; }
                    Payload += lines[i];
                }
            }

            Headers = headers.ToArray();
            PathParams = pathParams.ToArray();
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // public properties                                                                                                //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        /// <summary>Gets the plain HTTP message.</summary>
        public string PlainMessage
        {
            get; protected set;
        }


        /// <summary>Gets the HTTP method.</summary>
        public virtual string Method
        {
            get; protected set;
        } = string.Empty;


        /// <summary>Gets the request path.</summary>
        public virtual string Path
        {
            get; protected set;
        } = string.Empty;

        /// <summary>Gets the HTTP path params.</summary>
        public virtual HttpPathParams[] PathParams
        {
            get; protected set;
        }

        /// <summary>Gets the HTTP hgeaders.</summary>
        public virtual HttpHeader[] Headers
        {
            get; protected set;
        }


        /// <summary>Gets the HTTP payload.</summary>
        public virtual string Payload
        {
            get; protected set;
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // public methods                                                                                                   //
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>Returns a reply to the HTTP request.</summary>
        /// <param name="status">Status code.</param>
        /// <param name="payload">Payload.</param>
        public virtual void Reply(int status, string? payload = null)
        {
            string data;

            switch(status)
            {
                case 200:
                    data = "HTTP/1.1 200 OK\n"; break;
                case 400:
                    data = "HTTP/1.1 400 Bad Request\n"; break;
                case 404:
                    data = "HTTP/1.1 404 Not Found\n"; break;
                default:
                    data = "HTTP/1.1 400 Bad Request: I dont know what happened\n"; break;
            }
            
            if(string.IsNullOrEmpty(payload)) 
            {
                data += "Content-Length: 0\n";
            }
            data += "Content-Type: text/plain\n\n";


            if (!string.IsNullOrEmpty(payload))
            {
                string format = PathParams.FirstOrDefault(pathParam => pathParam.Name.Equals("format"))?.Value;
                if (format == "plain")
                {
                    dynamic jsonObj = JsonConvert.DeserializeObject(payload);
                    data += ConvertToJsonStringWithSpaces(jsonObj);
                }
                else
                {
                    data += payload;
                }
            }

            byte[] buf = Encoding.ASCII.GetBytes(data);
            _Client.GetStream().Write(buf, 0, buf.Length);
            _Client.Close();
            _Client.Dispose();
        }

        private string ConvertToJsonStringWithSpaces(JToken token)
        {
            if (token.Type == JTokenType.Object)
            {
                var properties = token.Children<JProperty>();
                List<string> propertyStrings = new List<string>();

                foreach (var property in properties)
                {
                    propertyStrings.Add($"{property.Name}:{ConvertToJsonStringWithSpaces(property.Value)}");
                }

                return string.Join("\n", propertyStrings);
            }
            else if (token.Type == JTokenType.Array)
            {
                var items = token.Children();
                List<string> itemStrings = new List<string>();

                foreach (var item in items)
                {
                    itemStrings.Add(ConvertToJsonStringWithSpaces(item));
                }

                return string.Join("\n", itemStrings);
            }
            else
            {
                return token.ToString();
            }
        }
    }
}
