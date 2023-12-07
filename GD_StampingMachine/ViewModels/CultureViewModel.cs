﻿using GD_StampingMachine.Singletons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GD_StampingMachine.ViewModels
{
    public class CultureViewModel : GD_CommonLibrary.BaseViewModel
    {
         public override string ViewModelName => (string)System.Windows.Application.Current.TryFindResource("Name_LanguageSettingViewModel");

        public List<CultureInfo> SupportedCultures
        {
            get
            {
                return CulturesHelper.SupportedCultures;
            }
        }



        private CultureInfo _selectedCulture = Properties.Settings.Default.DefaultCulture;

        public CultureInfo SelectedCulture
        {
            get
            {
                try
                {
                    if (_selectedCulture == CultureInfo.InvariantCulture)
                    {
                        var CurrentCulture = Thread.CurrentThread.CurrentCulture;
                        if (CulturesHelper.SupportedCultures.Contains(CurrentCulture))
                            _selectedCulture = CurrentCulture;
                    }

                    if (_selectedCulture != null)
                    {
                        CulturesHelper.ChangeCulture(_selectedCulture);
                    }
                }
                catch(Exception ex)
                {
                    _ = LogDataSingleton.Instance.AddLogDataAsync(this.ViewModelName, ex.Message);
                }
                return _selectedCulture;
            }
            set
            {
                _selectedCulture = value;
                OnPropertyChanged();
            }
        }
    }
}
