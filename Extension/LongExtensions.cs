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
