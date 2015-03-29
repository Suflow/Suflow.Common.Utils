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
