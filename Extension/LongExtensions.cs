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

namespace System
{
    public static class LongExtensions
    { 
        public static String ConvertToBase(this long num, int nbase)
        {
            String chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            // check if we can convert to another base
            if (nbase < 2 || nbase > chars.Length)
                return "";

            int index;
            String newNumber = "";

            // in r we have the offset of the char that was converted to the new base
            while (num >= nbase)
            {
                index = (int)(num % nbase);
                newNumber = chars[index] + newNumber;
                num = num / nbase;
            }
            // the last number to convert
            index = (int)num;
            newNumber = chars[index] + newNumber;

            return newNumber;
        }
        
        public static string ToStringBase62(this long num)
        {
            return ConvertToBase(num, 62).PadLeft(11, '0');
        } 
    }
}
