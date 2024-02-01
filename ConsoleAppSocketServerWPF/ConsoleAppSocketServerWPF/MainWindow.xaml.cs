//Leonardo Frassineti 4H
using ConsoleAppSocketServerWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace SharedProject
{
    internal class UserList
    {
        List<User> usersList;
        internal List<User> UsersList { get => usersList; set => usersList = value; }
        public UserList() { UsersList = new List<User>(); }
        /// <summary>
        /// Crea ed aggiunge alla lista un nuovo giocatore
        /// </summary>
        /// <param name="alias">Nome del nuovo giocatore </param>
        /// <param name="socketAlias">Socket del nuovo giocatore</param>
        public void AddUser(string alias, Socket socketAlias)
        {
            User user = new User(alias, socketAlias);
            UsersList.Add(user);
        }
        ///<summary>
        ///
        /// </summary>
        /// <param name="socketAlias">Socket del client a cui inviare la lista</param>
        public void InviaListaAlias(Socket socketAlias)
        {
            string lista = "";
            if (UsersList.Count > 0)
            {
                lista = UsersList[0].Alias;
                for(int i = 1; i < UsersList.Count; i++)
                    lista = lista + ";" + UsersList[i].Alias;

            }
        }
    }
}
namespace ConsoleAppSocketServerWPF
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    /// 
    internal class User
    {
        string alias;
        Socket socketAlias;
        DateTime dateTimeStart;
        DateTime dateTimeEnd;

        public string Alias { get => alias; }
        public Socket SocketAlias { get => socketAlias; }
        public DateTime DateTimeStart { get => dateTimeStart; }
        public DateTime DateTimeEnd { get => dateTimeEnd; set => dateTimeEnd = value; }
        public User (string alias, Socket socketAlias) { this.alias = alias; this.socketAlias = socketAlias; this.dateTimeStart = DateTime.Now; }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string data = null;
            //Array of bytes
            byte[] bytes = new byte[1024];


            //Establish the remote endpoint for the socket
            //Dns.GetHostName returns the name of the host running the application


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
        }
        private void Btn_Send_Click(object sender, RoutedEventArgs e)
        {
            //Sends message contained in Tbx_InputMessage
            string strMsg = Tbx_InputMessage.Text;
            //Creates the msg to send
            byte[] msg = Encoding.ASCII.GetBytes(strMsg + "<EOF>");

            //Send the data through the socket.
            int bytesSent = sender.Send(msg);
        }

        private void Btn_Disconnect_Click(object sender, RoutedEventArgs e)
        {
            //Shutdowns comunication on 
        }

        private void Btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            //Gets the ip address from the textbox Tbx_IPv4Input
            IPAddress ipAddress = IPAddress.Parse(Tbx_IPv4Input.Text);
            IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, 11000);
            //Gets the Username/Alias from the textbox Tbx_UsernameInput and creates the User
            User client = new User(Tbx_UsernameInput.Text,new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp));
            //Connects the user to the server
            //Receives the user list
            //Starts a thread that sends messages to the server
            //Starts a thread that receives messages to the server
        }

    }
}
