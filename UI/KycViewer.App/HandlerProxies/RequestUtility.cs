using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Newtonsoft.Json;

namespace KycViewer.App.HandlerProxies
{
    public static class RequestUtility
    {
        public static StringContent Serialize(object obj)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                TypeNameHandling = TypeNameHandling.All,
                Converters =
                {
                    new MeetingDocumentConverter()
                }
            };

            string json = JsonConvert.SerializeObject(obj, settings);
            StringContent stringHttpContent = new StringContent(json, Encoding.UTF8, "application/json");

            return stringHttpContent;
        }

        public static T Deserialize<T>(string jsonString)
        {
            JsonSerializer jsonSerializer = new();
            return jsonSerializer.Deserialize<T>(new JsonTextReader(new StringReader(jsonString)));
        }
    }

    public class RequestBuilder
    {
        private HttpMethod _method;
        private StringContent _content = null;
        private string _url;

        public RequestBuilder UsingMethod(HttpMethod method)
        {
            _method = method;

            return this;
        }

        public RequestBuilder WithContent(string content)
        {
            _content = new StringContent(content);

            return this;
        }

        public RequestBuilder WithContent(object content)
        {
            _content = RequestUtility.Serialize(content);

            return this;
        }

        public RequestBuilder ToUrl(string url)
        {
            _url = url;

            return this;
        }

        public async Task<T> ExecuteOn<T>(HttpClient httpClient, CancellationToken cancellationToken)
        {
            HttpRequestMessage message = new(_method, _url)
            {
                Content = _content
            };

            message.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            string response = await (
                await httpClient.SendAsync(message, cancellationToken)
            ).Content.ReadAsStringAsync(cancellationToken);

            JsonSerializer jsonSerializer = new();
            T obj = jsonSerializer.Deserialize<T>(new JsonTextReader(new StringReader(response)));

            return obj;
        }

        public async Task<HttpResponseMessage> ExecuteOn(HttpClient httpClient, CancellationToken cancellationToken)
        {
            HttpRequestMessage message = new(_method, _url)
            {
                Content = _content
            };

            message.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            return await httpClient.SendAsync(message, cancellationToken);
        }
    }
}