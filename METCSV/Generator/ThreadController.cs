using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace METCSV.Generator
{
    class ThreadController
    {
        List<Thread> threads_toStart = new List<Thread>();
        List<Thread> threads_finished = new List<Thread>();
        List<Thread> threads_inWork = new List<Thread>();

        public void addThread(Thread thread)
        {
            threads_toStart.Add(thread);
        }

        public void startControll()
        {
            while(threads_toStart.Count > 0 || threads_inWork.Count > 0)
            {
                moveThreadsToFinishedListIfNotAlive();
                startNewThreadIfPossible();
                Thread.Sleep(1000);
            }
        }

        public void moveThreadsToFinishedListIfNotAlive()
        {
            for (int i = 0; i < threads_inWork.Count; i++)
            {
                if (threads_inWork[i].IsAlive == false)
                {
                    threads_finished.Add(threads_inWork[i]);
                    Database.Log.Logging.log_message("Thread Finished: " + threads_inWork[i].ManagedThreadId);
                    threads_inWork.RemoveAt(i);
                }
            }
        }

        public void startNewThreadIfPossible()
        {
            if (threads_inWork.Count < Environment.ProcessorCount)
            {
                if (threads_toStart.Count > 0)
                {
                    threads_toStart[0].Start();
                    threads_inWork.Add(threads_toStart[0]);
                    Database.Log.Logging.log_message("Thread started: " + threads_toStart[0]);
                    threads_toStart.RemoveAt(0);
                }
            }
        }
    }
}
