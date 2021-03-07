using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace JsonApiConsumer.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<HttpClientResponse<TModel>> DeserializeAsHttpClientResponse<TModel>(this HttpResponseMessage result) where TModel : class, new()
        {
            var httpClientResponse = new HttpClientResponse<TModel>();
            if (result.IsSuccessStatusCode)
            {
                string contentString = await result.Content.ReadAsStringAsync();
                var model = contentString.Length > 0 ? Newtonsoft.Json.JsonConvert.DeserializeObject<TModel>(contentString) : default(TModel);
                httpClientResponse.Data = model;
                httpClientResponse.StatusCode = (int)result.StatusCode;
                httpClientResponse.IsSuccessStatusCode = true;
            }
            else
            {
                string contentString = await result.Content.ReadAsStringAsync();

                try
                {
                    httpClientResponse = contentString.Length > 0 ? Newtonsoft.Json.JsonConvert.DeserializeObject<HttpClientResponse<TModel>>(contentString) : new HttpClientResponse<TModel>();
                }
                catch (Exception)
                {
                    httpClientResponse.Errors = new System.Collections.Generic.List<Error>();
                    httpClientResponse.Errors.Add(new Error() { description = contentString, code = contentString });
                }

                httpClientResponse.IsSuccessStatusCode = false;
                httpClientResponse.StatusCode = (int)result.StatusCode;
            }
            return httpClientResponse;
        }
    }
}
