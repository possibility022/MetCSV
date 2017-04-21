﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.Network
{
    class DownloadingThread
    {
        public enum DownloadingResult
        {
            readyToStart,
            complete,
            inProgress,
            faild
        }

        public delegate void DownloadDone();
        public DownloadDone done;
        protected string fileName;
        protected Task task;
        private object _lock = new object();

        DownloadingResult downloadingResult = DownloadingResult.readyToStart;

        public DownloadingResult GetDownloadingResult()
        {
            lock(_lock)
            {
                return downloadingResult;
            }
        }

        protected void SetDownloadingResult(DownloadingResult value)
        {
            lock(_lock)
            {
                downloadingResult = value;
            }
        }

        public string GetFileName()
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
