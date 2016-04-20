using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Suflow.Common.Utils {
    public class BackgroundWorkerHelper {
        /// <summary>
        /// Creates a background worker that executes code in separate thread
        /// </summary>
        public static BackgroundWorker CreateBackgroundWorker(Action<BackgroundWorker> actualAction, Action<ProgressChangedEventArgs> progressChangedAction, Action<RunWorkerCompletedEventArgs> completedAction) {
            var worker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            worker.DoWork += delegate {
                try {
                    actualAction(worker);
                }
                catch (Exception e) {
                    worker.ReportProgress(100, "ERROR" + e.Message);
                }
            };
            if (progressChangedAction != null)
                worker.ProgressChanged += (s, e) => progressChangedAction(e);
            if (completedAction != null)
                worker.RunWorkerCompleted += (s, e) => completedAction(e);
            return worker;
        }

    }
}
