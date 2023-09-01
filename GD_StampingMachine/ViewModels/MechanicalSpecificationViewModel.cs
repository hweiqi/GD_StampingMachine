﻿using GD_CommonLibrary;
using GD_StampingMachine.GD_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels
{
    public class MachanicalSpecificationViewModel : GD_CommonLibrary.BaseViewModel
    {
        public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_MachanicalSpecificationViewModel");
        public MachanicalSpecificationViewModel(MachanicalSpecificationModel _machanicalSpecification)
        {
            this.MachanicalSpecification = _machanicalSpecification;
        }


        public MachanicalSpecificationModel MachanicalSpecification { get; set; } = new ();

    }
}
