using System;
using System.Collections.Generic;
using System.Text;

namespace CN.SMSSDK.Sharp.Models.Dtos
{
    public class GetTokenParamDto
    {
        public string duid { get; set; }
        public string sign { get; set; }
        public string sdkver { get; set; }
        public string appkey { get; set; }
        public string aesKey { get; set; }
        public int plat { get; set; }
    }

    public class GetTokenResDto
    {
        public int status { get; set; }
        public string message { get; set; }
        public GetTokenResWrapperDto result { get; set; }
    }

    public class GetTokenResWrapperDto
    {
        public string token { get; set; }
        public string timestamp { get; set; }
        public string key { get; set; }
    }
}
