using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.Network
{
    class DownloadingThread
    {
        public delegate void DownloadDone();
        public DownloadDone done;
        protected string fileName;
        protected Task task;

        public string getFileName()
        {
            return fileName;
        }

        public bool TaskIsCompleted()
        {
            if (task == null)
                return true;
            else
                return task.IsCompleted;
        }
    }
}
