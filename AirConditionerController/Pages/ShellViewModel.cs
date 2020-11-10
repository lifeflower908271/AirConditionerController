using System;
using System.Windows;
using Stylet;
using StyletIoC;
using NLECloudSDK;
using Microsoft.Research.DynamicDataDisplay;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Microsoft.Research.DynamicDataDisplay.Charts;

using System.Windows.Controls;
using Utilities;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using System.Windows.Media.Animation;
using NLECloudSDK.Model;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Windows.Media.Effects;
using System.Windows.Controls.Primitives;

namespace AirConditionerController.Pages
{
    public class ShellViewModel : Screen
    {
        private readonly IWindowManager _windowManager;
        private readonly IContainer _container;
        public ShellView TransView => (ShellView)View; // 与当前vm关联的视图

        private NLECloudAPI nle_api; // api接口
        private String m_token; // 接口访问令牌
        private object _mutex = new object(); // 互斥锁的同步块索引对象
        private Thread threadWork;
        private bool isRunning = false;


        #region 数据源
        public int TempCtrlStatusIdx { get; set; } // 温控状态索引
        public bool IsOnline { get; set; } = false; // 设备在线状态
        public bool IsOpen { get; set; } = false; // 空调开关状态
        public string UpperLimit { get; set; } // 上限值温度
        public string LowerLimit { get; set; } // 下限值温度
        public string CurrentTemp { get; set; } // 当前室温
        public string Alarm { get; set; } // 温度报警
        public string UpperLimitCtrl { get; set; } // 上限值温度
        public string LowerLimitCtrl { get; set; } // 下限值温度
        #endregion

        public ShellViewModel(IWindowManager windowManager, IContainer container)
        {
            _windowManager = windowManager;
            _container = container;

            nle_api = Store.Get<NLECloudAPI>(UDG_.NLE_API);

        }

        protected override void OnViewLoaded()
        {
            // 登录排队机系统
            this.View.Visibility = Visibility.Hidden;
            var vmLogin = _container.Get<LoginViewModel>();
            var rst = _windowManager.ShowDialog(vmLogin);
            if (rst == false)
            {
                RequestClose();
                return;
            }
            this.View.Visibility = Visibility.Visible;

            m_token = Store.Get<String>(UDG_.ACCESS_TOKEN);

            InitThread();

        }


        public async void CmdPower(object sender, RoutedEventArgs e)
        {
            var ui = (ToggleButton)sender;
            int val; if (ui.IsChecked ?? false) val = 1; else val = 0;
            var isSuccess = await Task<bool>.Factory.StartNew(() =>
            {
                lock (_mutex)
                {
                    var resp = nle_api.Cmds(UDG_.DEVICE_ID, UDG_.APITAG_POWER_CTRL, val, m_token);
                    _waitTime = 0;
                    return resp.IsSuccess();
                }
            });
        }

        public async void CmdCommit()
        {
            if(string.IsNullOrEmpty(UpperLimitCtrl) || string.IsNullOrEmpty(UpperLimitCtrl))
            {
                _windowManager.ShowMessageBox("请输入温控阈值","提示");
                return;
            }
            int upperLimitCtrl = -1, lowerLimitCtrl = 1;
            if(!int.TryParse(UpperLimitCtrl, out upperLimitCtrl) || !int.TryParse(LowerLimitCtrl, out lowerLimitCtrl))
            {
                _windowManager.ShowMessageBox("请输入正确的温度值", "提示");
                return;
            }
            if(upperLimitCtrl < lowerLimitCtrl)
            {
                _windowManager.ShowMessageBox("请输入正确的温度阈值", "提示");
                return;
            }
            var isSuccess = await Task<bool>.Factory.StartNew(() =>
            {
                lock (_mutex)
                {
                    var resp = nle_api.Cmds(UDG_.DEVICE_ID, UDG_.APITAG_UPPER_LIMIT_CTRL, upperLimitCtrl, m_token);
                    if (!resp.IsSuccess())
                        return false;
                    resp = nle_api.Cmds(UDG_.DEVICE_ID, UDG_.APITAG_LOWER_LIMIT_CTRL, lowerLimitCtrl, m_token);
                    if (!resp.IsSuccess())
                        return false;
                    _waitTime = 0;
                    return true;
                }
            });
        }

