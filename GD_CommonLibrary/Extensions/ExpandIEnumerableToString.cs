﻿using DevExpress.Mvvm.Native;
using DevExpress.Office;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_CommonLibrary.Extensions
{
    public static partial class CommonExtensions
    {
        public static string ExpandToString<T>(this IEnumerable<T> IEnumerableValue)
        {
           return IEnumerableValue.ExpandToString(',');
        }

        public static string ExpandToString<T>(this IEnumerable<T> IEnumerableValue , char splitCharacter)
        {
            var RString = "";
            IEnumerableValue.ForEach(obj =>
            {
                RString += obj.ToString() + splitCharacter;
            });

            RString = RString.TrimEnd(splitCharacter);
            return RString;
        }

    }
}