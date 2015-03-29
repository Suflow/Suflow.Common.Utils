using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Suflow.Common.Utils
{
    /// <summary>
    /// Performance profiler
    /// </summary>
    public class PerformanceHelper
    {
        private static long _depth = 0;
        private Stopwatch _stopWatch;
        private float _startMemory;
        private long _startDatabaseHit;

        public static long TotalDatabaseHitCount = 0;

        public long MemoryIncrease
        {
            get
            {
                var endMemory = (System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64 / 1024f);
                return (long)(endMemory - _startMemory);
            }
        }

        public long Duration
        {
            get
            {
                return _stopWatch.ElapsedMilliseconds;
            }
        }

        public long DatabaseHit
        {
            get
            {
                return TotalDatabaseHitCount - _startDatabaseHit;
            }
        }

        public string WhiteSpace
        {
            get
            {
                var result = "";
                for (var i = 0; i < _depth; ++i)
                    result += " ";
                return result;
            }
        }

        public string Name { get; set; }

        public string Info
        {
            get
            {
                if (Name == null)
                {
                    var sf = new StackTrace(true).GetFrame(1);
                    Name = sf.GetFileName().Substring(sf.GetFileName().LastIndexOf('\\') + 1) + " line: " + sf.GetFileLineNumber();
                }
                return string.Format("| {0} ms | {1} kb | {2} db hit | {3} |", Duration.ToString().PadLeft(5, ' '), MemoryIncrease.ToString().PadLeft(5, ' '), DatabaseHit, Name);
            }
        }

        public void StartNew(string message)
        {
            Name = message;

            ++_depth;
            _startMemory = (System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64 / 1024f);
            _startDatabaseHit = TotalDatabaseHitCount;
            _stopWatch = Stopwatch.StartNew();
        }

        public void Stop()
        {
            _stopWatch.Stop();
            --_depth;
        }

        public static void CompareMethods(params Func<object>[] methods)
        {
            var result = CompareMethods(3, 3, 1000, "\n\t", methods);
            var textWriter = Console.Out;
            textWriter.Write(result);
        }

        public static string CompareMethods(int warmupCount, int setCount, int repCount, string lineBreaker, params Func<object>[] methods)
        {
            var log = new StringBuilder();
            foreach (var method in methods)
            {
                for (int i = 0; i > warmupCount; ++i)
                {
                    method.Invoke();
                }
            }
            for (int j = 0; j < setCount; ++j)
            {
                log.AppendLine("Set" + j + ":");
                foreach (var method in methods)
                {
                    var stopWatch = Stopwatch.StartNew();
                    for (int i = 0; i < repCount; ++i)
                    {
                        method.Invoke();
                    }
                    stopWatch.Stop();
                    log.AppendLine(method.Method.Name + " took: " + stopWatch.ElapsedTicks + " ticks.");
                }
                log.AppendLine("");
            }
            return log.ToString();
        }
    }
}
