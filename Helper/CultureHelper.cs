using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;

namespace Suflow.Common.Utils
{
    public class CultureHelper : IDisposable
    {
        private readonly CultureInfo _currentCulture;
        private readonly CultureInfo _currentUICulture;

        public static CultureHelper InvariantCulture
        {
            get { return new CultureHelper(CultureInfo.InvariantCulture, CultureInfo.InvariantCulture); }
        }

        /// <summary></summary>
        /// <param name="culture">Which culture-dependent functions (date, number, and currency) need to use</param>
        /// <param name="uiculture">What resource file application needs to use. </param>
        public CultureHelper(string culture, string uiculture)
            : this(new CultureInfo(culture), new CultureInfo(uiculture))
        {
        }

        public CultureHelper(CultureInfo cultureInfo, CultureInfo uiCultureInfo)
        {
            _currentCulture = Thread.CurrentThread.CurrentCulture;
            _currentUICulture = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = uiCultureInfo;
        }

        public void Dispose()
        {
            Thread.CurrentThread.CurrentCulture = _currentCulture;
            Thread.CurrentThread.CurrentUICulture = _currentUICulture;
        }
    }
}
