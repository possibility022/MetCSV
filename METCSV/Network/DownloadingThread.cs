using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace METCSV.Network
{
    class DownloadingThread
    {
        public delegate void DownloadDone();
        public DownloadDone done;
        protected string fileName;
        protected Thread thread;

        public string getFileName()
        {
            return fileName;
        }

        public bool threadAlive()
        {
            if (thread != null)
                return thread.IsAlive;
            else
                return false;
        }
    }
}
