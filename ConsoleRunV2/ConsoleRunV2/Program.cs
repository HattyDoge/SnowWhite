//Frassineti Leonardo 4H 2023-09-28
//Capire come funziona il multi-threading attraverso una gara di corsa tra 3 persone
using System.Threading;
using static System.Console;
namespace ConsoleAppCorsa
{
    internal class Program
    {
        static int posAndrea = 0;
        static int posBaldo = 0;
        static int posCarlo = 0;
        static object lock_ = new object();
        static int classifica = 0;
        static string command = "";
        //Andrea
        static Thread thAndrea;
        //Baldo
        static Thread thBaldo;
        //Carlo
        static Thread thCarlo;
        class DataRunner
        {
            int rowPos;
            string legs;
            string torso;
            string head;
            public DataRunner ( int pos, string legs, string torso, string head) 
            {
                this.rowPos = pos; // posizione testa
                this.legs = legs;
                this.torso = torso;
                this.head = head;
            }
            public int RowPos { get { return rowPos; } set { rowPos = value; } }
            public string Legs { get { return legs; } }
            public string Torso { get { return torso; } }
            public string Head { get { return head; } }
        }
        static void Pronti()
        {
            WriteDown("Andrea", posAndrea, 2);
            WriteDown(@" (- L -)", posAndrea, 3);
            WriteDown(@"   /▓\", posAndrea, 4);
            WriteDown(@"   ┘└", posAndrea, 5);
            WriteDown("Baldo", posBaldo, 6);
            WriteDown(@"  (°W°)", posBaldo, 7);
            WriteDown(@" ╔╗╔█╗╔╗", posBaldo, 8);
            WriteDown(@" ╚═╝║╚═╝", posBaldo, 9);
            WriteDown("Carlo", posCarlo, 10);
            WriteDown(@" (T-T)", posCarlo, 11);
            WriteDown(@"  ┌■┐", posCarlo, 12);
            WriteDown(@"  /\", posCarlo, 13);
        }
        static void Runner(object obj)
        {
            DataRunner data = (DataRunner)obj; 
            int speed = 40;
            for (int pos = 0; pos < 114; pos++)
            {
                if (command.Length == 2)
                {
                    if (command[0] == 'A')
                        if (command[1] == 'B')
                            thBaldo.Join();
                        else if (command[1] == 'C')
                            thCarlo.Join();
                }
                WriteDown(data.Legs, pos, data.RowPos + 2);
                Thread.Sleep(speed);
                WriteDown(@"   /▓\", pos, data.RowPos + 1);
                Thread.Sleep(speed);
                WriteDown(@" (- L -)", pos, data.RowPos);
                Thread.Sleep(speed);
            }
            lock (lock_)
            {
                classifica++;
                SetCursorPosition(115, 2);
                Write(classifica);
            }
        }
        static void Andrea()
        {
            int andreaSpeed = 40;
            for (posAndrea = 0; posAndrea < 114; posAndrea++)
            {
                if (command.Length == 2)
                {
                    if (command[0] == 'A')
                        if (command[1] == 'B')
                            thBaldo.Join();
                        else if (command[1] == 'C')
                            thCarlo.Join();
                }
                WriteDown(@"   ┘└", posAndrea, 5);
                Thread.Sleep(andreaSpeed);
                WriteDown(@"   /▓\", posAndrea, 4);
                Thread.Sleep(andreaSpeed);
                WriteDown(@" (- L -)", posAndrea, 3);
                Thread.Sleep(andreaSpeed);
            }
            lock (lock_)
            {
                classifica++;
                SetCursorPosition(115, 2);
                Write(classifica);
            }
        }
        static void Baldo()
        {
            int baldoSpeed = 40;
            for (posBaldo = 0; posBaldo < 114; posBaldo++)
            {
                if (command.Length == 2)
                {
                    if (command[0] == 'B')
                        if (command[1] == 'A')
                            thAndrea.Join();
                        else if (command[1] == 'C')
                            thCarlo.Join();
                }
                WriteDown(@" ╚═╝║╚═╝", posBaldo, 9);
                Thread.Sleep(baldoSpeed);
                WriteDown(@" ╔╗╔█╗╔╗", posBaldo, 8);
                Thread.Sleep(baldoSpeed);
                WriteDown(@"  (°W°)", posBaldo, 7);
                Thread.Sleep(baldoSpeed);
            }
            lock (lock_)
            {
                classifica++;
                SetCursorPosition(115, 6);
                Write(classifica);
            }
        }
        static void Carlo()
        {
            int carloSpeed = 40;
            for (posCarlo = 0; posCarlo < 114; posCarlo++)
            {
                if (command.Length == 2)
                {
                    if (command[0] == 'C')
                        if (command[1] == 'A')
                            thAndrea.Join();
                        else if (command[1] == 'B')
                            thBaldo.Join();
                }
                WriteDown(@"  /\", posCarlo, 13);
                Thread.Sleep(carloSpeed);
                WriteDown(@"  ┌■┐", posCarlo, 12);
                Thread.Sleep(carloSpeed);
                WriteDown(@" (T-T)", posCarlo, 11);
                Thread.Sleep(carloSpeed);
            }
            lock (lock_)
            {
                classifica++;
                SetCursorPosition(115, 10);
                Write(classifica);
            }
        }
        static void ThreadStatus()
        {
            lock (lock_)
            {
                WriteDown($"{thAndrea.ThreadState}        ", 40, 2);
                if (thAndrea.IsAlive)
                    WriteDown("IsAlive == True ", 10, 2);
                else
                    WriteDown("IsAlive == False", 10, 2);

                WriteDown($"{thBaldo.ThreadState}        ", 40, 6);
                if (thBaldo.IsAlive)
                    WriteDown("IsAlive == True ", 10, 6);
                else
                    WriteDown("IsAlive == False", 10, 6);

                WriteDown($"{thCarlo.ThreadState}       ", 40, 10);
                if (thCarlo.IsAlive)
                    WriteDown("IsAlive == True ", 10, 10);
                else
                    WriteDown("IsAlive == False", 10, 10);
            }
        }
        static Thread SelectRunner()
        {
            WriteDown("a) Andrea  ", 2, 16);
            WriteDown("b) Baldo   ", 2, 17);
            WriteDown("c) Carlo   ", 2, 18);
            WriteDown("           ", 2, 19);
            SetCursorPosition(3, 20);
            char input = ReadKey().KeyChar;
            switch (input)
            {
                case 'a': // Andrea
                    return thAndrea;

                case 'b': // Baldo
                    return thBaldo;

                case 'c': // Carlo
                    return thCarlo;

                default: return null;
            }
        }
        static void Control()
        {

            char input = ReadKey().KeyChar;
            switch (input)
            {
                case 'a': // Abort
                    {
                        SelectRunner()?.Abort();
                    }
                    break;

                case 's': // Suspend
                    {
                        var runner = SelectRunner();
                        if (runner == null)
                            break;
                        if (runner.ThreadState != ThreadState.Suspended && runner.IsAlive)
                            runner.Suspend();
                    }
                    break;

                case 'r': // Resume
                    {
                        var runner = SelectRunner();
                        if (runner == null)
                            break;
                        if (runner.ThreadState == ThreadState.Suspended && runner.IsAlive)
                            runner?.Resume();
                    }
                    break;

                case 'j': // Join
                    {
                        var runner = SelectRunner();
                        if (runner == thAndrea)
                            command = "A";
                        else if (runner == thBaldo)
                            command = "B";
                        else
                            command = "C";
                        runner = SelectRunner();
                        if (runner == thAndrea)
                            command += "A";
                        else if (runner == thBaldo)
                            command += "B";
                        else
                            command += "C";
                    }
                    break;
            }
        }
        static void WriteDown(string exp, int posHorizontal, int posVertical)
        {
            lock (lock_)
            {
                SetCursorPosition(posHorizontal, posVertical);
                Write(exp);
            }
        }
        static void Main(string[] args)
        {
            //SetCursorPosition(0, 24);
            //Write("Stato Main = Running");
            Title = "Frassineti Leonardo 4H 2023-09-28";
            CursorVisible = false;
            SetCursorPosition(WindowWidth / 2 - 26, WindowHeight / 2);
            Write("PREMERE UN QUALSIASI TASTO PER INIZIARE LA GARA");
            ReadKey();
            SetCursorPosition(WindowWidth / 2 - 26, WindowHeight / 2);
            Write("                                               ");
            Pronti();
            thAndrea = new Thread(Runner);
            thAndrea.Name = "Andrea";
            thBaldo = new Thread(Runner);
            thBaldo.Name = "Baldo";
            thCarlo = new Thread(Runner);
            thCarlo.Name = "Carlo";
            ThreadStatus();

            //SetCursorPosition(0, 23);
            //Write("Stato Baldo = Unstarted  ");
            //Thread.Sleep(2000);

            thBaldo.Start();
            thAndrea.Start();
            thCarlo.Start();

            do
            {
                WriteDown("Menu", 2, 15);
                WriteDown("a) Abort   ", 2, 16);
                WriteDown("s) Suspend ", 2, 17);
                WriteDown("r) Resume  ", 2, 18);
                WriteDown("j) Join    ", 2, 19);
                ThreadStatus();
                if (KeyAvailable)
                {
                    SetCursorPosition(3, 20);
                    Control();
                }
            }
            while (thAndrea.IsAlive || thBaldo.IsAlive || thCarlo.IsAlive);

            /*
            SetCursorPosition(0, 23);
            Write("Stato Baldo = Running   ");
            
            thBaldo.Suspend();

            SetCursorPosition(0, 23);
            Write("Stato Baldo = Suspend   ");
            Thread.Sleep(2000);

            thBaldo.Resume();

            SetCursorPosition(0, 23);
            Write("Stato Baldo = Running    ");
            Thread.Sleep(2000);

            SetCursorPosition(0, 24);
            Write("Stato Main = Join    ");
            thBaldo.Join();
            Thread.Sleep(2000);
            SetCursorPosition(0, 24);
            Write("Stato Main = Running    ");*/
            /* Per fermare completamente il thread
            thBaldo.Abort();

            SetCursorPosition(0, 23);
            Write("Stato Baldo = Stopped");
            Thread.Sleep(2000);
            */
            //thAndrea.Join(); serve per far aspettare la prossima istruzione che il thread thAndrea finisca l'esecuzione
            Thread.Sleep(2000);
            for (int i = 0; i < WindowHeight; i++)
                WriteDown("                                                                                                                                                       ", 1, i);
            SetCursorPosition(WindowWidth / 2 - 11, WindowHeight / 2);
            Write("GARA FINITA");
            ReadLine();
        }
    }
}