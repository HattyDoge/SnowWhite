using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
// using SharedProject; --> necessario ma io non ho fatto in tempo a crearlo separato, le classi sono qui sotto
using System.Threading.Tasks;

namespace server
{
	internal class Giocatore
	{
		private string alias;
		private Socket socketAlias;
		private DateTime dateTimeStart;
		private DateTime dateTimeEnd;

		public string Alias { get => alias; }
		public Socket SocketAlias { get => socketAlias; }
		public DateTime DateTimeStart { get => dateTimeStart; }
		public DateTime DateTimeEnd { get => dateTimeEnd; set => DateTimeEnd = value; }

		public Giocatore(string alias, Socket socketAlias)
		{
			this.alias = alias;
			this.socketAlias = socketAlias;
			this.dateTimeStart = DateTime.Now;
		}
	}
	internal class ListaGiocatori
	{
		private List<Giocatore> playersList;

		public ListaGiocatori()
		{
			PlayersList = new List<Giocatore>();
		}

		internal List<Giocatore> PlayersList { get => playersList; set => playersList = value; }

		public void AddGiocatore(string alias, Socket socketAlias)
		{
			Giocatore giocatore = new Giocatore(alias, socketAlias);
			PlayersList.Add(giocatore);
		}

		public void InviaListaAlias(Socket socketAlias)
		{
			string lista = "";
			if (playersList.Count > 0)
			{
				lista = playersList[0].Alias;
				for (int i = 1; i < PlayersList.Count; i++) { lista = lista + ";" + PlayersList[i].Alias; }
			}
			lista = lista + "<EOF>";
			byte[] msg = Encoding.ASCII.GetBytes(lista);
			socketAlias.Send(msg);
		}

		public void InvianuovoATutti(string alias)
		{
			foreach (Giocatore g in PlayersList)
			{
				byte[] msg = Encoding.ASCII.GetBytes(alias + "<NEW>");
				g.SocketAlias.Send(msg);
			}
		}
	}
	internal class Program
	{
		private const int MAXPLAYERS = 10;
		static object lockListaGiocatori = new object();
		private static ListaGiocatori listaGiocatori = new ListaGiocatori();

		static private void GestisciClient(Socket handler, string alias)
		{
			byte[] bytes = new byte[1024];
			int bytesRec;
			string messaggio;

			do
			{
				messaggio = "";
				do
				{
					bytesRec = handler.Receive(bytes);
					messaggio += Encoding.ASCII.GetString(bytes, 0, bytesRec);
				} while ((messaggio[messaggio.Length - 1] != '>') && (messaggio[messaggio.Length - 5] != '<'));
				string tag = messaggio.Substring(messaggio.Length - 5);
				messaggio = messaggio.Remove(messaggio.Length - 5);

				switch (tag)
				{
					case "<CHA>":
						lock (lockListaGiocatori)
						{
							foreach (Giocatore p in listaGiocatori.PlayersList)
							{
								byte[] msg = Encoding.ASCII.GetBytes(alias + ";" + messaggio + "<CHA>");
								p.SocketAlias.Send(msg);
							}
						}
						break;
				}
			} while (true);

		}

		static void Main(string[] args)
		{
			string alias = null;
			byte[] bytes = new byte[1024];

			IPAddress ipAddress = IPAddress.Loopback;
			IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

			Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			try
			{
				listener.Bind(localEndPoint);
				listener.Listen(MAXPLAYERS);


				while (true)
				{
					Console.WriteLine("waiting for connections...");
					Socket handler = listener.Accept();
					alias = null;

					do
					{
						int bytesRec = handler.Receive(bytes);
						alias += Encoding.ASCII.GetString(bytes, 0, bytesRec);
					} while (alias.IndexOf("<EOF>") == -1);

					alias = alias.Remove(alias.Length - 5);

					lock (lockListaGiocatori)
					{
						listaGiocatori.InvianuovoATutti(alias);
						listaGiocatori.AddGiocatore(alias, handler);
						listaGiocatori.InviaListaAlias(handler);
					}
					Task.Run(() => GestisciClient(handler, alias));
					Console.WriteLine("giocatore: '{0}' collegato", alias);
				}
			}
			catch
			{

			}
		}
	}
}