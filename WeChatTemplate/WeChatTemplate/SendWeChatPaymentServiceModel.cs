using System;

namespace WeChatTemplate
{
    internal class SendWeChatPaymentServiceModel
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    public static int SendWeChatPaymentService()
    {
        var db = UnitOfWork.Instance("").GetDbClient();
        var entitylist = db.Queryable<WebDriver>().Where(x => x.WeChatMessage == 1).ToList();
        //生成所有账单
        var contractbill = db.Queryable<BASE_CONTRACTMONTHBILL>().ToList();
        var dist = entitylist.GroupBy(x => x.Phone).ToDictionary(x => x.Key, x => x.ToList()).ToList();
        var newlist = new List<WebDriver>();
        foreach (var item in dist)
        {
            var list = item.Value;
            double? w = 0;
            var name = "";
            var openid = "";
            foreach (var co in list)
            {
                var res = co.ContractNo;
                var cont = contractbill.Where(x => x.MAINCONTRACTNO == res).Sum(x => x.SCANTYAMOUNT);
                w = w + cont;
                name = co.Name;
                openid = co.OpenId;
            }
            newlist.Add(new WebDriver { Name = name, Phone = item.Key, Amount = (double)w, OpenId = openid });
        }


        if (entitylist.Count == 0)
        {
            return -2;
        }

        var month = DateTime.Now.Month;
        var appid = WebConfigurationManager.AppSettings["WechatComponentAppId"].ToString();
        var secret = WebConfigurationManager.AppSettings["WechatComponentSecret"].ToString(); //微信公众号appsecret
                                                                                              //auth.getAccessToken
        var authurl = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appid, secret);
        var content = "";
        using (WebClient client = new WebClient() { Encoding = System.Text.Encoding.UTF8 })
        {
            content = client.DownloadString(authurl);
        }
        OAuthToken mode = JsonConvert.DeserializeObject<OAuthToken>(content);
        DefLog.Debug(string.Format("accesstoken:{0}", content));
        if (mode.errcode == 0)
        {
            //send发送订阅通知
            var resurl = string.Format("https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}", mode.access_token);
            //关注者的所有openid
            //var datas = string.Format("https://api.weixin.qq.com/cgi-bin/user/get?access_token={0}",mode.access_token);
            //using (WebClient client = new WebClient() { Encoding = System.Text.Encoding.UTF8 })
            //{
            //    content = client.DownloadString(datas);
            //}
            newlist.ForEach(x =>
            {
                var jsonResult = "";
                try
                {
                    var sendWechat = new SendWeChatPaymentServiceModel();
                    sendWechat.touser = x.OpenId;
                    //根据openid找到对应的人员信息
                    //var resinfo = string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN", mode.access_token, x.OpenId);
                    //using (WebClient client = new WebClient() { Encoding = System.Text.Encoding.UTF8 })
                    //{
                    //    content = client.DownloadString(resinfo);
                    //}
                    sendWechat.url = WebConfigurationManager.AppSettings["template_url"].ToString(); ;
                    sendWechat.template_id = WebConfigurationManager.AppSettings["template_id"].ToString();
                    sendWechat.data = new Dictionary<string, DataValue>();
                    sendWechat.data.Add("first", new DataValue { value = "账单缴费通知" });
                    sendWechat.data.Add("keyword1", new DataValue { value = month + "月" });
                    sendWechat.data.Add("keyword2", new DataValue { value = x.Amount.ToString() });
                    sendWechat.data.Add("remark", new DataValue { value = x.Name + "      " + x.Phone });
                    string sendData = JsonConvert.SerializeObject(sendWechat);
                    jsonResult = HttpPost.PostHttps(resurl, sendData);
                    ETTemplate et = JsonConvert.DeserializeObject<ETTemplate>(jsonResult);
                    if (et.errcode == 0)
                    {
                        DefLog.Debug(string.Format("当前司机：{0} 手机号：{1}订阅信息成功,内容{2}", x.Name, x.Phone, jsonResult));
                    }
                    else
                    {
                        DefLog.Error(string.Format("当前司机：{0} 手机号：{1}订阅信息失败,内容{2}", x.Name, x.Phone, jsonResult));
                    }
                }
                catch (Exception)
                {
                    DefLog.Error(string.Format("当前司机：{0} 手机号：{1}订阅信息失败,内容{2}", x.Name, x.Phone, jsonResult));
                }
            });
            return 1;
        }
        return -1;
    }
}
