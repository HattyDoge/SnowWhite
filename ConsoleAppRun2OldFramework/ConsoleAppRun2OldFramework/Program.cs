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
        static void Pronti()
        {
            SetCursorPosition(posAndrea, 2);
            Write("Andrea");
            SetCursorPosition(posAndrea, 3);
            Write(@" ( - L -)");
            SetCursorPosition(posAndrea, 4);
            Write(@"   /▓\");
            SetCursorPosition(posAndrea, 5);
            Write(@"   ┘└");
            SetCursorPosition(posBaldo, 6);
            Write("Baldo");
            SetCursorPosition(posBaldo, 7);
            Write(@"    (°W°)");
            SetCursorPosition(posBaldo, 8);
            Write(@"   ╔╗╔█╗╔╗");
            SetCursorPosition(posBaldo, 9);
            Write(@"   ╚═╝║╚═╝");
            SetCursorPosition(posCarlo, 10);
            Write("Carlo");
            SetCursorPosition(posCarlo, 11);
            Write(@" (T-T)");
            SetCursorPosition(posCarlo, 12);
            Write(@"  ┌■┐");
            SetCursorPosition(posCarlo, 13);
            Write(@"  /\");
        }
        static void Andrea()
        {
            int andreaSpeed = 40;
            for (posAndrea = 0; posAndrea < 114; posAndrea++)
            {
                lock (lock_)
                {
                    SetCursorPosition(posAndrea, 5);
                    Write(@"   ┘└");
                }
                Thread.Sleep(andreaSpeed);
                lock (lock_)
                {
                    SetCursorPosition(posAndrea, 4);
                    Write(@"   /▓\");
                }
                Thread.Sleep(andreaSpeed);
                lock (lock_)
                {
                    SetCursorPosition(posAndrea, 3);
                    Write(@" ( - L -)");
                }
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
                lock (lock_)
                {
                    SetCursorPosition(posBaldo, 9);
                    Write(@"   ╚═╝║╚═╝");
                }
                Thread.Sleep(baldoSpeed);
                lock (lock_)
                {
                    SetCursorPosition(posBaldo, 8);
                    Write(@"   ╔╗╔█╗╔╗");
                }
                Thread.Sleep(baldoSpeed);
                lock (lock_)
                {
                    SetCursorPosition(posBaldo, 7);
                    Write(@"    (°W°)");
                }
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
                lock (lock_)
                {
                    SetCursorPosition(posCarlo, 13);
                    Write(@"  /\");
                }
                Thread.Sleep(carloSpeed);
                lock (lock_)
                {
                    SetCursorPosition(posCarlo, 12);
                    Write(@"  ┌■┐");
                }
                Thread.Sleep(carloSpeed);
                lock (lock_)
                {
                    SetCursorPosition(posCarlo, 11);
                    Write(@" (T-T)");
                }
                Thread.Sleep(carloSpeed);
            }
            lock (lock_)
            {
                classifica++;
                SetCursorPosition(115, 10);
                Write(classifica);
            }
        }
        static void Main(string[] args)
        {
            //SetCursorPosition(0, 24);
            //Write("Stato Main = Running");
            Title = "Frassineti Leonardo 4H 2023-09-28";
            CursorVisible = false;
            Pronti();
            //Andrea
            Thread thAndrea = new Thread(Andrea);
            //Baldo
            Thread thBaldo = new Thread(Baldo);
            //Carlo
            Thread thCarlo = new Thread(Carlo);
            //SetCursorPosition(0, 23);
            //Write("Stato Baldo = Unstarted  ");
            //Thread.Sleep(2000);

            thBaldo.Start();
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


            thAndrea.Start();
          //thAndrea.Join(); serve per far aspettare la prossima istruzione che il thread thAndrea finisca l'esecuzione
            thCarlo.Start();

            ReadLine();
        }
    }
}