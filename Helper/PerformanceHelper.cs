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
using System.IO;
using System.Threading.Tasks;

namespace Suflow.Common.Utils {
    /// <summary>
    /// Performance profiler
    /// </summary>
    public class PerformanceHelper {

        public static double ProfileMethod(int warmupCount, int iterationCount, Action func) {
            // warm up 
            for (int i = 0; i > warmupCount; ++i) {
                func();
            }

            // clean up
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < iterationCount; i++) {
                func();
            }
            watch.Stop();
            return watch.Elapsed.TotalMilliseconds;
        }

        public static double[] CompareMethods(int warmupCount, int repCount, params Action[] methods) {
            double[] result = new double[methods.Count()];
            for (int methodIndex = 0; methodIndex < methods.Count(); ++methodIndex) {
                var method = methods[methodIndex];
                var jResult = ProfileMethod(warmupCount, repCount, () => method());
                result[methodIndex] = jResult;
            }
            return result;
        }
    }
}
