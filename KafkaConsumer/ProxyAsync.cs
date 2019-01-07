using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.IO;

namespace KafkaConsumer
{
    //class ProxyAsync
    public static class ProxyAsync
    {
        public static string MDMSVC_URL
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["MDMSVCUrl"];
            }
        }

        public static void PostAsync(ProxyFor For, string URI, object Param, Type RequestType)
        {
            string AbsPath = string.Empty;
            if (For == ProxyFor.MDM_CONSUMER)
            {
                AbsPath = MDMSVC_URL;
            }

            string requestUri = AbsPath + URI;

            DataContractJsonSerializer serializerToUpload = new DataContractJsonSerializer(RequestType);
            string body = string.Empty;
            using (var memoryStream = new MemoryStream())
            {
                using (var reader = new StreamReader(memoryStream))
                {
                    serializerToUpload.WriteObject(memoryStream, Param);
                    memoryStream.Position = 0;
                    body = reader.ReadToEnd();
                }
            }
            serializerToUpload = null;

            HttpClient hc = new HttpClient();
            StringContent json = new StringContent(body, Encoding.UTF8, "application/json");
            hc.PostAsync(requestUri, json);
        }

        public static void GetAsync(ProxyFor For, string URI)
        {
            string AbsPath = string.Empty;
            if (For == ProxyFor.MDM_CONSUMER)
            {
                AbsPath = MDMSVC_URL;
            }

            string requestUri = AbsPath + URI;

            HttpClient hc = new HttpClient();
            hc.GetAsync(requestUri);
        }
    }
}
