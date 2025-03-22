using Project11.Models;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace Project12
{
    public partial class MainWindow : Window
    {
        private UdpClient udpClient;
        private TcpClient tcpClient;
        private NetworkStream tcpStream;
        private IPEndPoint serverEndPoint;
        private readonly string host = "127.0.0.1";
        private readonly int port = 5000;

        public ObservableCollection<Book> Books { get; set; } = new ObservableCollection<Book>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            udpClient = new UdpClient();
            serverEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
        }

        private void AddToList_Click(object sender, RoutedEventArgs e)
        {
            Books.Add(new Book
            {
                NumberDouble = int.TryParse(txtNumberDouble.Text, out int num) ? num : null,
                UpdateDate = dpUpdateDate.SelectedDate,
                BooleanCheck = chkBooleanCheck.IsChecked ?? false,
                StringCheck = txtStringCheck.Text
            });

            ClearFields();
        }

        private async void SendToServer_Click(object sender, RoutedEventArgs e)
        {
            if (Books.Count == 0)
            {
                MessageBox.Show("No books to send!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string jsonData = SerializeData();
            if (jsonData == null) return;

            string selectedProtocol = (cmbProtocol.SelectedItem as ComboBoxItem)?.Content.ToString();
            switch (selectedProtocol)
            {
                case "TCP":
                    await SendViaTcp(jsonData);
                    break;
                case "UDP":
                    await SendViaUdp(jsonData);
                    break;
                default:
                    MessageBox.Show("Invalid protocol selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        private string SerializeData()
        {
            try
            {
                return JsonSerializer.Serialize(Books, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Serialization error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private async Task SendViaUdp(string jsonData)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(jsonData);
                await udpClient.SendAsync(data, data.Length, serverEndPoint);
                MessageBox.Show("Data sent via UDP!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"UDP error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EnsureTcpConnected()
        {
            if (tcpClient == null || !tcpClient.Connected)
            {
                try
                {
                    tcpClient = new TcpClient(host, port);
                    tcpStream = tcpClient.GetStream();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Cannot connect to server: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task SendViaTcp(string jsonData)
        {
            EnsureTcpConnected();
            if (tcpStream == null) return;

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(jsonData);
                await tcpStream.WriteAsync(data, 0, data.Length);
                await tcpStream.FlushAsync();

                byte[] buffer = new byte[1024];
                int bytesRead = await tcpStream.ReadAsync(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                MessageBox.Show($"Server Response: {response}", "Response", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"TCP error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                tcpClient?.Close();
                tcpClient = null;
            }
        }

        private void ClearFields()
        {
            txtNumberDouble.Clear();
            dpUpdateDate.SelectedDate = null;
            chkBooleanCheck.IsChecked = false;
            txtStringCheck.Clear();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            udpClient?.Close();
            tcpStream?.Close();
            tcpClient?.Close();
        }
    }
}
