﻿//Leonardo Frassineti 4H
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class User
    {
        string alias;
        Socket socketAlias;
        DateTime dateTimeStart;
        DateTime dateTimeEnd;
        bool master;
		bool guessedRight = false;
        public int score = 0;
        public int Score { get => score; set => score += value; }
		public bool GuessedRight { get => guessedRight; set => guessedRight = value; }
		public string Alias { get => alias; }
        public Socket SocketAlias { get => socketAlias; }
        public DateTime DateTimeStart { get => dateTimeStart; }
        public DateTime DateTimeEnd { get => dateTimeEnd; set => dateTimeEnd = value; }
        public bool Master { get => master; set => master = value; }
        public User(string alias, Socket socketAlias) { this.alias = alias; this.socketAlias = socketAlias; this.dateTimeStart = DateTime.Now; master = false; }
    }
    internal class UserList
    {
        int masterIndex;
        List<User> usersList;
        public int MasterIndex { get => masterIndex; }
		internal List<User> UsersList { get => usersList; set => usersList = value; }
        public UserList() { UsersList = new List<User>(); masterIndex = -1; }
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
        /// <summary>
        ///
        /// </summary>
        /// <param name="socketAlias">Socket del client a cui inviare la lista</param>
        public void InviaListaAlias(Socket socketAlias)
        {
            string lista = "";
            if (UsersList.Count > 0)
            {
                lista = UsersList[0].Alias;
                for (int i = 1; i < UsersList.Count; i++)
                    lista = lista + ";" + UsersList[i].Alias;

            }
        }
        public void BecomeMaster(int index)
        {
            usersList[index].Master = true;
            masterIndex = index;
        }
        public void MasterBecomeGuesser()
        {
            if(masterIndex >= 0)
            usersList[masterIndex].Master = false;
        }
        public User this[int i]
        {
            get => usersList[i];
            set => usersList[i] = value;
        }
    }
    internal class Program
    {
        static string[] wordsDB = {"scopa", "vecchietta", "montagna","tavolo","telefono","monitor","maglia"};
        static int endGame = 0;
		static string wordToGuess = "";
        static byte[] bytes = new byte[1024];
        static object _lock = new object();
        static int addScore = 100;
        static int defaultScore = 100; 
        static Thread thReceiveMessages;
        static UserList userList = new UserList();
        static void Main(string[] args)
        {
            Console.Title = "Frassineti Leonardo 4H";


            Console.WriteLine("\nProgramma Server Frassineti Leonardo 4H 22-04-2024\n");
            //Establish the local endpoint for the socket
            //Dns.GetHostName returns the name of the host running the application
            
            IPAddress ipAddress = IPAddress.Any;//Fornisce un indirizzo IP che indica che il server deve attendere 
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            //Create a TCP/IP socket
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //Bind the socket to the local endpoint and listen for incoming connections.
            while (true)
            {
                try
                {
                    //Associate the IP address and the port
                    listener.Bind(localEndPoint);
                    //Number of maximum client
                    listener.Listen(10);

                    thReceiveMessages = new Thread(new ThreadStart(ReceiveMessages));
                    thReceiveMessages.Start();
                    ReceiveClients(listener);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                //Program finished
            }
        }
        static void ReceiveClients(Socket listener)
        {
            //Start listening for connections
            while (true)
            {
                //Message received to null 
                string data = null;
                //Array of bytes to send as message
                Console.WriteLine("Waiting for connection...");
                //Program is suspended while waiting for an incoming connection.
                Socket handler = listener.Accept();
                string strMsg = null;
                Console.WriteLine("Socket connected to {0}", handler.RemoteEndPoint.ToString());
                //An incoming connection needs to be processed
                #region Sends Basic information
                lock (_lock)
                {
                    if (userList.UsersList.Count != 0)
                    {
                        for (int i = 0; i < userList.UsersList.Count; i++)
                        {
                            strMsg += userList[i].Alias + "|";
                        }
                        strMsg = strMsg.Remove(strMsg.LastIndexOf('|'));

                        byte[] msg = Encoding.UTF8.GetBytes(strMsg + "<EOF>");
                        handler.Send(msg);
                        data = null;
                    }
                    else
                    {
                        byte[] msg = Encoding.UTF8.GetBytes("<EMT>" + "<EOF>");
                        handler.Send(msg);
                    }
                    #endregion
                    #region gets the user alias and adds it to the list
                    do
                    {
                        //Receive data from client and Transforms data
                        data += Encoding.UTF8.GetString(bytes, 0, handler.Receive(bytes));
                        //Receives until final instruction marked with <EOF>
                    } while (data.IndexOf("<EOF>") <= -1);
                    data = data.Remove(data.IndexOf("<EOF>"), 5);
                    data = data.Remove(data.IndexOf("<LOG>"), 5);
                    userList.AddUser(data, handler);
                    #endregion
                    for (int i = 0; i < userList.UsersList.Count; i++)
                    {
                        userList[i].SocketAlias.Send(Encoding.UTF8.GetBytes("<LOG><ENT>" + data + "<EOF>"));
                    }
                }
                Thread.Sleep(1);
                StartMatch();
                //Show the data on the console.
                //Checks if both messages are "ciao" and proceeds to close the connection
            }
        }
        static void StartMatch()
        {
            //Waits so that messages don't get mix together while sending
            Thread.Sleep(1);
            if (userList.UsersList.Count > 1)
            {
                #region Choosing the master and giving him a word to draw
                Random random = new Random();
                int iMaster = random.Next(userList.UsersList.Count);
                userList.MasterBecomeGuesser();
                userList.BecomeMaster(iMaster);
                wordToGuess = wordsDB[random.Next(wordsDB.Length)];
                userList[iMaster].SocketAlias.Send(Encoding.UTF8.GetBytes($"<LOG><MST>{wordToGuess}<EOF>"));
                #endregion
                #region Encrypting the word and sending it to all guessers
                for (int i = 0; i < userList.UsersList.Count; i++)
                {
                    userList[i].GuessedRight = false;
                    if (i == iMaster)
                        continue;
                    string temp = wordToGuess[0] + " ";
                    for (int j = 1; j < wordToGuess.Length - 1; j++)
                    {
                        temp += "_ ";
                    }
                    temp += wordToGuess[wordToGuess.Length - 1];
                    userList[i].SocketAlias.Send(Encoding.UTF8.GetBytes($"<LOG><GSR>{temp}<EOF>"));
                }
                #endregion
                //Waits so that messages don't get mix together while sending
                Thread.Sleep(1);
                // Sends to all the users the username of the master

                for (int i = 0; i < userList.UsersList.Count; i++)
                {
                    userList[i].SocketAlias.Send(Encoding.UTF8.GetBytes($"<LOG><END>{userList[userList.MasterIndex].Alias}<EOF>"));
                }
            }
            else
                // STP = Stop ferma il round in attesa di utenti
                userList[0].SocketAlias.Send(Encoding.UTF8.GetBytes($"<LOG><STP><EOF>"));
            addScore = defaultScore;
            Thread.Sleep(1);
            //Invia lo score a tutti
            for (int i = 0;i < userList.UsersList.Count;i++)
            {
                userList[i].SocketAlias.Send(Encoding.UTF8.GetBytes("Score:<EOF>"));
                for(int j = 0; j < userList.UsersList.Count;j++)
                {
                    Thread.Sleep(1);
                    userList[i].SocketAlias.Send(Encoding.UTF8.GetBytes($"{userList[j].Alias} = {userList[j].score}<EOF>"));
                }
            }
        }
        static void ReceiveMessages()
        {
            try
            {
                string msg;
                while (true)
                {
                    msg = "";
                    lock (_lock)
                    {
                        for (int i = 0; i < userList.UsersList.Count; i++)
                        {
                            try
                            {
                                if (userList[i].SocketAlias.Available > 0)
                                {
                                    msg = "";
                                    do
                                    {
                                        msg += Encoding.UTF8.GetString(bytes, 0, userList[i].SocketAlias.Receive(bytes));
                                    } while (msg.IndexOf("<EOF>") <= -1);
                                    if (!msg.StartsWith("<DRW>"))
                                    {
                                        msg = msg.Remove(msg.IndexOf("<EOF>"), 5);
                                        if (msg.StartsWith("<LOG><EXT>"))
                                        {
                                            
                                            throw new Exception();
                                        }
                                        // Controlla se la parola è quella scelta dal master
                                        if (!userList[i].Master)
                                        {
                                            // Controlla se l'utente ha azzeccato la parola
                                            if (msg.Remove(0, userList[i].Alias.Length + 2).ToLower() == wordToGuess.ToLower())
                                            {
                                                //Gestisce le risposte e lo score
                                                userList[i].GuessedRight = true;
                                                userList[i].SocketAlias.Send(Encoding.UTF8.GetBytes("<LOG><GDR><EOF>"));
                                                userList[i].score += addScore;
                                                //Diminiusce lo score finchè non è a 30 per tenere uno aumento base
                                                if(addScore >= 40)
                                                    addScore -= 10;
                                            }
                                        }

                                        Console.WriteLine(msg);

                                        msg = $"{msg}<EOF>";
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                //Gestisce le disconnessioni
                                msg = "<LOG>" + userList[i].Alias + "<EXT><EOF>";
                                Console.WriteLine(userList[i].Alias + " Exited the chat");
                                userList.UsersList.RemoveAt(i);
                                //Ricomincia
                                StartMatch();
                                if (userList.UsersList.Count == 1)
                                    endGame = 1;
                            }
                        }
                        // Controlla se tutti tranne il master abbiano risposto giusto
                        for (int i = 0; i < userList.UsersList.Count; i++)
                        {
                            if(!userList[i].Master)
                                if (userList[i].GuessedRight)
                                    endGame ++;

                            userList[i].SocketAlias.Send(Encoding.UTF8.GetBytes(msg));
                        }
                        //Ricomincia il match
						if (endGame == userList.UsersList.Count-1 && userList.UsersList.Count > 1)
						{
							StartMatch();
						}
                        endGame = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
        }
    }
}
