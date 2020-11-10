using System;
using NLECloudSDK;

namespace Utilities
{
    /// <summary>
    /// 用户定义的全局变量（Udg：User define global）
    /// </summary>
    public static class UDG_
    {
        private const String API_HOST = nameof(API_HOST); // 云平台API域名
        public const String NLE_API = nameof(NLE_API); // 云平台API接口
        public const String ACCESS_TOKEN = nameof(ACCESS_TOKEN); // 访问令牌

        public const int DEVICE_ID = 145562; // 设备ID

        public const string APITAG_UPPER_LIMIT = "upperLimit";  // 传感器：上限值温度
        public const string APITAG_LOWER_LIMIT = "lowerLimit";  // 传感器：下限值温度
        public const string APITAG_CURRENT_TEMP = "currentTemp";  // 传感器：当前室温
        public const string APITAG_ALARM = "alarm";  // 传感器：温度报警
        public const string APITAG_POWER = "power";  // 传感器：空调开关
        public const string APITAG_UPPER_LIMIT_CTRL = "upperLimitCtrl";  // 执行器：上限值温度
        public const string APITAG_LOWER_LIMIT_CTRL = "lowerLimitCtrl";  // 执行器：下限值温度
        public const string APITAG_POWER_CTRL = "powerCtrl";  // 执行器：空调开关

        public const int TEMP_CTRL_STATUS_IDX_HEAT = 0; // 温控状态：加热
        public const int TEMP_CTRL_STATUS_IDX_COOL = 1; // 温控状态：制冷
        public const int TEMP_CTRL_STATUS_IDX_ALARM = 2; // 温控状态：预警
        public static void Initialize()
        {
            Store.Set(API_HOST, ApplicationSettings.Get(CFG_.API_HOST));
            Store.Set(NLE_API, new NLECloudAPI(Store.Get<String>(API_HOST)));
            Store.Set<object>(ACCESS_TOKEN, null);
        }
    }
}