        private int _waitTime = 0;
        private const int WAIT_TIME_MAX = 10;
        private int blink = 0;
        /// <summary>
        /// 更新状态
        /// </summary>
        private void Run()
        {
            while (isRunning)
            {
    
                START: Thread.Sleep(100);
                _waitTime++;
                lock (_mutex)
                {
                    if (_waitTime < WAIT_TIME_MAX)
                        continue;
                    _waitTime = 0;

                    // 获取设备在线状态
                    {
                        var resp = nle_api.GetDevicesStatus(UDG_.DEVICE_ID.ToString(), m_token);
                        if (!resp.IsSuccess())
                            continue;
                        var rst = resp.ResultObj;
                        if (rst == null)
                            continue;
                        var data = rst.ToList()[0];
                        IsOnline = data.IsOnline;
                        if (!IsOnline)
                        {
                            IsOpen = false;
                            UpperLimit = "";
                            LowerLimit = "";
                            CurrentTemp = "";
                            Alarm = "";
                            continue;
                        }
                    }

                    // 获取其他相关数据
                    {
                        var resp = nle_api.GetSensors(UDG_.DEVICE_ID
                            , $"{UDG_.APITAG_LOWER_LIMIT},{UDG_.APITAG_UPPER_LIMIT},{UDG_.APITAG_CURRENT_TEMP},{UDG_.APITAG_POWER},{UDG_.APITAG_ALARM}"
                            , m_token);
                        if (!resp.IsSuccess())
                            continue;
                        var rst = resp.ResultObj;
                        var list = rst.ToList();
                        foreach (var dto in list)
                        {
                            switch (dto.ApiTag)
                            {
                                case UDG_.APITAG_POWER:
                                    IsOpen = int.Parse(dto.Value.ToString()) == 0 ? false : true;
                                    if (!IsOpen)
                                    {
                                        UpperLimit = "";
                                        LowerLimit = "";
                                        CurrentTemp = "";
                                        Alarm = "";
                                        goto START;
                                    }
                                    break;
                                case UDG_.APITAG_CURRENT_TEMP:
                                    CurrentTemp = dto.Value.ToString();
                                    break;
                                case UDG_.APITAG_UPPER_LIMIT:
                                    UpperLimit = dto.Value.ToString();
                                    break;
                                case UDG_.APITAG_LOWER_LIMIT:
                                    LowerLimit = dto.Value.ToString();
                                    break;
                                case UDG_.APITAG_ALARM:
                                    Alarm = dto.Value.ToString();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    var currentTemp = int.Parse(CurrentTemp);
                    var upperLimit = int.Parse(UpperLimit);
                    var lowerLimit = int.Parse(LowerLimit);
                    var alarm = int.Parse(Alarm);

                    if (currentTemp > upperLimit)
                    {
                        if (blink == 0)
                        {
                            TempCtrlStatusIdx = UDG_.TEMP_CTRL_STATUS_IDX_COOL;
                            blink = 1;
                        }
                        else if (blink == 1)
                        {
                            TempCtrlStatusIdx = UDG_.TEMP_CTRL_STATUS_IDX_ALARM;
                            blink = 0;
                        }
                    }
                    else if (currentTemp > lowerLimit)
                    {
                        TempCtrlStatusIdx = UDG_.TEMP_CTRL_STATUS_IDX_HEAT;
                    }
                    else
                    {
                        TempCtrlStatusIdx = UDG_.TEMP_CTRL_STATUS_IDX_HEAT;
                    }
                }
            }
        }


        private void InitThread()
        {
            if (this.threadWork == null)
            {
                this.isRunning = true;
                this.threadWork = new Thread(new ThreadStart(this.Run));
                this.threadWork.IsBackground = true;
                this.threadWork.Start();
            }
        }

        protected override void OnClose()
        {
            isRunning = false;
            threadWork?.Abort();
            base.OnClose();
        }
    }
}
