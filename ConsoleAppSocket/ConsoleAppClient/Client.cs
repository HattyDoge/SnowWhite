//Frassineti Leonardo
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
            string data = null;
            //Array of bytes
            byte[] bytes = new byte[1024];

            Console.WriteLine("\nProgramma Client\n");

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
                    data = null;
                    Console.Write("Text to send : ");
                    strMsg = Console.ReadLine();
                    byte[] msg = Encoding.ASCII.GetBytes(strMsg + "<EOF>");
                    //Send the data through the socket.
                    int bytesSent = sender.Send(msg);
                    do
                    {
                        int bytesRec = sender.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    } while (data.IndexOf("<EOF>") <= -1);
                    data = data.Remove(data.IndexOf("<EOF>"), 5);
                    Console.WriteLine("Text received : {0}", data);

                    //Release the socket.

                } while (!(strMsg == "ciao" && data == "ciao") && sender.Connected);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
