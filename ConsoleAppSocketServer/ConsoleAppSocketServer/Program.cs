using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSocketServer
{
    internal class Giocatore
    {
        string alias;
        Socket scoketAlias;
        DateTime dateTimeStart;
        DateTime dateTimeEnd;

        public Socket SocketAlias { get => SocketAlias; }
        public DateTime DateTimeStart { get => DateTimeStart; }
        public DateTime DateTimeEnd { get => dateTimeEnd; set => DateTimeEnd = }
        static void Main(string[] args)
        {
        }
    }
}
