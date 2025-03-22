using System;
using System.Threading.Tasks;
namespace Project11
{
    public class Program
    {
        public static void Main()
        {
            ServerApp.StartTcpServer();
            //ServerApp.StartUdpServer();

            Console.WriteLine("Server is running... Press any key to exit.");
            Console.ReadKey();
        }
    }
}