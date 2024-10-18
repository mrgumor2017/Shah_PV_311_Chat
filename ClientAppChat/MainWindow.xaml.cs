using System.Collections.ObjectModel;
using System.Configuration;
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
        UdpClient udpClient;
        ObservableCollection<MessageInfo> messages = new ObservableCollection<MessageInfo>();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = messages;
            udpClient = new UdpClient();
            string address = ConfigurationManager.AppSettings["serverAddres"]!;
            short port = short.Parse(ConfigurationManager.AppSettings["serverPort"]!);

            serverEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
        }

        private void Leave_Button_click(object sender, RoutedEventArgs e)
        {
            string message = "$<leave>";
            SendMessage(message);
        }

        private void Join_Button_click(object sender, RoutedEventArgs e)
        {
            string nickname = Nickname.Text;
            if (string.IsNullOrWhiteSpace(nickname))
            {
                MessageBox.Show("Введіть нікнейм.");
            }
            else
            {
                string message = $"$<join>{nickname}";
                SendMessage(message);
                Listen();
            }
            
        }

        private void Send_Button_click(object sender, RoutedEventArgs e)
        {
           
            string message = Msg_text.Text;
            if (!string.IsNullOrWhiteSpace(message))
            {
                SendMessage(message);
            }
            else
            {
                MessageBox.Show("Повідомлення не може бути порожнім.");
            }
            
            Msg_text.Clear();
            
        }

        private async void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            await udpClient.SendAsync(data, serverEndPoint);
        }

        private async void Listen()
        {
            while (true)
            {
                var data = await udpClient.ReceiveAsync();
                string message = Encoding.UTF8.GetString(data.Buffer);
                messages.Add(new MessageInfo(message, DateTime.Now));
            }
        }
    }
}