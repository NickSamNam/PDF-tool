using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Activation_server {
    class Program {
        private static bool _running;

        static void Main(string[] args) {
            if (DatabaseHandler.IsOnline())
                _running = true;
            else
                Console.WriteLine("Database not running.");
            Console.CancelKeyPress += OnCancel;
            new Thread(ConsoleLoop).Start();
            new Thread(ServerLoop).Start();
        }

        private static void ServerLoop() {
            var listener = new TcpListener(IPAddress.Any, 420);
            listener.Start();
            while (_running) {
                var c = listener.AcceptTcpClient();
                var client = new Client(c);
                new Thread(() => client.Accept()).Start();
            }
            listener.Stop();
        }

        private static void ConsoleLoop() {
            var db = new DatabaseHandler();
            while (_running) {
                Console.Write("CM> ");
                switch (Console.ReadLine()) {
                    case "genkey":
                        Console.WriteLine(db.AddProductKey());
                        break;
                    case "dekey":
                        Console.Write("Product key: ");
                        Console.WriteLine(db.DeactivateProduct(Console.ReadLine()));
                        break;
                    case "help":
                        ShowHelp();
                        break;
                    default:
                        Console.WriteLine("Type help to see commands.");
                        break;
                }
            }
        }

        private static void ShowHelp() {
            Console.WriteLine("-----------------COMMANDS-----------------");
            Console.WriteLine("genkey:\tGenerate a new product key.");
            Console.WriteLine("dekey:\tDeactivate a product key. ");
            Console.WriteLine("------------------------------------------");
        }

        private static void OnCancel(object sender, ConsoleCancelEventArgs e) {
            _running = false;
        }
    }
}