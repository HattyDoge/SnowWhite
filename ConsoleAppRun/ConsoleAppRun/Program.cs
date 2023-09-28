//Frassineti Leonardo 4H 2023-09-28
using System.Threading;
using static System.Console;
namespace ConsoleAppCorsa
{
    internal class Program
    {
        static int posAndrea = 0;
        static int posBaldo = 0;
        static int posCarlo = 0;
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
                SetCursorPosition(posAndrea, 5);
                Write(@"   ┘└");
                Thread.Sleep(andreaSpeed);
                SetCursorPosition(posAndrea, 4);
                Write(@"   /▓\");
                Thread.Sleep(andreaSpeed);
                SetCursorPosition(posAndrea, 3);
                Write(@" ( - L -)");
                Thread.Sleep(andreaSpeed);
            }
        }
        static void Baldo()
        {
            int baldoSpeed = 50;
            for (posBaldo = 0; posBaldo < 114; posBaldo++)
            {
                SetCursorPosition(posBaldo, 9);
                Write(@"   ╚═╝║╚═╝");
                Thread.Sleep(baldoSpeed);
                SetCursorPosition(posBaldo, 8);
                Write(@"   ╔╗╔█╗╔╗");
                Thread.Sleep(baldoSpeed);
                SetCursorPosition(posBaldo, 7);
                Write(@"    (°W°)");
                Thread.Sleep(baldoSpeed);
            }
        }
        static void Carlo()
        {
            int carloSpeed = 30;
            for (posCarlo = 0; posCarlo < WindowWidth-1; posCarlo++)
            {
                SetCursorPosition(posCarlo, 13);
                Write(@"  /\");
                Thread.Sleep(carloSpeed);
                SetCursorPosition(posCarlo, 12);
                Write(@"  ┌■┐");
                Thread.Sleep(carloSpeed);
                SetCursorPosition(posCarlo, 11);
                Write(@" (T-T)");
                Thread.Sleep(carloSpeed);
            }
        }
        static void Main(string[] args)
        {
            Title = "Frassineti Leonardo 4H 28-09-2023";
            CursorVisible = false;
            Pronti();
            //Andrea
            Thread thAndrea = new Thread(Andrea);
            //Baldo
            Thread thBaldo = new Thread(Baldo);
            //Carlo
            Thread thCarlo = new Thread(Carlo);

            thAndrea.Start();
            thBaldo.Start();
            thCarlo.Start();

            ReadLine();
        }
    }
}