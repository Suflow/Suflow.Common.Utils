using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Suflow.Common.Utils
{
    public abstract class BackgroundTask : BackgroundTask<object>
    {
        public static bool StopAllTasks { get; set; }
        internal static string StopReason { get; set; }

        public static void Stop(string stopReason)
        {
            BackgroundTask.StopReason = stopReason;
            BackgroundTask.StopAllTasks = true;
        }
    } 

    public abstract class BackgroundTask<T>
    {
        private static object _taskLock = new Object(); 

        protected List<T> TaskContextItems { get; set; }
        protected Stopwatch TaskStopwatch { get; private set; }

        /// <summary>
        /// AllItems should be actualized here
        /// It is the place to validate and call set BackgroundTask.StopAllTasks to true if you want to stop execution
        /// </summary>
        protected virtual void BeforeExecute()
        {
        }

        protected virtual void Execute(T item)
        {
        }

        protected virtual void AfterExecute()
        {
        }

        protected virtual void ExecutionCanceled(string canceledReason)
        {
        }

        private void Start(object threadContext)
        {
            lock (_taskLock)
            {
                try
                {
                    TaskStopwatch = Stopwatch.StartNew();

                    //=================================
                    //BeforeExecute
                    //=================================
                    if (BackgroundTask.StopAllTasks)
                    {
                        ExecutionCanceled(BackgroundTask.StopReason);
                        return;
                    }
                    BeforeExecute();

                    //=================================
                    //Execute
                    //=================================
                    foreach (var item in TaskContextItems)
                    {
                        if (BackgroundTask.StopAllTasks)
                        {
                            ExecutionCanceled(BackgroundTask.StopReason);
                            return;
                        }
                        Execute(item);
                    }

                    //=================================
                    //AfterExecute
                    //=================================
                    if (BackgroundTask.StopAllTasks)
                    {
                        ExecutionCanceled(BackgroundTask.StopReason);
                        return;
                    }
                    AfterExecute();
                }
                catch (Exception e)
                {
                    //LOGGING
                }
            }
        }

        public bool Start()
        {
            try
            {
                ThreadPool.QueueUserWorkItem(Start);
                return true;
            }
            catch (Exception e)
            {
                //LOGGING
                return false;
            }
        }
         
        public BackgroundTask()
        {
            TaskContextItems = new List<T>();
            BackgroundTask.StopAllTasks = false;
            BackgroundTask.StopReason = String.Empty;
        }
    }
}
