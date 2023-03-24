﻿using DevExpress.DataAccess.Json;
using DevExpress.Office.Forms;
using DevExpress.Utils.StructuredStorage.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.WindowsUI;
using GD_StampingMachine.Model;
using GD_StampingMachine.Model.ProductionSetting;
using GD_StampingMachine.Properties;
using GD_StampingMachine.ViewModels.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace GD_StampingMachine.ViewModels.ProductSetting
{
    /// <summary>
    /// GridcontrolVM
    /// </summary>
    public class PartsParameterViewModel : ViewModelBase
    {
        public PartsParameterViewModel()
        {

        }
        public PartsParameterViewModel(PartsParameterModel PParameter)
        {
            PartsParameter = PParameter;
        }

        public PartsParameterModel PartsParameter { get; } = new PartsParameterModel();

        public double FinishProgress
        {
            get => PartsParameter.FinishProgress;
            set 
            { 
                PartsParameter.FinishProgress = value; 
                OnPropertyChanged(nameof(FinishProgress)); 
            }
        }


        public string ParameterA
        {
            get => PartsParameter.Parametert_A;
            set
            {
                PartsParameter.Parametert_A = value;
                OnPropertyChanged(nameof(ParameterA));
            }
        }
        public string ParameterB
        {
            get => PartsParameter.Parametert_B;
            set
            {
                PartsParameter.Parametert_B = value;
                OnPropertyChanged(nameof(ParameterB));
            }
        }
        public string ParameterC
        {
            get => PartsParameter.Parametert_C;
            set
            {
                PartsParameter.Parametert_C = value;
                OnPropertyChanged(nameof(ParameterC));
            }
        }


        /// <summary>
        /// 第一種選單
        /// </summary>
        public NumberSettingModelBase NumberSetting
        {
            get =>PartsParameter.NumberSetting;
            set 
            {
                PartsParameter.NumberSetting = value;
                OnPropertyChanged(nameof(NumberSetting));
            }
        }


        private NumberSettingViewModel _numberSettingVM = new NumberSettingViewModel();
        /// <summary>
        /// 第二種選單
        /// </summary>
        public NumberSettingViewModel NumberSettingVM
        {
            get
            {
                PartsParameter.NumberSetting = _numberSettingVM.NumberSetting;
                return _numberSettingVM;
            }
            set
            {
                _numberSettingVM = value;
                OnPropertyChanged(nameof(NumberSetting));
            }
        }


        private RelayParameterizedCommand _projectEditCommand;
        public RelayParameterizedCommand ProjectEditCommand
        {
            get
            {
                if (_projectEditCommand == null)
                {
                    _projectEditCommand = new RelayParameterizedCommand(obj =>
                    {

                    });
                }
                return _projectEditCommand;
            }
            set
            {
                _projectEditCommand = value;
                OnPropertyChanged(nameof(ProjectEditCommand));
            }
        }

        private RelayParameterizedCommand _projectDeleteCommand;
        public RelayParameterizedCommand ProjectDeleteCommand
        {
            get
            {
                if (_projectDeleteCommand == null)
                {
                    _projectDeleteCommand = new RelayParameterizedCommand(obj =>
                    {
                        if (obj is GridControl ObjGridControl)
                        {
                            if (ObjGridControl.ItemsSource is ObservableCollection<PartsParameterViewModel> GridItemSource)
                            {
                                var MessageBoxReturn = WinUIMessageBox.Show(null,
                                    (string)Application.Current.TryFindResource("Text_AskDelProject") +
                                    "\r\n" +
                                    $"{this.NumberSetting.NumberSettingMode}" +
                                    "?"
                                    ,
                                    (string)Application.Current.TryFindResource("Text_notify"),
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Exclamation,
                                    MessageBoxResult.None,
                                    MessageBoxOptions.None,
                                    DevExpress.Xpf.Core.FloatingMode.Window);

                                if (MessageBoxReturn == MessageBoxResult.Yes)
                                    GridItemSource.Remove(this);
                            }
                        }
                    });
                }
                return _projectDeleteCommand;
            }
          /*  set
            {
                _projectDeleteCommand = value;
                OnPropertyChanged(nameof(ProjectDeleteCommand));
            }*/
        }









    }
}
