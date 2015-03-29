using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Suflow.Common.Utils
{
    /// <summary>
    /// Provides a convenience methodology for implementing locked access to resources. 
    /// </summary>
    /// <remarks>
    /// Intended as an infrastructure class.
    /// </remarks>
    public class WriteLock : IDisposable
    {
        private readonly ReaderWriterLockSlim _rwLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteLockDisposable"/> class.
        /// </summary>
        /// <param name="rwLock">The rw lock.</param>
        public WriteLock(ReaderWriterLockSlim rwLock)
        {
            _rwLock = rwLock;
            _rwLock.EnterWriteLock();
        }

          void IDisposable.Dispose()
        {
            //this.Dispose(true);
            _rwLock.ExitWriteLock();
        }
    }
}
