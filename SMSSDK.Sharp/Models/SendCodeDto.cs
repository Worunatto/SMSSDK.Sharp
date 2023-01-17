using System;
using System.Collections.Generic;
using System.Text;

namespace CN.SMSSDK.Sharp.Models
{
    public class SendCodeParamDto
    {
        public string duid { get; set; }
        public string zone { get; set; }
        public string phone { get; set; }
        public string appkey { get; set; }
    }

    public class SendCodeResDto
    {
        public int status { get; set; }
        public string message { get; set; }
        public string error { get; set; }
    }
}
