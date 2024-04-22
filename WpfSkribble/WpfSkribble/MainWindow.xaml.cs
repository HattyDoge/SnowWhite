//Leonardo Frassineti 4H
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Net;
using System.Threading;
using System.IO;
using System.Xml;
using XamlWriter = System.Windows.Markup.XamlWriter;
using XamlReader = System.Windows.Markup.XamlReader;

namespace WpfSkribble
{
    internal class User
    {
        string alias;
        Socket socketAlias;
        DateTime dateTimeStart;
        DateTime dateTimeEnd;
        bool master = false;
        bool guessedRight = false;
		public bool GuessedRight { get => guessedRight; set => guessedRight = value; }
		public string Alias { get => alias; }
        public Socket SocketAlias { get => socketAlias; }
        public DateTime DateTimeStart { get => dateTimeStart; }
        public DateTime DateTimeEnd { get => dateTimeEnd; set => dateTimeEnd = value; }
        public bool Master { get => master; set => master = value; }
        public User(string alias, Socket socketAlias) { this.alias = alias; this.socketAlias = socketAlias; this.dateTimeStart = DateTime.Now; }
    }
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
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
        #region Funzioni per disegnare
        Point currentPoint = new Point();
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                currentPoint = e.GetPosition(this); // Memorizza la coordinata iniziale per utilizzarla dopo per creare la linea
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Line line = new Line(); //Crea una nuova linea
                line.Stroke = SystemColors.WindowFrameBrush;

