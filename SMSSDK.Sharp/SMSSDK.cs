using CN.SMSSDK.Sharp.Models;
using System;
using Teru.Code.Models;

namespace CN.SMSSDK.Sharp
{
    public static class SMSSDK
    {
        public static string AppKey { get; set; }
        public static string AppSecret { get; set; }
        public static string Duid { get; set; }
        public static string AppPkgName { get; set; }
        public static string Token { get; set; }
        public static CommonResult InitSDK(string appKey, string appSecret)
        {
            try
            {
                if (Token != null && AppKey == appKey && AppSecret == appSecret)
                    return new CommonResult(false, "只需要初始化一次");
                AppKey = appKey;
                AppSecret = appSecret;
                Token = null;
                var res = MobService.GetDuid();
                if (!res.Success)
                    return new CommonResult(false, res.Message);
                Duid = res.Result;
                AppPkgName = "com.example.testandroid";
                var res2 = MobService.Dsign();
                if (!res2.Success)
                    return new CommonResult(false, res2.Message);
                return new CommonResult(true, "成功");
            }
            catch(Exception ex)
            {
                return new CommonResult(false, ex.Message);
            }
        }

        public static CommonResult<SendCodeResDto> GetVerificationCode(string zone, string phone)
        {
            return MobService.SendCode(zone, phone);
        }

        
    }
}
