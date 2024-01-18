//Frassineti Leonardo 4H
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Channels;

namespace ConsoleAppClient
{
    internal class Client
    {
        static void Main(string[] args)
        {
            Console.Title = "Frassineti Leonardo 4H";
            string data = null;
            //Array of bytes
            byte[] bytes = new byte[1024];

            Console.WriteLine("\nProgramma Client Frassineti Leonardo\n");

            //Establish the remote endpoint for the socket
            //Dns.GetHostName returns the name of the host running the application
#if true
            IPAddress ipAddress = IPAddress.Parse("10.1.0.8");
#else
            IPAddress ipAddress = IPAddress.Loopback;
#endif
            IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, 11000);
            //Create a TCP/IP socket
            Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //Connect the remote endpoint and send
            try
            {
                sender.Connect(remoteEndPoint);
                //Istruction for debug
                Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());
                //Encode the data string into a byte array
                string strMsg = null;
                do
                {
                    
                    #region reads the message and sends it in bytes
                    Console.Write("Text to send : ");
                    strMsg = Console.ReadLine();
                    //Creates the msg to send
                    byte[] msg = Encoding.ASCII.GetBytes(strMsg + "<EOF>");
                    
                    //Send the data through the socket.
                    int bytesSent = sender.Send(msg);
                    //Checks if both messages are "ciao" and if the connection is still on if so it closes the connection
                    if (strMsg == "ciao" && data == "ciao")
                        break;
                    #endregion

                    #region receive data and removes protocol EOF
                    data = null;
                    do
                    {
                        int bytesRec = sender.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    } while (data.IndexOf("<EOF>") <= -1);
                    data = data.Remove(data.IndexOf("<EOF>"), 5);
                    #endregion
                    //Prints to screen message received
                    Console.WriteLine("Text received : {0}", data);

                } while (!(strMsg == "ciao" && data == "ciao") && sender.Connected);

                //Release the socket.
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            //Program finished
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
