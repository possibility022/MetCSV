namespace METCSV.Common.ExtensionMethods
{
    public static class TaskExtensions
    {
        public static void StartAll(this Task[] tasks)
        {
            foreach(var t in tasks)
            {
                t.Start();
            }
        }

        public static void WaitAll(this Task[] tasks)
        {
            Task.WaitAll(tasks);
        }
    }
}
