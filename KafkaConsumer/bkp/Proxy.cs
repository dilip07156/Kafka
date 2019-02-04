using System;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace KafkaConsumer
{
    public static class Proxy
    {
        public static string MDMSVC_URL
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["MDMSVCUrl"];
            }
        }

        private static HttpClient client = null;

        private static readonly object padlock = new object();

        public static async Task<TResponse> Get<TResponse>(string RelativeUrl)
        {
            //using (HttpClient client = new HttpClient())
            //{
            client = Instance;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(MDMSVC_URL + RelativeUrl);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<TResponse>();
            }
            else
            {
                return default(TResponse);
            }
            //}
        }

        public static async Task<TResponse> Post<TResponse, TRequest>(string RelativeUrl, TRequest value)
        {
            client = Instance;
            //client.Timeout = TimeSpan.FromMilliseconds(2000);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            DataContractJsonSerializer serializerToUpload = new DataContractJsonSerializer(value.GetType());

            ByteArrayContent content = null;

            using (var memoryStream = new MemoryStream())
            {
                using (var reader = new StreamReader(memoryStream))
                {
                    serializerToUpload.WriteObject(memoryStream, value);
                    memoryStream.Position = 0;
                    string body = reader.ReadToEnd();
                    var buffer = System.Text.Encoding.UTF8.GetBytes(body);
                    content = new ByteArrayContent(buffer);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }
            }

            HttpResponseMessage response = await client.PostAsync((MDMSVC_URL + RelativeUrl), content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<TResponse>();
            }
            else
            {
                return default(TResponse);
            }

        }


        public static HttpClient Instance
        {
            get
            {
                lock (padlock)
                {
                    if (client == null)
                    {
                        client = new HttpClient();
                        client.Timeout = TimeSpan.FromMilliseconds(2000);
                    }
                    return client;
                }
            }
        }
    }
}
