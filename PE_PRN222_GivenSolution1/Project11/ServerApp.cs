using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Project11.Models;

namespace Project11
{
    public class ServerApp
    {
        private static List<Book> books = new List<Book>();
        private const int TcpPort = 5000;
        private const int UdpPort = 5001;
        private const string IpAddress = "127.0.0.1";

        public static void StartTcpServer()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Parse(IpAddress), TcpPort);
            tcpListener.Start();
            Console.WriteLine($"[TCP] Server listening on {IpAddress}:{TcpPort}...");

            while (true)
            {
                using TcpClient tcpClient = tcpListener.AcceptTcpClient();
                using NetworkStream networkStream = tcpClient.GetStream();
                byte[] buffer = new byte[2048];
                int count = networkStream.Read(buffer, 0, buffer.Length);
                string receivedData = Encoding.UTF8.GetString(buffer, 0, count);

                string response = ProcessReceivedData(receivedData);
                SendTcpResponse(networkStream, response);
            }
        }

        public static void StartUdpServer()
        {
            using UdpClient udpServer = new UdpClient(UdpPort);
            Console.WriteLine($"[UDP] Server listening on {IpAddress}:{UdpPort}...");

            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, UdpPort);

            while (true)
            {
                byte[] receivedBytes = udpServer.Receive(ref remoteEP);
                string receivedData = Encoding.UTF8.GetString(receivedBytes);
                Console.WriteLine($"[UDP] Received from {remoteEP}: {receivedData}");

                string response = ProcessReceivedData(receivedData);
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                udpServer.Send(responseBytes, responseBytes.Length, remoteEP);
                Console.WriteLine($"[UDP] Response sent to {remoteEP}: {response}");
            }
        }

        private static string ProcessReceivedData(string jsonData)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                List<Book>? receivedBooks = JsonSerializer.Deserialize<List<Book>>(jsonData, options);

                if (receivedBooks == null || receivedBooks.Count == 0)
                    throw new Exception("Invalid or empty JSON data.");

                books.AddRange(receivedBooks);
                WriteToScreen(receivedBooks);

                return "Accepted";
            }
            catch (JsonException)
            {
                Console.WriteLine("Received data is not a valid JSON.");
                return "Invalid JSON";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return "Processing Error";
            }
        }

        private static void WriteToScreen(List<Book> books)
        {
            Console.WriteLine($"Received {books.Count} book(s):");
            foreach (var book in books)
            {
                Console.WriteLine($"ID: {book.Id}, NumberDouble: {book.NumberDouble}, UpdateDate: {book.UpdateDate}, BooleanCheck: {book.BooleanCheck}, StringCheck: {book.StringCheck}");
            }
        }

        private static void SendTcpResponse(NetworkStream stream, string response)
        {
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            stream.Write(responseBytes, 0, responseBytes.Length);
            Console.WriteLine($"[TCP] Response sent: {response}");
        }
    }
}
