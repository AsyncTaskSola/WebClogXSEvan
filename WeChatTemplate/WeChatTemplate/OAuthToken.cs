using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChatTemplate
{
    /// <summary>
    /// 订阅通知 WeChat 官方模板
    /// </summary>
    public class SendWeChatPaymentServiceModel
    {
        /// <summary>
        /// 接收者（用户）的 openid
        /// </summary>
        public string touser { get; set; }
        /// <summary>
        /// 所需下发的订阅模板id
        /// </summary>
        public string template_id { get; set; }
        /// <summary>
        /// 跳转网页时填写
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 跳转小程序时填写，格式如{ "appid": "pagepath": { "value": any } }
        /// </summary>
        //public List<object> miniprogram { get; set; }
        /// <summary>
        /// 模板内容，格式形如 { "key1": { "value": any }, "key2": { "value": any } }
        /// </summary>
        public Dictionary<string, DataValue> data { get; set; }
    }
    /// <summary>
    /// 订阅参数值
    /// </summary>
    public class DataValue
    {
        public string value { get; set; }
    }

    /// <summary>
    /// auth.getAccessToken
    /// </summary>
    public class OAuthToken
    {
        /// <summary>
        /// 获取到的凭证
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 凭证有效时间，单位：秒。目前是7200秒之内的值。
        /// </summary>
        public int expires_in { get; set; }
        /// <summary>
        /// 错误码
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string errmsg { get; set; }
    }

    public class ETTemplate
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
    }
}
