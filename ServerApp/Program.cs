using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerApp
{
    class ServerChat
    {
        const short port = 4040;
        const int MaxMembers = 5;  
        UdpClient server;
        IPEndPoint clientendpoint = null;
        Dictionary<IPEndPoint, string> membersWithNicknames = new Dictionary<IPEndPoint, string>();

        public ServerChat()
        {
            server = new UdpClient(port);
        }

        private void AddMember(string nickname)
        {
            if (membersWithNicknames.Count >= MaxMembers)
            {
                string fullMessage = "Server is full. Cannot join.";
                byte[] data = Encoding.UTF8.GetBytes(fullMessage);
                server.SendAsync(data, data.Length, clientendpoint);
                Console.WriteLine("Failed to add member, server is full: " + clientendpoint);
            }
            else if (!membersWithNicknames.ContainsKey(clientendpoint))
            {
                membersWithNicknames.Add(clientendpoint, nickname);
                Console.WriteLine($"Member added: {nickname} from {clientendpoint}");
            }
        }

        private void SendToAll(byte[] data)
        {
            foreach (IPEndPoint p in membersWithNicknames.Keys)
            {
                server.SendAsync(data, data.Length, p);
            }
        }

        private void RemoveMember()
        {
            if (membersWithNicknames.ContainsKey(clientendpoint))
            {
                string nickname = membersWithNicknames[clientendpoint];
                membersWithNicknames.Remove(clientendpoint);
                Console.WriteLine($"Member removed: {nickname} from {clientendpoint}");
            }
        }

        public void Start()
        {
            Console.WriteLine("Server started...");
            while (true)
            {
                byte[] data = server.Receive(ref clientendpoint);
                string message = Encoding.UTF8.GetString(data);
                Console.WriteLine($"{message} from {clientendpoint}. Date: {DateTime.Now}");

                if (message.StartsWith("$<join>"))
                {
                    string nickname = message.Substring(7);
                    AddMember(nickname);
                }
                else if (message == "$<leave>")
                {
                    RemoveMember();
                }
                else
                {
                    if (membersWithNicknames.TryGetValue(clientendpoint, out string nickname))
                    {
                        string messageWithNick = $"{nickname}: {message}";
                        SendToAll(Encoding.UTF8.GetBytes(messageWithNick));
                    }
                }
            }
        }

    }

    internal class Program
    {
        static void Main(string[] args)
        {
            ServerChat serverChat = new ServerChat();
            serverChat.Start();
        }
    }
}
