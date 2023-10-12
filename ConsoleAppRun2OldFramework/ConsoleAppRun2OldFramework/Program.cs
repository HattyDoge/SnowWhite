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
        //Andrea
        static Thread thAndrea;
        //Baldo
        static Thread thBaldo;
        //Carlo
        static Thread thCarlo;
        static void Pronti()
        {
            WriteDown("Andrea", posAndrea, 2);
            WriteDown(@" ( - L -)", posAndrea, 3);
            WriteDown(@"   /▓\", posAndrea, 4);
            WriteDown(@"   ┘└", posAndrea, 5);
            WriteDown("Baldo", posBaldo, 6);
            WriteDown(@"   (°W°)", posBaldo, 7);
            WriteDown(@"  ╔╗╔█╗╔╗", posBaldo, 8);
            WriteDown(@"  ╚═╝║╚═╝", posBaldo, 9);
            WriteDown("Carlo", posCarlo, 10);
            WriteDown(@" (T-T)", posCarlo, 11);
            WriteDown(@"  ┌■┐", posCarlo, 12);
            WriteDown(@"  /\", posCarlo, 13);
        }
        static void Andrea()
        {
            int andreaSpeed = 40;
            for (posAndrea = 0; posAndrea < 114; posAndrea++)
            {
                WriteDown(@"   ┘└", posAndrea, 5);
                Thread.Sleep(andreaSpeed);
                WriteDown(@"   /▓\", posAndrea, 4);
                Thread.Sleep(andreaSpeed);
                WriteDown(@" ( - L -)", posAndrea, 3);
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
                WriteDown(@"  ╚═╝║╚═╝",posBaldo, 9);
                Thread.Sleep(baldoSpeed);
                WriteDown(@"  ╔╗╔█╗╔╗", posBaldo, 8);
                Thread.Sleep(baldoSpeed);
                WriteDown(@"   (°W°)", posBaldo, 7);
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
            WriteDown("1) Andrea  ", 2, 16);
            WriteDown("2) Baldo   ", 2, 17);
            WriteDown("3) Carlo   ", 2, 18);
            WriteDown("          ", 2, 19);

            SetCursorPosition(3, 20);
            char input = ReadKey().KeyChar;
            switch (input)
            {
                case '1': // Andrea
                    return thAndrea;

                case '2': // Baldo
                    return thBaldo;

                case '3': // Carlo
                    return thCarlo;

                default: return null;
            }
        }
        static void Control()
        {

            char input = ReadKey().KeyChar;
            switch (input)
            {
                case '1': // Abort
                    { 
                       SelectRunner().Abort();
                    } break;

                case '2': // Suspend
                    {
                        SelectRunner().Suspend();
                    }
                    break;

                case '3': // Resume
                    {
                        SelectRunner().Resume();
                    }
                    break;

                case '4': // Join
                    {
                        SelectRunner().Join();
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
            thAndrea = new Thread(Andrea);
            thBaldo = new Thread(Baldo);
            thCarlo = new Thread(Carlo);
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
                WriteDown("1) Abort   ", 2, 16);
                WriteDown("2) Suspend ", 2, 17);
                WriteDown("3) Resume  ", 2, 18);
                WriteDown("4) Join    ", 2, 19);
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


            ReadLine();
        }
    }
}