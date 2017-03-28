using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.Generator
{
    class TaskController
    {
        List<Task> tasks_toStart = new List<Task>();
        List<Task> tasks_finished = new List<Task>();
        List<Task> tasks_inWork = new List<Task>();

        public void addThread(Task thread)
        {
            tasks_toStart.Add(thread);
        }

        public void startControll()
        {
            while(tasks_toStart.Count > 0 || tasks_inWork.Count > 0)
            {
                moveThreadsToFinishedListIfNotAlive();
                startNewThreadIfPossible();
                System.Threading.Thread.Sleep(1000);
            }
        }

        public void moveThreadsToFinishedListIfNotAlive()
        {
            for (int i = 0; i < tasks_inWork.Count; i++)
            {
                if (tasks_inWork[i].IsCanceled)
                {
                    tasks_finished.Add(tasks_inWork[i]);
                    Database.Log.Logging.log_message("Thread Finished: " + tasks_inWork[i].Id);
                    tasks_inWork.RemoveAt(i);
                }
            }
        }

        public void startNewThreadIfPossible()
        {
            if (tasks_inWork.Count < Environment.ProcessorCount)
            {
                if (tasks_toStart.Count > 0)
                {
                    tasks_toStart[0].Start();
                    tasks_inWork.Add(tasks_toStart[0]);
                    Database.Log.Logging.log_message("Thread started: " + tasks_toStart[0].Id);
                    tasks_toStart.RemoveAt(0);
                }
            }
        }
    }
}
