using System;
using System.Threading;

namespace GAP_Server
{
    class Program
    {
        public static bool IsRunning = false;

        static void Main(string[] args)
        {
            Console.Title = "GAP Server";
            IsRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.Start(16, 26950);
        }

        public static void MainThread()
        {
            Console.WriteLine("Main thread started running " + Constants.TicksPerSec + " per second");
            DateTime nextStep = DateTime.Now;

            while (IsRunning)
            {
                while(nextStep < DateTime.Now)
                {
                    GameLogic.Update();

                    nextStep = nextStep.AddMilliseconds(Constants.MSPerTick);

                    if(nextStep > DateTime.Now)
                    {
                        Thread.Sleep(nextStep - DateTime.Now);
                    }
                }

            }
        }
    }
}
