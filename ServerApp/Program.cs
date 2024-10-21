using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerApp
{
    class ServerChat
    {
        const string address = "192.168.31.160";
        const short port = 4040;
        TcpListener listener = null;

        public ServerChat()
        {
            listener = new TcpListener(new IPEndPoint(IPAddress.Parse(address),port));
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine("Waiting for connection...");
            TcpClient client =listener.AcceptTcpClient();
            Console.WriteLine("Connected!");
            NetworkStream ns = client.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);
            while (true)
            {
                string message = reader.ReadLine();
                Console.WriteLine($"{message} from {client.Client.LocalEndPoint}. Date: {DateTime.Now}");
                writer.WriteLine("Thanks");
                writer.Flush();
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
