﻿//Leonardo Frassineti 4H
using ConsoleAppSocketServerWPF;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

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
        public User(string alias, Socket socketAlias) { this.alias = alias; this.socketAlias = socketAlias; this.dateTimeStart = DateTime.Now; }
    }

    public partial class MainWindow : Window
    {
        static User clientUser;
        static object _lock = new object();
        static string data = null;
        //Array of bytes
        static byte[] bytes = new byte[1024];
        static List<string> userNames = new List<string>();
        static Socket senderServer;
        static Thread thReceiveMessages;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Btn_Send_Click(object sender, RoutedEventArgs e)
        {
            //Sends message contained in Tbx_InputMessage
            string strMsg = Tbx_InputMessage.Text;
            //Clears the chat bar
            Tbx_InputMessage.Clear();
            //Creates the msg to send
            byte[] msg = Encoding.UTF8.GetBytes($"{clientUser.Alias}: {strMsg}<EOF>");
            //Send the data through the socket.
            senderServer.Send(msg);

        }

        private void Btn_Disconnect_Click(object sender, RoutedEventArgs e)
        {
            //Shutdowns comunication on
            byte[] msg = Encoding.UTF8.GetBytes($"<LOG><EXT><EOF>");
            //Send the data through the socket.
            senderServer.Send(msg);

            thReceiveMessages.Abort();
            senderServer.Shutdown(SocketShutdown.Both);
            senderServer.Close();
            Lbx_Log.Items.Add("Exited Chat");

        }

        private void Btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            //Gets the ip address from the textbox Tbx_IPv4Input
            try
            {
                IPAddress ipAddress = IPAddress.Parse(Tbx_IPv4Input.Text);
                IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, 11000);
                //Gets the Username/Alias from the textbox Tbx_UsernameInput and creates the User
                clientUser = new User(Tbx_UsernameInput.Text, new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp));
                //Puts the client socket to
                senderServer = clientUser.SocketAlias;
                //Connects the user to the server
                clientUser.SocketAlias.Connect(remoteEndPoint);
                //Receives the user list
                Lbx_Log.Items.Add($"Socket connected to {senderServer.RemoteEndPoint}");
                //Starts a thread that sends messages to the server
                string strMsg = null;
                #region reads the message and splits the users names in a vector
                do
                {
                    int bytesRec = senderServer.Receive(bytes);
                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                } while (data.IndexOf("<EOF>") <= -1);
                data = data.Remove(data.IndexOf("<EOF>"), 5);
                #endregion
                #region checks if there are users
                if (data.StartsWith("<EMT>"))
                {
                    Lbx_Log.Items.Add("Empty Chat");
                }
                else
                {
                    var temp = data.Split('|');
                    for (int i = 0; i < temp.Length; i++)
                    {
                        userNames.Add(temp[i]);
                        Lbx_Log.Items.Add($"{temp[i]} entered the chat");
                    }
                }
                #endregion
                #region sends the client data to the server
                strMsg = clientUser.Alias;
                if (strMsg == "")
                    strMsg = "<EMT>";
                byte[] msg = Encoding.UTF8.GetBytes("<LOG>" + strMsg + "<EOF>");
                //Send the data through the socket.
                senderServer.Send(msg);
                #endregion
                //Starts a thread that receives messages to the server
                thReceiveMessages = new Thread(new ThreadStart(ReceiveMessages)); thReceiveMessages.Start();
            }
            catch
            {
                Lbx_Log.Items.Add("Couldn't connect to the server");
            }
        }
        private void ReceiveMessages()
        {
            while (true)
            {
                data = "";
                if (senderServer.Available > 0)
                {
                    lock (_lock)
                    {
                        do
                        {
                            int bytesRec = senderServer.Receive(bytes);
                            data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                        } while (data.IndexOf("<EOF>") <= -1);
                        data = data.Remove(data.IndexOf("<EOF>"), 5);
                    }
                    //Data consist in the message if it has <LOG> it means it is data sent to update things about the users
                    if (data.StartsWith("<LOG>"))
                    {
                        data = data.Remove(data.IndexOf("<LOG>"), 5);
                        if (data.Contains("<ENT>"))
                        {
                            data = data.Remove(data.IndexOf("<ENT>"), 5);
                            Dispatcher.Invoke(() =>
                            {
                                Lbx_Log.Items.Add($"{data} entered the chat");
                            });
                        }
                        if (userNames.Count > 0)
                            foreach (string user in userNames)
                                if (data.Contains(user))
                                {
                                    if (data.Contains("<EXT>"))
                                    {
                                        Dispatcher.Invoke(() =>
                                        {
                                            Lbx_Log.Items.Add($"{user} exited the chat");
                                        });
                                        userNames.Remove(user);
                                    }
                                }
                    }
                    else
                    {
                        //Format is "User Alias" : "Text"
                        Dispatcher.Invoke(() =>
                        {
                            Lbx_Chat.Items.Add(data);
                        });

                    }
                }


            }
        }
    }
}

