﻿using GD_StampingMachine.GD_Model;
using System;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels.ParameterSetting
{
    public class InputOutputViewModel : ParameterSettingBaseViewModel
    {

        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_InputOutputViewModel");

        public override ICommand RecoverSettingCommand => throw new NotImplementedException();

        public override ICommand SaveSettingCommand => throw new NotImplementedException();

        public override ICommand LoadSettingCommand => throw new NotImplementedException();

        public override ICommand DeleteSettingCommand => throw new NotImplementedException();

        public InputOutputViewModel(InputOutputModel InputOutput)
        {
            _inputOutput = InputOutput;
        }
        private InputOutputModel _inputOutput;





    }
}