                //Punto inizio linea
                line.X1 = currentPoint.X;
                line.Y1 = currentPoint.Y;
                //Ultimo punto dela linea
                line.X2 = e.GetPosition(this).X;
                line.Y2 = e.GetPosition(this).Y;
                //Si mette come nuovo inizio della linea la fine della vecchia così da poterla creare continua
                currentPoint = e.GetPosition(this);
                //Disegnamo i movimenti
                Canvas_Draw.Children.Add(line);
                //trasforma in testo la linea
                string stLine = $"<DRW>{line.X1}|{line.Y1}|{line.X2}|{line.Y2}<EOF>";
                //Creates the msg to send
                byte[] msg = Encoding.UTF8.GetBytes(stLine);
                //Send the data through the socket.
                senderServer.Send(msg);
                //traduce il testo in linea

            }
        }
        private void Btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            Canvas_Draw.Children.Clear();
            Canvas_Result.Children.Clear();
            senderServer.Send(Encoding.UTF8.GetBytes("<DRW><CLR><EOF>"));
        }
        #endregion


        #region Funzioni per comunicare
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
            Btn_Send.IsEnabled = false;
            Tbx_InputMessage.IsEnabled = false;
            Lbx_WordToFind.Visibility = Visibility.Hidden;
            Lbl_WordToFind.Visibility = Visibility.Hidden;
            Lbl_WordToDraw.Visibility = Visibility.Hidden;
            Canvas_Draw.Visibility = Visibility.Hidden;
            Canvas_Result.Visibility = Visibility.Hidden;
            Btn_Clear.Visibility = Visibility.Hidden;
            Btn_Disconnect.IsEnabled = false;
            Btn_Connect.IsEnabled = true;
            //Shutdowns comunication on
            lock (_lock)
            {
                byte[] msg = Encoding.UTF8.GetBytes($"<LOG><EXT><EOF>");
                //Send the data through the socket.
                senderServer.Send(msg);

                thReceiveMessages.Abort();
                senderServer.Shutdown(SocketShutdown.Both);
            }
            senderServer.Close();
            Lbx_Log.Items.Add("Exited Chat");

        }

        private void Btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (Tbx_UsernameInput.Text != "")
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
                    byte[] msg = Encoding.UTF8.GetBytes("<LOG>" + strMsg + "<EOF>");
                    //Send the data through the socket.
                    senderServer.Send(msg);
                    #endregion
                    //Starts a thread that receives messages to the server
                    thReceiveMessages = new Thread(new ThreadStart(ReceiveMessages)); thReceiveMessages.Start();
					Btn_Send.IsEnabled = true;
					Tbx_InputMessage.IsEnabled = true;
					Btn_Disconnect.IsEnabled = true;
					Btn_Connect.IsEnabled = false;
				}
                catch
                {
                    Lbx_Log.Items.Add("Couldn't connect to the server");
                }
			}
            else
            {
                Lbx_Log.Items.Add("Username Missing");
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
                    lock (_lock)
                    {
                        //DRW = Draw sono tutti messaggi che sono indirizzati ai canvas
                        if (data.StartsWith("<DRW>"))
                        {
                            data = data.Remove(data.IndexOf("<DRW>"), 5);
                            // CLR = Clear pulisce il canvas
                            if (data.Contains("<CLR>"))
                            {
                                data = data.Remove(data.IndexOf("<CLR>"), 5);
                                Dispatcher.Invoke(() =>
                                {
                                    Canvas_Draw.Children.Clear();
                                    Canvas_Result.Children.Clear();
                                });
                            }
                            string[] parts = data.Split('|');

                            if (parts.Length == 4)
                            {
                                // Traduce il messagio in arrivo in modo che sia comprensibile dal canvas
                                Dispatcher.Invoke(() =>
                                {
                                    Line line = new Line();
                                    line.Stroke = SystemColors.WindowFrameBrush;
                                    line.X1 = double.Parse(parts[0]);
                                    line.Y1 = double.Parse(parts[1]);
                                    line.X2 = double.Parse(parts[2]);
                                    line.Y2 = double.Parse(parts[3]);
                                    Canvas_Result.Children.Add(line);
                                });
                            }
                        }
                        //LOG = tutti messaggi di sistema
                        else if (data.StartsWith("<LOG>"))
                        {
                            data = data.Remove(data.IndexOf("<LOG>"), 5);
                            // ENT = Enters
                            if (data.Contains("<ENT>"))
                            {
                                data = data.Remove(data.IndexOf("<ENT>"), 5);
                                Dispatcher.Invoke(() =>
                                {
                                    Lbx_Log.Items.Add($"{data} entered the chat");
                                    userNames.Add(data);
                                });
                            }
                            // MST = Master l'utente è stato selezionato come guesser 
                            if (data.Contains("<MST>"))
                            {
                                data = data.Remove(data.IndexOf("<MST>"), 5);
                                clientUser.Master = true;
                                Dispatcher.Invoke(() =>
                                {
                                    Lbx_WordToFind.Items.Clear();
                                    Lbx_WordToFind.Visibility = Visibility.Visible;
                                    Lbl_WordToFind.Visibility = Visibility.Hidden;
                                    Lbl_WordToDraw.Visibility = Visibility.Visible;
                                    Lbx_WordToFind.Items.Add($"{data}");
                                    Canvas_Draw.Visibility = Visibility.Visible;
                                    Canvas_Result.Visibility = Visibility.Hidden;
                                    Btn_Clear.Visibility = Visibility.Visible;
                                });
                            }
                            // GSR = Guesser l'utente è stato selezionato come guesser 
                            if (data.Contains("<GSR>"))
                            {
                                data = data.Remove(data.IndexOf("<GSR>"), 5);
                                clientUser.Master = false;
                                Dispatcher.Invoke(() =>
                                {
                                    Lbx_WordToFind.Items.Clear();
                                    Lbx_WordToFind.Visibility = Visibility.Visible;
                                    Lbl_WordToFind.Visibility = Visibility.Visible;
                                    Lbl_WordToDraw.Visibility = Visibility.Hidden;
                                    Lbx_WordToFind.Items.Add($"{data}");
                                    Canvas_Draw.Visibility = Visibility.Hidden;
                                    Canvas_Result.Visibility = Visibility.Visible;
                                    Btn_Clear.Visibility = Visibility.Hidden;
                                });
                            }
                            // GDR = Guessed Right l'utente ha indovinato la parola
                            if (data.Contains("<GDR>"))
                            {
                                data = data.Remove(data.IndexOf("<GDR>"), 5);
                                clientUser.GuessedRight = true;
                                Dispatcher.Invoke(() =>
                                {
                                    Lbx_Chat.Items.Add($"Hai indovinato !");
                                });
                            }
                            // END indica la fine del round e l'inizio
                            if (data.Contains("<END>"))
                            {
                                data = data.Remove(data.IndexOf("<END>"), 5);
                                clientUser.GuessedRight = true;
                                Dispatcher.Invoke(() =>
                                {
                                    Canvas_Draw.Children.Clear();
                                    Canvas_Result.Children.Clear();
                                    Lbx_Log.Items.Add($"Il gioco è ricominciato");
                                    Lbx_Log.Items.Add($"Il disegnatore è {data}");
                                });
                            }
                            // STP = Stop ferma il round in attesa di utenti
                            if (data.Contains("<STP>"))
                            {
                                data = data.Remove(data.IndexOf("<STP>"), 5);
                                Dispatcher.Invoke(() =>
                                {
                                    Canvas_Draw.Children.Clear();
                                    Canvas_Result.Children.Clear();
                                    Lbx_Log.Items.Add($"Il gioco è fermo per mancanza di utenti");
                                    Lbx_WordToFind.Visibility = Visibility.Hidden;
                                    Lbl_WordToFind.Visibility = Visibility.Hidden;
                                    Lbl_WordToDraw.Visibility = Visibility.Hidden;
                                    Canvas_Draw.Visibility = Visibility.Hidden;
                                    Canvas_Result.Visibility = Visibility.Hidden;
                                    Btn_Clear.Visibility = Visibility.Hidden;
                                });
                            }
                            if (userNames.Count > 0)
                                foreach (string user in userNames)
                                    if (data.Contains(user))
                                    {
                                        // EXT = Exit serve per indicare l'uscita di un user
                                        if (data.Contains("<EXT>"))
                                        {
                                            Dispatcher.Invoke(() =>
                                            {
                                                Lbx_Log.Items.Add($"{user} exited the chat");
                                            });
                                            userNames.Remove(user);
                                            break;
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
                        Dispatcher.Invoke(() =>
                        {
                            // Mantiene la chat pulita
                            if (Lbx_Chat.Items.Count > 18)
                            {
                                Lbx_Chat.Items.RemoveAt(0);
                            }
                            if (Lbx_Log.Items.Count > 18)
                            {
                                Lbx_Log.Items.RemoveAt(0);
                            }
                        });
                    }
                }


            }
        }
        #endregion


    }
}