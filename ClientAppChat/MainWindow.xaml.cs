using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientAppChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IPEndPoint serverEndPoint;
        TcpClient tcpClient;
        NetworkStream ns = null;
        StreamWriter writer = null;
        StreamReader reader = null;
        ObservableCollection<MessageInfo> messages = new ObservableCollection<MessageInfo>();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = messages;
            tcpClient = new TcpClient();
            string address = ConfigurationManager.AppSettings["serverAddres"]!;
            short port = short.Parse(ConfigurationManager.AppSettings["serverPort"]!);

            serverEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
        }

        private void Disconnect_Button_click(object sender, RoutedEventArgs e)
        {
            ns.Close();
            tcpClient.Close();
        }

        private void Connect_Button_click(object sender, RoutedEventArgs e)
        {
            //string nickname = Nickname.Text;
            try
            {
                tcpClient.Connect(serverEndPoint);
                ns = tcpClient.GetStream();
                writer = new StreamWriter(ns);
                reader = new StreamReader(ns);
                Listen();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Send_Button_click(object sender, RoutedEventArgs e)
        {
            
            string message = Msg_text.Text;
            if (!string.IsNullOrWhiteSpace(message))
            {
                writer.WriteLine(message);
                writer.Flush();
            }
            else
            {
                MessageBox.Show("Повідомлення не може бути порожнім.");
            }
            
            Msg_text.Clear();
        }

        private async void Listen()
        {
            
            while (true)
            {
                string? message = await reader.ReadLineAsync();
                messages.Add(new MessageInfo(message, DateTime.Now));
            }
        }
    }
}