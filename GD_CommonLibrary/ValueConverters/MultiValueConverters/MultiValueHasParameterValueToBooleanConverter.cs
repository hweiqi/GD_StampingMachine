﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using GD_CommonLibrary;

namespace GD_CommonLibrary
{

    public class MultiValueHasParameterValueToBooleanConverter : BaseMultiValueConverter
    {
        public bool Invert { get; set; } = false;
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {   
            if (parameter != null)
            {
                foreach (var item in values)
                {
                    if (Equals(item, parameter))
                        // return true;
                        return !Invert;
                }
                return Invert;
            }
            return Invert;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if (!(bool)value)
                {
                    if(parameter is bool)
                    {
                        var FalseList = new List<object> ();
                        for(int i=0;i<   targetTypes.Count();i++)
                        {
                            FalseList.Add(Invert);
                        }
                        return FalseList.ToArray();
                    }
                }
            }

            throw new NotImplementedException();
        }

       
    }
}
