﻿//Frassineti Leonardo 4H
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
            Console.Title = "Frassineti Leonardo 4H";
            //Message received to null 
            string data = null;
            //Array of bytes to send as message
            byte[] bytes = new byte[1024];

            Console.WriteLine("\nProgramma Server Frassineti Leonardo\n");
            //Establish the local endpoint for the socket
            //Dns.GetHostName returns the name of the host running the application
#if false
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];
#elif true
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
                listener.Listen(2);

                //Start listening for connections
                while (true)
                {
                    Console.WriteLine("Waiting for connection...");
                    //Program is suspended while waiting for an incoming connection.
                    Socket handler = listener.Accept();
                    string strMsg = null;
                    Console.WriteLine("Socket connected to {0}", handler.RemoteEndPoint.ToString());
                    //An incoming connection needs to be processed
                    do
                    {
                        data = null;
                        do
                        {
                            //Receive data from client
                            int bytesRec = handler.Receive(bytes);
                            //Transforms data
                            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                            //Receives until final instruction marked with <EOF>

                        } while (data.IndexOf("<EOF>") <= -1);
                        data = data.Remove(data.IndexOf("<EOF>"), 5);
                        //Show the data on the console.
                        Console.WriteLine("Text received : {0}", data);

                        //Checks if both messages are "ciao" and proceeds to close the connection
                        if (strMsg == "ciao" && data == "ciao")
                            break;

                        #region Echo the data back to the client.
                        Console.Write("Text to send : ");
                        strMsg = Console.ReadLine();
                        byte[] msg = Encoding.ASCII.GetBytes(strMsg + "<EOF>");
                        
                        handler.Send(msg);
                        #endregion
                    } while (!(strMsg == "ciao" && data == "ciao") && handler.Connected);//Checks if both messages are "ciao" and if the connection is still on if so it closes the connection
                    //Close connection
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //Program finished
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
