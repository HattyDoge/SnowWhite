using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfAppClient
{
	/// <summary>
	/// Logica di interazione per MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}
		private bool ascoltaServer = true;
		private Socket clientSocket;

		public void connetti()
		{
			byte[] bytes = new byte[1024];
			int bytesRec;

			try
			{
				IPAddress ipAddress = IPAddress.Loopback;
				IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

				clientSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				try
				{
					int attempts = 0;
					while (!clientSocket.Connected)
					{
						try
						{
							attempts++;
							clientSocket.Connect(remoteEP);
						}
						catch (SocketException)
						{
							Dispatcher.Invoke(() =>
							{
								txtMessaggiDiServizio.AppendText("Connection attempt " + attempts);
							});
						}
					}
					Dispatcher.Invoke(() =>
					{
						txtMessaggiDiServizio.AppendText("connesso al server" + "\r\n");
						txtMessaggiDiServizio.AppendText("socket connected to" + clientSocket.RemoteEndPoint.ToString() + "\r\n");
					});

					string alias = "";
					Dispatcher.Invoke(() =>
					{
						alias = TXTBOX_alias.Text;
					});

					byte[] msg = Encoding.ASCII.GetBytes(alias + "<EOF>");
					int bytesSent = clientSocket.Send(msg);

					string listaAlias = "";
					do
					{
						bytesRec = clientSocket.Receive

						listaAlias += Encoding.ASCII

					} while (listaAlias.IndexOf("<EOF>") < 1);
					listaAlias = listaAlias.Remove("<EOF>");

					task.run(() => AscoltaServer());
				}
				catch
				{

				}
			}
			catch
			{

			}
		}

		private void AscoltaServer()
		{

		}

		private void BTN_connect_Click(object sender, RoutedEventArgs e)
		{
			if (this.txtAlias.Text == string.Empty)
			{
				this.txtMessaggiDiServizio.AppendText("Inserisci l'alias\n");
				return;
			}
			txtAlias.IsEnabled = false;
			BTN_connect.IsEnabled = false;
			Task.Run(() => connetti());
		}

		private void btnSend_Click(object sender, RoutedEventArgs e)
		{
			if (clientSocket != null)
			{
				byte[] msg = Encoding.ASCII.GetBytes(txtMessaggio.Text);
				int bytesSent = clientSocket.Send(msg);
				txtMessaggio.Text = "";
			}
		}
	}
}