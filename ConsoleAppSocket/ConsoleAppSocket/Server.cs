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
            //Array of bytes
            byte[] bytes = new byte[1024];

            Console.WriteLine("\nProgramma Server\n");
            //Establish the local endpoint for the socket
            //Dns.GetHostName returns the name of the host running the application
        #if false
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];
        #elif false
            IPAddress ipAddress = IPAddress.Any;//Fornisce un indirizzo IP che indica che il server deve attendere 
                                                //l'attività dei client su tutte le interfacce di rete. Questo campo è di sola lettura
        #else
            IPAddress ipAddress = IPAddress.Parse("10.1.0.8");
        #endif
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            //Create a TCP/IP socket
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                //Associate the IP address and the port
                listener.Bind(localEndPoint);
                //Number of maximum client
                listener.Listen(1);

                //Start listening for connections
                while(true)
                {
                    Console.WriteLine("Waiting for connection...");
                    //Program is suspended while waiting for an incoming connection.
                    Socket handler = listener.Accept();
                    data = null;

                    //An incoming connection needs to be processed
                    do
                    {
                        //Receive data from client
                        int bytesRec = handler.Receive(bytes);
                        //Transforms data
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        //Receives until final instruction marked with <EOF>
                    } while (data.IndexOf("<EOF>") <= -1);

                    //Show the data on the console.
                    Console.WriteLine("Text received : {0}", data);

                    #region Echo the data back to the client.
                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
