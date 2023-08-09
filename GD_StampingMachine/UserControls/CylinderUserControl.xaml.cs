﻿using GD_CommonLibrary;
using GD_StampingMachine.UserControls.CustomControls;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GD_StampingMachine.UserControls
{
    /// <summary>
    /// CylinderControlUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class CylinderUserControl : UserControl
    {
        public CylinderUserControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CylinderIsSingalPackIconForegroundProperty =
            DependencyProperty.Register(nameof(CylinderIsSingalPackIconForeground), typeof(SolidColorBrush), typeof(CylinderUserControl), new PropertyMetadata(Brushes.DarkGreen));
        public static readonly DependencyProperty CylinderIsSingalPackIconProperty =
            DependencyProperty.Register(nameof(CylinderIsSingalPackIcon), typeof(PackIconKind), typeof(CylinderUserControl), new PropertyMetadata());

        public static readonly DependencyProperty CylinderIsUpSingal_IsTriggeredProperty =
            DependencyProperty.Register(nameof(CylinderIsUpSingal_IsTriggered), typeof(bool), typeof(CylinderUserControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty CylinderIsMiddleSingal_IsTriggeredProperty =
            DependencyProperty.Register(nameof(CylinderIsMiddleSingal_IsTriggered), typeof(bool), typeof(CylinderUserControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty CylinderIsDownSingal_IsTriggeredProperty =
            DependencyProperty.Register(nameof(CylinderIsDownSingal_IsTriggered), typeof(bool), typeof(CylinderUserControl), new FrameworkPropertyMetadata(true , FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty CylinderUpIsActivedProperty =
            DependencyProperty.Register(nameof(CylinderUpIsActived), typeof(bool), typeof(CylinderUserControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty CylinderMiddleIsActivedProperty =
            DependencyProperty.Register(nameof(CylinderMiddleIsActived), typeof(bool), typeof(CylinderUserControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty CylinderDownIsActivedProperty =
            DependencyProperty.Register(nameof(CylinderDownIsActived), typeof(bool), typeof(CylinderUserControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty CylinderUp_IsCheckedProperty =
    DependencyProperty.Register(nameof(CylinderUp_IsChecked), typeof(bool?), typeof(CylinderUserControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty CylinderMiddle_IsCheckedProperty =
            DependencyProperty.Register(nameof(CylinderMiddle_IsChecked), typeof(bool?), typeof(CylinderUserControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty CylinderDown_IsCheckedProperty =
            DependencyProperty.Register(nameof(CylinderDown_IsChecked), typeof(bool?), typeof(CylinderUserControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty CylinderUp_IsEnabledProperty =
DependencyProperty.Register(nameof(CylinderUp_IsEnabled), typeof(bool), typeof(CylinderUserControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty CylinderMiddle_IsEnabledProperty =
            DependencyProperty.Register(nameof(CylinderMiddle_IsEnabled), typeof(bool), typeof(CylinderUserControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty CylinderDown_IsEnabledProperty =
            DependencyProperty.Register(nameof(CylinderDown_IsEnabled), typeof(bool), typeof(CylinderUserControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));






        public static readonly DependencyProperty CylinderUpCommandProperty =
            DependencyProperty.Register(nameof(CylinderUpCommand), typeof(ICommand), typeof(CylinderUserControl), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty CylinderMiddleCommandProperty =
            DependencyProperty.Register(nameof(CylinderMiddleCommand), typeof(ICommand), typeof(CylinderUserControl), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty CylinderDownCommandProperty =
            DependencyProperty.Register(nameof(CylinderDownCommand), typeof(ICommand), typeof(CylinderUserControl), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty CylinderUpCommandParameterProperty = 
            DependencyProperty.Register(nameof(CylinderUpCommandParameter), typeof(object), typeof(CylinderUserControl), new FrameworkPropertyMetadata((object)null));
        public static readonly DependencyProperty CylinderMiddleCommandParameterProperty = 
            DependencyProperty.Register(nameof(CylinderMiddleCommandParameter), typeof(object), typeof(CylinderUserControl), new FrameworkPropertyMetadata((object)null));
        public static readonly DependencyProperty CylinderDownCommandParameterProperty = 
            DependencyProperty.Register(nameof(CylinderDownCommandParameter), typeof(object), typeof(CylinderUserControl), new FrameworkPropertyMetadata((object)null));


        /// <summary>
        /// 氣壓缸磁簧圖示顏色
        /// </summary>
        public SolidColorBrush CylinderIsSingalPackIconForeground
        {
            get { return (SolidColorBrush)GetValue(CylinderIsSingalPackIconForegroundProperty); }
            set { SetValue(CylinderIsSingalPackIconForegroundProperty, value); }
        }

        /// <summary>
        /// 氣壓缸磁簧圖示
        /// </summary>
        public PackIconKind CylinderIsSingalPackIcon
        {
            get { return (PackIconKind)GetValue(CylinderIsSingalPackIconProperty); }
            set { SetValue(CylinderIsSingalPackIconProperty, value); }
        }
        /// <summary>
        /// 觸發氣壓缸上方磁簧
        /// </summary>
        public bool CylinderIsUpSingal_IsTriggered
        {
            get { return (bool)GetValue(CylinderIsDownSingal_IsTriggeredProperty); }
            set { SetValue(CylinderIsDownSingal_IsTriggeredProperty, value); }
        }
        /// <summary>
        /// 觸發氣壓缸中間磁簧
        /// </summary>
        public bool CylinderIsMiddleSingal_IsTriggered
        {
            get { return (bool)GetValue(CylinderIsMiddleSingal_IsTriggeredProperty); }
            set { SetValue(CylinderIsMiddleSingal_IsTriggeredProperty, value); }
        }
        /// <summary>
        /// 觸發氣壓缸下方磁簧
        /// </summary>
        public bool CylinderIsDownSingal_IsTriggered
        {
            get { return (bool)GetValue(CylinderIsDownSingal_IsTriggeredProperty); }
            set { SetValue(CylinderIsDownSingal_IsTriggeredProperty, value); }
        }

        /// <summary>
        /// 啟用氣壓缸
        /// </summary>
        public bool CylinderUpIsActived
        {
            get { return (bool)GetValue(CylinderUpIsActivedProperty); }
            set { SetValue(CylinderUpIsActivedProperty, value); }
        }
        /// <summary>
        /// 啟用氣壓缸
        /// </summary>
        public bool CylinderMiddleIsActived
        {
            get { return (bool)GetValue(CylinderMiddleIsActivedProperty); }
            set { SetValue(CylinderMiddleIsActivedProperty, value); }
        }
        /// <summary>
        /// 啟用氣壓缸
        /// </summary>
        public bool CylinderDownIsActived
        {
            get { return (bool)GetValue(CylinderDownIsActivedProperty); }
            set { SetValue(CylinderDownIsActivedProperty, value); }
        }

        /// <summary>
        /// 氣壓缸io被觸發
        /// </summary>
        public bool? CylinderUp_IsChecked
        {
            get { return (bool?)GetValue(CylinderUp_IsCheckedProperty); }
            set { SetValue(CylinderUp_IsCheckedProperty, value); }
        }     
        /// <summary>         
        /// 氣壓缸io被觸發
        /// </summary>
        public bool? CylinderMiddle_IsChecked
        {
            get { return (bool?)GetValue(CylinderMiddle_IsCheckedProperty); }
            set { SetValue(CylinderMiddle_IsCheckedProperty, value); }
        }     
        /// <summary>
        /// 啟用氣壓缸io被觸發
        /// </summary>
        public bool? CylinderDown_IsChecked
        {
            get { return (bool?)GetValue(CylinderDown_IsCheckedProperty); }
            set { SetValue(CylinderDown_IsCheckedProperty, value); }
        }


        /// <summary>
        /// 氣壓缸io被觸發
        /// </summary>
        public bool CylinderUp_IsEnabled
        {
            get { return (bool)GetValue(CylinderUp_IsEnabledProperty); }
            set { SetValue(CylinderUp_IsEnabledProperty, value); }
        }
        /// <summary>         
        /// 氣壓缸io被觸發
        /// </summary>
        public bool CylinderMiddle_IsEnabled
        {
            get { return (bool)GetValue(CylinderMiddle_IsEnabledProperty); }
            set { SetValue(CylinderMiddle_IsEnabledProperty, value); }
        }
        /// <summary>
        /// 啟用氣壓缸io被觸發
        /// </summary>
        public bool CylinderDown_IsEnabled
        {
            get { return (bool)GetValue(CylinderDown_IsEnabledProperty); }
            set { SetValue(CylinderDown_IsEnabledProperty, value); }
        }






        /// <summary>
        /// 氣壓缸上
        /// </summary>
        public ICommand CylinderUpCommand
        {
            get { return (ICommand)GetValue(CylinderUpCommandProperty); }
            set { SetValue(CylinderUpCommandProperty, value); }
        }

        /// <summary>
        /// 氣壓缸停
        /// </summary>
        public ICommand CylinderMiddleCommand
        {
            get { return (ICommand)GetValue(CylinderMiddleCommandProperty); }
            set { SetValue(CylinderMiddleCommandProperty, value); }
        }

        /// <summary>
        /// 氣壓缸下
        /// </summary>
        public ICommand CylinderDownCommand
        {
            get { return (ICommand)GetValue(CylinderDownCommandProperty); }
            set { SetValue(CylinderDownCommandProperty, value); }
        }

        public object CylinderUpCommandParameter
        {
            get { return (object)GetValue(CylinderUpCommandParameterProperty); }
            set { SetValue(CylinderUpCommandParameterProperty, value); }
        }
        public object CylinderMiddleCommandParameter
        {
            get { return (object)GetValue(CylinderMiddleCommandParameterProperty); }
            set { SetValue(CylinderMiddleCommandParameterProperty, value); }
        }
        public object CylinderDownCommandParameter
        {
            get { return (object)GetValue(CylinderDownCommandParameterProperty); }
            set { SetValue(CylinderDownCommandParameterProperty, value); }
        }









    }




}