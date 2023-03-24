﻿using DevExpress.XtraPrinting.Native.WebClientUIControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.Extensions
{
    public static class CommonExtensions
    {


        /// <summary>
        /// 深層複製
        /// </summary>
        /// <typeparam name="T">複製對象類別</typeparam>
        /// <param name="source">複製對象
        /// <returns>複製出的物件</returns>
        public static T DeepCloneByJson<T>(this T source)
        {
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }
    }
}