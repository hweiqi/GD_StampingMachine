﻿using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using GD_CommonLibrary;
using GD_CommonLibrary.Method;
using GD_StampingMachine.GD_Enum;
using GD_StampingMachine.Model;
using GD_StampingMachine.Model.ParameterSetting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class SeparateSettingViewModel : ParameterSettingBaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_SeparateSettingViewModel");
        public SeparateSettingViewModel(SeparateSettingModel SeparateSetting)
        {
            _separateSetting= SeparateSetting;
            initSeparateSetting();
        }

        private SeparateSettingModel _separateSetting = new();


        public ObservableCollection<SeparateBoxViewModel> SeparateBoxVMObservableCollection
        {
            get
            {
                return _separateSetting.UnifiedSetting_SeparateBoxObservableCollection;
            }
            set
            {
                _separateSetting.UnifiedSetting_SeparateBoxObservableCollection = value;
                OnPropertyChanged();
            }
        }

        public SeparateBoxViewModel SingleSetting_SeparateBoxModel
        {
            get
            {
                return _separateSetting.SingleSetting_SeparateBox;
            }
            set
            {
                _separateSetting.SingleSetting_SeparateBox = value;
                OnPropertyChanged();
            }
        }


        public double SingleSetting_SeparateBoxValue
        {
            get
            {
                if(SettingType == SettingTypeEnum.UnifiedSetting)
                {
                    _separateSetting.UnifiedSetting_SeparateBoxObservableCollection.ForEach(x => x.BoxSliderValue = _separateSetting.SingleSetting_SeparateBox.BoxSliderValue);
                }
                return _separateSetting.SingleSetting_SeparateBox.BoxSliderValue;
            }
            set
            {
                _separateSetting.SingleSetting_SeparateBox.BoxSliderValue = value;
                OnPropertyChanged(nameof(SingleSetting_SeparateBoxValue));
            }
        }

        public SettingTypeEnum SettingType
        {
            get => _separateSetting.SettingType;
            set
            {
                _separateSetting.SettingType = value;
                if (_separateSetting.SettingType == SettingTypeEnum.SingleSetting)
                {
                    _separateSetting.UnifiedSetting_SeparateBoxObservableCollection.ForEach(x =>
                    {
                        x.BoxSliderIsEnabled = true;
                    });
                }
                if (_separateSetting.SettingType == SettingTypeEnum.UnifiedSetting)
                {
                    _separateSetting.UnifiedSetting_SeparateBoxObservableCollection.ForEach(x =>
                    {
                        x.BoxSliderIsEnabled = false;
                    });
                }


                OnPropertyChanged();
            }
        }



        public override ICommand RecoverSettingCommand
        {
            get => new RelayCommand(() =>
            {
                _separateSetting = new SeparateSettingModel();
                initSeparateSetting();
            });
        }

        public override ICommand SaveSettingCommand
        {
            get => new RelayCommand(() =>
            {
                JsonHM.WriteParameterSettingJsonSetting(Method.GD_JsonHelperMethod.ParameterSettingNameEnum.SeparateSetting, _separateSetting, true);       
            });
        }
        public override ICommand LoadSettingCommand
        {
            get => new RelayCommand(() =>
            {
               
                if (JsonHM.ReadParameterSettingJsonSetting(Method.GD_JsonHelperMethod.ParameterSettingNameEnum.SeparateSetting, out SeparateSettingModel SSetting, true))
                {
                    _separateSetting = SSetting;
                    OnPropertyChanged(nameof(SettingType));
                    OnPropertyChanged(nameof(SingleSetting_SeparateBoxValue));
                    OnPropertyChanged(nameof(SeparateBoxVMObservableCollection));
                }
            });
        }

        public override ICommand DeleteSettingCommand => throw new NotImplementedException();

        private void initSeparateSetting()
        {
            SingleSetting_SeparateBoxValue = 0;
            if (_separateSetting.UnifiedSetting_SeparateBoxObservableCollection.Count == 0)
            {
                for (int i = 1; i <= 10; i++)
                {
                    _separateSetting.UnifiedSetting_SeparateBoxObservableCollection.Add(new SeparateBoxViewModel()
                    {
                        BoxIndex = i,
                        BoxSliderValue = 0,
                        BoxIsEnabled = true,
                    }) ;
                }
            }
        }


    }
}
