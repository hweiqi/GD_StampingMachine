﻿using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Diagnostics;
using DevExpress.XtraRichEdit.Model;

namespace GD_StampingMachine.Method
{
    public class CsvHelperMethod
    {
        /// <summary>
        /// 複寫檔案
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="CSVData"></param>
        /// <returns></returns>
        public bool WriteCSVFile<T>(string fileName, object CSVData)
        {
            var CSVDataIEnumerable = new List<object>() { CSVData };
            return WriteCSVFileIEnumerable(fileName, CSVDataIEnumerable);
        }
        /// <summary>
        /// 複寫檔案
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="CSVData"></param>
        /// <returns></returns>
        public bool WriteCSVFileIEnumerable<T>(string fileName , List<T> CSVDataIEnumerable)
        {
            try
            {
                var writeConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true
                };
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                using (var writer = new StreamWriter(fileName, false))
                using (var csv = new CsvWriter(writer, writeConfiguration))
                {
                    csv.WriteRecords(CSVDataIEnumerable);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
                return false;
            }
        }



        /// <summary>
        /// 讀取檔案
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="CSVData"></param>
        /// <returns></returns>
        public bool ReadCSVFile<T>(string fileName, out T CSVData)
        {
            CSVData = default(T);
           if(ReadCSVFileIEnumerable(fileName, out List<T> CSVDataIEnumerable))
           {
                if(CSVDataIEnumerable.Count()!=0)
                {
                    CSVData = CSVDataIEnumerable.FirstOrDefault();
                    return true;
                }
           }
            return false;

        }

        /// <summary>
        /// 讀取檔案
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="CSVData"></param>
        /// <returns></returns>
        public bool ReadCSVFileIEnumerable<T>(string fileName , out List<T> CSVDataIEnumerable)
        {
            CSVDataIEnumerable = new List<T>();

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                return false;


            try
            {
                var readConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true
                };

                using (var reader = new StreamReader(fileName))
                using (var csv = new CsvReader(reader, readConfiguration))
                {
                    CSVDataIEnumerable = csv.GetRecords<T>().ToList();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
                return false;
            }
        }







    }
}
