using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleAppServer
{
    internal class Server
    {
        static void Main(string[] args)
        {
            string data = null;

            byte[] bytes = new byte[1024];

            Console.WriteLine("\nProgramma Server\n");
            //Establish the local endpoint for the socket
            //Dns.GetHostName returns the name of the host running the application
        #if true
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];
        #elif false
            IPAddress ipAddress = IPAddress.Any;//Fornisce un indirizzo IP che indica che il server deve attendere l'attività dei client su tutte le interfacce di rete. Questo campo è di sola lettura
        #else
            IPAddress ipAddress = IPAddress.Parse("192.168.0.8");
        #endif
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            //Create a TCP/IP socket
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                //Start listening for connections
                while(true)
                {
                    Console.WriteLine("Waiting for connection...");
                    //Program is suspended while waiting for an incoming connection.
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
