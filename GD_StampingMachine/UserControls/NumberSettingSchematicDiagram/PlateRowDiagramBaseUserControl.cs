﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GD_StampingMachine.UserControls.NumberSettingSchematicDiagram
{
    public class PlateRowDiagramBaseUserControl :UserControl
    {

        public PlateRowDiagramBaseUserControl()
        {
            this.Resources.Add(nameof(RedMeasurementLineVisibility), Visibility.Visible);
            this.Resources.Add(nameof(PlateRowBackground), (SolidColorBrush)new BrushConverter().ConvertFrom("#d6d5ce"));
        }

        public Visibility RedMeasurementLineVisibility
        {
            get => (Visibility)GetValue(RedMeasurementLineVisibilityProperty);
            set => SetValue(RedMeasurementLineVisibilityProperty, value);
        }

        public Brush PlateRowBackground
        {
                get => (Brush)GetValue(PlateRowBackgroundProperty);
                set => SetValue(PlateRowBackgroundProperty, value);
        }

        public static readonly DependencyProperty RedMeasurementLineVisibilityProperty = DependencyProperty.Register(
            nameof(RedMeasurementLineVisibility),
            typeof(Visibility),
            typeof(PlateRowDiagramBaseUserControl),    
            new PropertyMetadata(Visibility.Visible, RedMeasurementLineVisibilityPropertyChanged));



        public static readonly DependencyProperty PlateRowBackgroundProperty = DependencyProperty.Register(
            nameof(PlateRowBackground),
            typeof(Brush),
            typeof(PlateRowDiagramBaseUserControl),
            new PropertyMetadata((SolidColorBrush)new BrushConverter().ConvertFrom("#d6d5ce") , PlateRowBackgroundPropertyChanged));






        private static void RedMeasurementLineVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var UserControl = (d as PlateRowDiagramBaseUserControl);
            UserControl.Resources[nameof(RedMeasurementLineVisibility)] = e.NewValue;
        }
        private static void PlateRowBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var UserControl = (d as PlateRowDiagramBaseUserControl);

            if (e.NewValue is Brush)
                UserControl.Resources[nameof(PlateRowBackground)] = e.NewValue;

            if (e.NewValue is string NewValueString)
            {
                if (NewValueString.Contains("#"))
                {
                    UserControl.Resources[nameof(PlateRowBackground)] = (SolidColorBrush)new BrushConverter().ConvertFrom(NewValueString);
                }
            }
        }
        





    }
}
