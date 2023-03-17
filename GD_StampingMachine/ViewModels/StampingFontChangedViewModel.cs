﻿using DevExpress.CodeParser;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm.Native;
using GD_StampingMachine.Method;
using GD_StampingMachine.Model;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GD_StampingMachine.ViewModels
{
    public class StampingFontChangedViewModel : ViewModelBase
    {

        private ObservableCollection<StampingTypeModel> _stampingTypeVMObservableCollection;
        public ObservableCollection<StampingTypeModel> StampingTypeVMObservableCollection
        {
            get
            {
                if (_stampingTypeVMObservableCollection == null)
                    _stampingTypeVMObservableCollection = new ObservableCollection<StampingTypeModel>();
                return _stampingTypeVMObservableCollection;
            }
            set
            {
                _stampingTypeVMObservableCollection = value;
                OnPropertyChanged(nameof(StampingTypeVMObservableCollection));
            }
        }


        private ObservableCollection<StampingTypeModel> _unusedStampingTypeVMObservableCollection;
        public ObservableCollection<StampingTypeModel> UnusedStampingTypeVMObservableCollection
        {
            get
            {
                if (_unusedStampingTypeVMObservableCollection == null)
                    _unusedStampingTypeVMObservableCollection = new ObservableCollection<StampingTypeModel>();
                return _unusedStampingTypeVMObservableCollection;
            }
            set
            {
                _unusedStampingTypeVMObservableCollection = value;
                OnPropertyChanged(nameof(UnusedStampingTypeVMObservableCollection));
            }
        }
        /// <summary>
        /// 鋼印機上的字模
        /// </summary>
        public StampingTypeModel StampingFontSelected { get; set; }
        /// <summary>
        /// 被新建出來還沒放上去的字模/被換下來的字模
        /// </summary>
        public StampingTypeModel UnusedStampingFontSelected { get; set; }

        public RelayCommand StampingFontReplaceCommand
        {
            get => new RelayCommand(() =>
            {
                if (StampingFontSelected != null && UnusedStampingFontSelected != null)
                {
                    StampingTypeModelExchanged();
                }
            });
        }

        private void StampingTypeModelExchanged()
        {
            var FontString = StampingFontSelected.StampingTypeString;
            var FontStringNumber = StampingFontSelected.StampingTypeNumber;
            var FontStringUseCount = StampingFontSelected.StampingTypeUseCount;

            var UnusedFontString = UnusedStampingFontSelected.StampingTypeString;
            var UnusedFontStringNumber = UnusedStampingFontSelected.StampingTypeNumber;
            var UnusedFontStringUseCount = UnusedStampingFontSelected.StampingTypeUseCount;

            var ST_index = StampingTypeVMObservableCollection.FindIndex(x => x == StampingFontSelected);
            var UST_index = UnusedStampingTypeVMObservableCollection.FindIndex(x => x == UnusedStampingFontSelected);

            StampingTypeVMObservableCollection[ST_index] = new StampingTypeModel()
            {
                StampingTypeNumber = FontStringNumber,
                StampingTypeString = UnusedFontString,
                StampingTypeUseCount = UnusedFontStringUseCount
            };

            UnusedStampingTypeVMObservableCollection[UST_index] = new StampingTypeModel()
            {
                StampingTypeNumber = UnusedFontStringNumber,
                StampingTypeString = FontString,
                StampingTypeUseCount = FontStringUseCount
            };
        }

        private ObservableCollection<StampingTypeModel> _newUnusedStampingFont;
        /// <summary>
        /// 新增字模
        /// </summary>
        public ObservableCollection<StampingTypeModel> NewUnusedStampingFont
        {
            get
            {
                if (_newUnusedStampingFont == null)
                {
                    _newUnusedStampingFont = new ObservableCollection<StampingTypeModel>();
                }

                if (_newUnusedStampingFont.Count == 0)
                {
                    _newUnusedStampingFont.Add(new StampingTypeModel()
                    {
                        StampingTypeNumber = 0,
                        StampingTypeUseCount = 0,
                        StampingTypeString = null,
                        IsNewAddStamping = true,
                    });
                };

                return _newUnusedStampingFont;
            }
            set
            {
                _newUnusedStampingFont = value;
                OnPropertyChanged(nameof(NewUnusedStampingFont));
            }
        }


        public ICommand UnusedStampingFontAddCommand
        {
            get => new RelayCommand(() =>
            {
                var FirstFont = NewUnusedStampingFont.FirstOrDefault().Clone() as StampingTypeModel;
                FirstFont.IsNewAddStamping = false;
                UnusedStampingTypeVMObservableCollection.Add(FirstFont);
                /*
                if (!string.IsNullOrEmpty(NewUnnsedStampingFontString))
                {
                    UnusedStampingTypeVMObservableCollection.Add(new StampingTypeModel()
                    {
                        StampingTypeNumber = 0,
                        StampingTypeString = NewUnnsedStampingFontString,
                        StampingTypeUseCount = 0
                    });
                }*/
            });
        }

        public RelayCommand UnusedStampingFontDelCommand
        {
            get => new RelayCommand(() =>
            {
                if (UnusedStampingFontSelected != null)
                {
                    //下列配合DragDrop會導致介面異常 已停用
                    UnusedStampingTypeVMObservableCollection.Remove(UnusedStampingFontSelected);




                }

            });
        }



        private double _stampingFontTurntable_RorateAngle;
        public double StampingFontTurntable_RorateAngle
        {
            get => _stampingFontTurntable_RorateAngle;
            set
            {
                _stampingFontTurntable_RorateAngle = value;
                OnPropertyChanged(nameof(StampingFontTurntable_RorateAngle));
            }
        }

        private StampingTypeModelMartixClass _stampingTypeModelMartix= new StampingTypeModelMartixClass();

        public StampingTypeModelMartixClass StampingTypeModelMartix
        {
            get => _stampingTypeModelMartix; 
            set
            {
                _stampingTypeModelMartix = value;
                OnPropertyChanged(nameof(StampingTypeModelMartix));
            }
        }


        private readonly object balanceLock = new object();
        private bool StampingTypeModel_ReadyStamping_IsRotating = false;
        private StampingTypeModel _stampingTypeModel_readyStamping;
        public StampingTypeModel StampingTypeModel_ReadyStamping
        {
            get
            {
                if (_stampingTypeModel_readyStamping == null)
                {
                    if (_stampingTypeVMObservableCollection.Count != 0)
                    {
                        _stampingTypeModel_readyStamping = _stampingTypeVMObservableCollection.FirstOrDefault();
                    }
                }
                var FIndex = StampingTypeVMObservableCollection.ToList().FindIndex(x => x.Equals(_stampingTypeModel_readyStamping));
                if (FIndex != -1)
                {
                    StampingTypeModelMartix = new StampingTypeModelMartixClass();


                    //離原點差距的角度-以逆時針計算
                    double TargetAngle = -360 * FIndex / StampingTypeVMObservableCollection.Count;

                    var AngleGap = Math.Abs(TargetAngle - StampingFontTurntable_RorateAngle);
                    if (TargetAngle > StampingFontTurntable_RorateAngle)
                    {
                        AngleGap += 360;
                    }
                    int SleepTime = 0;
                    if (AngleGap != 0)
                    {
                        SleepTime = Convert.ToInt32(360 / AngleGap);
                        if (SleepTime >= 5)
                            SleepTime = 5;
                    }

                    //延遲旋轉
                    Task.Run(() =>
                    {


                        if (StampingTypeModel_ReadyStamping_IsRotating)
                        {
                            StampingTypeModel_ReadyStamping_IsRotating = false;
                        }
                        try
                        {


                            StampingTypeModelMartix.BottomStampingTypeModel = StampingTypeVMObservableCollection[FIndex];
                            double RotateGap = StampingTypeVMObservableCollection.Count / 4;



                            var LeftIndex = FIndex + RotateGap * 1;
                            while (LeftIndex >= StampingTypeVMObservableCollection.Count)
                            {
                                LeftIndex -= StampingTypeVMObservableCollection.Count;
                            }
                            StampingTypeModelMartix.LeftStampingTypeModel = StampingTypeVMObservableCollection[(int)LeftIndex];

                            var TopIndex = FIndex + RotateGap * 2;
                            while (TopIndex >= StampingTypeVMObservableCollection.Count)
                            {
                                TopIndex -= StampingTypeVMObservableCollection.Count;
                            }
                            StampingTypeModelMartix.TopStampingTypeModel = StampingTypeVMObservableCollection[(int)TopIndex];

                            var RightIndex = FIndex + RotateGap * 3;
                            while (RightIndex >= StampingTypeVMObservableCollection.Count)
                            {
                                RightIndex -= StampingTypeVMObservableCollection.Count;
                            }
                            StampingTypeModelMartix.RightStampingTypeModel = StampingTypeVMObservableCollection[(int)RightIndex];


                        }
                        catch (Exception ex)
                        {

                        }

                        //先取得目前角度->方向逆時針
                        lock (balanceLock)
                        {




                            var CurrentAngle = StampingFontTurntable_RorateAngle;
                            StampingTypeModel_ReadyStamping_IsRotating = true;
                            while (StampingTypeModel_ReadyStamping_IsRotating)
                            {
                                if (StampingFontTurntable_RorateAngle < -360)
                                {
                                    StampingFontTurntable_RorateAngle += 360;
                                }
                                if (StampingFontTurntable_RorateAngle > 360)
                                {
                                    StampingFontTurntable_RorateAngle -= 360;
                                }
                                //以0.1度進行逆時針旋轉  超過360度需要處理?

                                if (Math.Abs(TargetAngle - StampingFontTurntable_RorateAngle) < 1.2)
                                {
                                    break;
                                }

                                StampingFontTurntable_RorateAngle -= 0.5;
                                System.Threading.Thread.Sleep(SleepTime);
                            }
                            StampingFontTurntable_RorateAngle = TargetAngle;
                            StampingTypeModel_ReadyStamping_IsRotating = false;


                            //找出上下左右四個格子的方塊
                            //下方



                        }

                    });
                }

                return _stampingTypeModel_readyStamping;
            }
            set
            {
                _stampingTypeModel_readyStamping = value;
                OnPropertyChanged(nameof(StampingTypeModel_ReadyStamping));
            }
        }



        /*動起來很好看 所以保留在這裡*/
        /*private bool StampingTypeModel_ReadyStamping_IsRotating = false;
        private StampingTypeModel _stampingTypeModel_readyStamping;
        public StampingTypeModel StampingTypeModel_ReadyStamping
        {
            get => _stampingTypeModel_readyStamping;
            set
            {
                _stampingTypeModel_readyStamping = value;
                OnPropertyChanged(nameof(StampingTypeModel_ReadyStamping));
                var FIndex = StampingTypeVMObservableCollection.ToList().FindIndex(x => x.Equals(_stampingTypeModel_readyStamping));
                if (FIndex != -1)
                {
                    //離原點差距的角度-以逆時針計算
                    double TargetAngle = -360 * FIndex / StampingTypeVMObservableCollection.Count;

                    var AngleGap = Math.Abs(TargetAngle - StampingFontTurntable_RorateAngle);
                    if (TargetAngle > StampingFontTurntable_RorateAngle)
                    {
                        AngleGap += 360;
                    }
                    int SleepTime = 0;
                    if (AngleGap != 0)
                    {
                        SleepTime = Convert.ToInt32(360 / AngleGap);
                        if (SleepTime >= 5)
                            SleepTime = 5;
                    }

                    //延遲旋轉
                    Task.Run(() =>
                    {
                        if (StampingTypeModel_ReadyStamping_IsRotating)
                        {
                            StampingTypeModel_ReadyStamping_IsRotating = false;
                        }

                        //先取得目前角度->方向逆時針
                        lock (balanceLock)
                        {
                            var CurrentAngle = StampingFontTurntable_RorateAngle;
                            StampingTypeModel_ReadyStamping_IsRotating = true;
                            while (StampingTypeModel_ReadyStamping_IsRotating)
                            {
                                if (StampingFontTurntable_RorateAngle < -360)
                                {
                                    StampingFontTurntable_RorateAngle += 360;
                                }
                                if (StampingFontTurntable_RorateAngle > 360)
                                {
                                    StampingFontTurntable_RorateAngle -= 360;
                                }
                                //以0.1度進行逆時針旋轉  超過360度需要處理?

                                if (Math.Abs(TargetAngle - StampingFontTurntable_RorateAngle) < 1.2)
                                {
                                    break;
                                }
                                StampingFontTurntable_RorateAngle -= 0.5;
                                System.Threading.Thread.Sleep(SleepTime);
                            }
                            StampingFontTurntable_RorateAngle = TargetAngle;
                            StampingTypeModel_ReadyStamping_IsRotating = false;
                        }

                    });
                }
            }
        }*/






        /// <summary>
        /// DropDarg
        /// </summary>
        public StampingTypeDropTarget DragStampingTypeDropTarget { get; set; } = new StampingTypeDropTarget();

    }

    public class StampingTypeModelMartixClass : ViewModelBase
    {
        private StampingTypeModel _bottomStampingTypeModel;
        private StampingTypeModel _rightStampingTypeModel;
        private StampingTypeModel _topStampingTypeModel;
        private StampingTypeModel _leftStampingTypeModel;

        public StampingTypeModel BottomStampingTypeModel
        {
            get=> _bottomStampingTypeModel;
            set
            {
                _bottomStampingTypeModel = value;
                OnPropertyChanged(nameof(BottomStampingTypeModel));
            }
        }
        public StampingTypeModel RightStampingTypeModel
        {
            get => _rightStampingTypeModel;
            set
            {
                _rightStampingTypeModel = value;
                OnPropertyChanged(nameof(RightStampingTypeModel));
            }
        }
        public StampingTypeModel TopStampingTypeModel
        {
            get => _topStampingTypeModel;
            set
            {
                _topStampingTypeModel = value;
                OnPropertyChanged(nameof(TopStampingTypeModel));
            }
        }
        public StampingTypeModel LeftStampingTypeModel
        {
            get => _leftStampingTypeModel; set
            {
                _leftStampingTypeModel = value;
                OnPropertyChanged(nameof(LeftStampingTypeModel));
            }
        }
    }



}
