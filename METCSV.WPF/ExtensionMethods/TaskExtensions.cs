using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace METCSV.WPF.ExtensionMethods
{
    static class TaskExtensions
    {
        public static void StartAll(this Task[] tasks)
        {
            foreach(var t in tasks)
            {
                t.Start();
            }
        }
    }
}
