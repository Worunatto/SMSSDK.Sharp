using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CN.SMSSDK.Sharp.Models
{
    public partial class DeviceInfo
    {
        [JsonProperty("adsid")]
        public string Adsid { get; set; }

        [JsonProperty("androidid")]
        public string Androidid { get; set; }

        [JsonProperty("carrier")]
        public long Carrier { get; set; }

        [JsonProperty("factory")]
        public string Factory { get; set; }

        [JsonProperty("mac")]
        public string Mac { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("plat")]
        public long Plat { get; set; }

        [JsonProperty("screensize")]
        public string Screensize { get; set; }

        [JsonProperty("serialno")]
        public string Serialno { get; set; }

        [JsonProperty("sysver")]
        public string Sysver { get; set; }

        public static string GetRandomDevice()
        {
            //{"screensize":"1080x2400","factory":"Xiaomi","carrier":-1,"model":"M2104K10AC","sysver":"11","plat":1,"adsid":"658c0d3e-f778-48a9-a5ff-bb8a54b6eaa6","androidid":"c2768c7c33f983f1","mac":"8c:aa:ce:0e:2b:dd","serialno":"5haursbmaujn89e6"}
            var ins = new DeviceInfo() { 
                Screensize = "1080x1920", 
                Factory = "Google", 
                Carrier = -1, 
                Model = "Phone", 
                Sysver = "11", 
                Plat = 1, 
                Adsid = Guid.NewGuid().ToString(), 
                Androidid = "android", 
                Mac = "ff:ff:ff:ff:ff:ff", 
                Serialno = "android" 
            };
            return JsonConvert.SerializeObject(ins);
        }
    }

}
