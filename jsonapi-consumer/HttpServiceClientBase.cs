using JsonApiConsumer.Exceptions;
using JsonApiConsumer.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace JsonApiConsumer
{
    public class HttpServiceClientBase : IHttpServiceClientBase
    {
        protected readonly HttpClient httpClient;
        private readonly IHttpContextAccessor httpContextAccessor;

        public HttpServiceClientBase(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.httpContextAccessor = httpContextAccessor;
        }

        protected virtual void AddDefaultHeaders()
        {
            httpClient.DefaultRequestHeaders.Clear();
        }

        public T CallHttpService<T>(string overridePath = "", string overrideQueryString = "") where T : class, new()
        {
            var method = httpContextAccessor.HttpContext.Request.Method;
            var path = httpContextAccessor.HttpContext.Request.Path;
            var queryString = httpContextAccessor.HttpContext.Request.QueryString;
            var body = httpContextAccessor.HttpContext.Request.Body;
            var response = httpContextAccessor.HttpContext.Response;

            string fullPath = path;

            if (!string.IsNullOrEmpty(overridePath))
                fullPath = overridePath;

            if (!string.IsNullOrEmpty(overrideQueryString))
                fullPath += overrideQueryString;
            else
                fullPath += queryString.ToUriComponent();

            #region HttpHeader

            AddDefaultHeaders();

            #endregion

            #region Request Body

            var requestBody = string.Empty;
            if (body.CanSeek)
            {
                body.Seek(0, SeekOrigin.Begin);

                StreamReader reader = new StreamReader(body);
                requestBody = reader.ReadToEnd();

                body.Seek(0, SeekOrigin.Begin);
            }

            var requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");

            #endregion

            HttpResponseMessage responseMessage;
            switch (method.ToLowerInvariant())
            {
                case "get":
                    responseMessage = httpClient.GetAsync(fullPath).ConfigureAwait(false).GetAwaiter().GetResult();
                    break;
                case "put":
                    responseMessage = httpClient.PutAsync(fullPath, requestContent).ConfigureAwait(false).GetAwaiter().GetResult();
                    break;
                case "post":
                    responseMessage = httpClient.PostAsync(fullPath, requestContent).ConfigureAwait(false).GetAwaiter().GetResult();
                    break;
                case "delete":
                    responseMessage = httpClient.DeleteAsync(fullPath).ConfigureAwait(false).GetAwaiter().GetResult();
                    break;
                default:
                    throw new NotSupportedException("not supporter http method");
            }

            if (responseMessage == null)
            {
                throw new InvalidOperationException("Response message cannot be null");
            }

            using (responseMessage)
            {
                response.StatusCode = (int)responseMessage.StatusCode;

                var responseFeature = httpContextAccessor.HttpContext.Features.Get<IHttpResponseFeature>();
                if (responseFeature != null)
                {
                    responseFeature.ReasonPhrase = responseMessage.ReasonPhrase;
                }

                var responseHeaders = responseMessage.Headers;

                // Ignore the Transfer-Encoding header if it is just "chunked".
                // We let the host decide about whether the response should be chunked or not.
                if (responseHeaders.TransferEncodingChunked == true &&
                    responseHeaders.TransferEncoding.Count == 1)
                {
                    responseHeaders.TransferEncoding.Clear();
                }

                foreach (var header in responseHeaders)
                {
                    response.Headers.Append(header.Key, header.Value.ToArray());
                }

                if (responseMessage.Content != null)
                {
                    var contentHeaders = responseMessage.Content.Headers;

                    foreach (var header in contentHeaders)
                    {
                        response.Headers.Append(header.Key, header.Value.ToArray());
                    }

                    var clientBody = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    response.WriteAsync(clientBody);
                }
            }

            var result = responseMessage.DeserializeAsHttpClientResponse<T>().GetAwaiter().GetResult();
            if (!responseMessage.IsSuccessStatusCode)
            {
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
                    throw new HttpServiceClientException(result.SubStatusCode, result?.Errors?.FirstOrDefault().code ?? "");
                else
                    throw new HttpServiceClientException((int)responseMessage.StatusCode, result?.Errors?.Select(x => x.code).ToArray() ?? new string[] { });
            }

            return result.Data;
        }

    }
}
