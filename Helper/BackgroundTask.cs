////////////////////////////////////////////////////////////////////////////////
//
//    Suflow, Enterprise Applications
//    Copyright (C) 2015 Suflow
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation, either version 3 of the
//    License, or (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
////////////////////////////////////////////////////////////////////////////////
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
                catch (Exception)
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
            catch (Exception)
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
