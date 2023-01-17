using CN.SMSSDK.Sharp.Models;
using CN.SMSSDK.Sharp.Models.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Teru.Code.Models;
using Teru.Code.Services;

namespace CN.SMSSDK.Sharp
{
    public static class MobService
    {
        public static CommonResult<string> GetDuid()
        {
            try
            {
                var url = "http://devs.data.mob.com/dinfo";
                var headers = new Dictionary<string, string>();
                headers.Add("Content-Type", "application/x-www-form-urlencoded");
                headers.Add("User-Agent", "mob/1.0");
                var form = new Dictionary<string, string>();
                form.Add("m", AESHelper.EncryptString(DeviceInfo.GetRandomDevice()));

                var res = Web.Post(url, headers, form, true);

                if (!res.success)
                    return new CommonResult<string>(false, res.message);

                var json = JsonConvert.DeserializeObject<DinfoDto>(res.result);

                if (json.status != 200)
                    return new CommonResult<string>(false, json.message ?? "内部错误");

                return new CommonResult<string>(true, "成功", json.duid);
            }
            catch (Exception ex)
            {
                return new CommonResult<string>(false, ex.Message);
            }
        }

        public static CommonResult<bool> Dsign()
        {
            if (SMSSDK.AppKey == null)
                return new CommonResult<bool>(false, "请先Init");
            try
            {
                var url = "http://devs.data.mob.com/dsign";
                var headers = new Dictionary<string, string>();
                headers.Add("Content-Type", "application/x-www-form-urlencoded");
                headers.Add("User-Agent", "mob/1.0");

                var form = new Dictionary<string, string>();
                form.Add("product", "SMSSDK");
                form.Add("appkey", SMSSDK.AppKey);
                form.Add("duid", SMSSDK.Duid);
                form.Add("apppkg", SMSSDK.AppPkgName);
                form.Add("appver", "1");
                form.Add("sdkver", "25");
                form.Add("network", "wifi");

                var res = Web.Post(url, headers, form, true);

                if (!res.success)
                    return new CommonResult<bool>(false, res.message);

                var json = JsonConvert.DeserializeObject<DsignDto>(res.result);

                if (json.status != 200)
                    return new CommonResult<bool>(false, json.message ?? "内部错误");

                return new CommonResult<bool>(true, "成功", json.reup);
            }
            catch (Exception ex)
            {
                return new CommonResult<bool>(false, ex.Message);
            }
        }

        public static CommonResult<GetTokenResWrapperDto> GetToken()
        {
            if (SMSSDK.AppKey == null)
                return new CommonResult<GetTokenResWrapperDto>(false, "请先Init");
            try
            {
                var url = "http://sdkapi.sms.mob.com/token/get";
                
                var dto = new GetTokenParamDto()
                {
                    duid = SMSSDK.Duid,
                    appkey = SMSSDK.AppKey,
                    sign = HashHelper.MD5(SMSSDK.AppPkgName),
                    sdkver = "2.1.2",
                    plat = 1,
                    aesKey = HashHelper.MD5(DateTime.Now.Ticks.ToString())
                };
                var bodystr = JsonConvert.SerializeObject(dto);
                var bodybytes = System.Text.Encoding.UTF8.GetBytes(bodystr);
                var bodybytesenc = RSAHelper.Encrypt(bodybytes);
                
                var headers = new Dictionary<string, string>();
                headers.Add("Content-Type", "application/x-www-form-urlencoded");
                headers.Add("User-Agent", "SMSSDK/2.1.2 (Android; 11/30) Google/Phone Example/com.example.testandroid/1.1.20");
                headers.Add("Accept-Encoding", "gzip");
                headers.Add("Transfer-Encoding", "chunked");
                headers.Add("appkey", SMSSDK.AppKey);
                headers.Add("token", "");
                headers.Add("hash", HashHelper.CRC32(bodybytesenc));
                
                var res = Web.PostRaw(url, headers, new MemoryStream(bodybytesenc));

                if (!res.success)
                    return new CommonResult<GetTokenResWrapperDto>(false, res.message);

                Stream resrawstream = res.result2;
                MemoryStream resrawstream1 = new MemoryStream();
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = resrawstream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    resrawstream1.Write(buffer, 0, bytesRead);
                }
                resrawstream.Close();
                resrawstream1.Position = 0;

                var br = new BinaryReader(resrawstream1);
                var resrawbytes = br.ReadBytes((int)resrawstream1.Length);

                var dec = RSAHelper.Decrypt(resrawbytes);
                var decstr = Encoding.UTF8.GetString(dec);

                var json = JsonConvert.DeserializeObject<GetTokenResDto>(decstr);

                if (json.status != 200)
                    return new CommonResult<GetTokenResWrapperDto>(false, json.message ?? "内部错误");
                SMSSDK.Token = json.result.token;
                return new CommonResult<GetTokenResWrapperDto>(true, "成功", json.result);
            }
            catch (Exception ex)
            {
                return new CommonResult<GetTokenResWrapperDto>(false, ex.Message);
            }
        }

        public static CommonResult<SendCodeResDto> SendCode(string zone, string phone)
        {
            if (SMSSDK.AppKey == null)
                return new CommonResult<SendCodeResDto>(false, "请先Init");
            if (SMSSDK.Token == null)
            {
                var res = GetToken();
                if (!res.Success)
                    return new CommonResult<SendCodeResDto>(false, res.Message);
            }
            try
            {
                var url = "http://code.sms.mob.com/verify/code";

                var dto = new SendCodeParamDto()
                {
                    duid = SMSSDK.Duid,
                    appkey = SMSSDK.AppKey,
                    zone = zone,
                    phone = phone
                };
                var bodystr = JsonConvert.SerializeObject(dto);
                var bodybytes = System.Text.Encoding.UTF8.GetBytes(bodystr);
                var bodybytesenc = RSAHelper.Encrypt(bodybytes);

                var headers = new Dictionary<string, string>();
                headers.Add("Content-Type", "application/x-www-form-urlencoded");
                headers.Add("User-Agent", "SMSSDK/2.1.2 (Android; 11/30) Google/Phone Example/com.example.testandroid/1.1.20");
                headers.Add("Accept-Encoding", "gzip");
                headers.Add("Transfer-Encoding", "chunked");
                headers.Add("appkey", SMSSDK.AppKey);
                headers.Add("token", SMSSDK.Token);
                headers.Add("hash", HashHelper.CRC32(bodybytesenc));

                var res = Web.PostRaw(url, headers, new MemoryStream(bodybytesenc));

                if (!res.success)
                    return new CommonResult<SendCodeResDto>(false, res.message);

                Stream resrawstream = res.result2;
                MemoryStream resrawstream1 = new MemoryStream();
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = resrawstream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    resrawstream1.Write(buffer, 0, bytesRead);
                }
                resrawstream.Close();
                resrawstream1.Position = 0;

                var br = new BinaryReader(resrawstream1);
                var resrawbytes = br.ReadBytes((int)resrawstream1.Length);

                var dec = RSAHelper.Decrypt(resrawbytes);
                var decstr = Encoding.UTF8.GetString(dec);

                var json = JsonConvert.DeserializeObject<SendCodeResDto>(decstr);

                if (json.status != 200)
                    return new CommonResult<SendCodeResDto>(false, json.error ?? "内部错误");

                return new CommonResult<SendCodeResDto>(true, "成功", json);
            }
            catch (Exception ex)
            {
                return new CommonResult<SendCodeResDto>(false, ex.Message);
            }
        }
    }
}
