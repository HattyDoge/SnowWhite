//Leonardo Frassineti 4H
using ConsoleAppSocketServerWPF;
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
namespace SharedProject
{
    internal class ListaGiocatori
    {
        List<Giocatore> playersList;
        internal List<Giocatore> PlayersList { get => playersList; set => playersList = value; }
        public ListaGiocatori() { PlayersList = new List<Giocatore>(); }
        /// <summary>
        /// Crea ed aggiunge alla lista un nuovo giocatore
        /// </summary>
        /// <param name="alias">Nome del nuovo giocatore </param>
        /// <param name="socketAlias">Socket del nuovo giocatore</param>
        public void AddGIocatore(string alias, Socket socketAlias)
        {
            Giocatore giocatore = new Giocatore(alias, socketAlias);
            PlayersList.Add(giocatore);
        }
        ///<summary>
        ///
        /// </summary>
        /// <param name="socketAlias">Socket del client a cui inviare la lista</param>
        public void InviaListaAlias(Socket socketAlias)
        {
            string lista = "";
            if (PlayersList.Count > 0)
            {
                lista = PlayersList[0].Alias;
                for(int i = 1; i < PlayersList.Count; i++)
                    lista = lista + ";" + PlayersList[i].Alias;

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
    internal class Giocatore
    {
        string alias;
        Socket socketAlias;
        DateTime dateTimeStart;
        DateTime dateTimeEnd;

        public string Alias { get => alias; }
        public Socket SocketAlias { get => socketAlias; }
        public DateTime DateTimeStart { get => dateTimeStart; }
        public DateTime DateTimeEnd { get => dateTimeEnd; set => dateTimeEnd = value; }
        public Giocatore (string alias, Socket socketAlias) { this.alias = alias; this.socketAlias = socketAlias; this.dateTimeStart = DateTime.Now; }
        static void Main(string[] args)
        {
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
