using JsonApiConsumer;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace jsonapi_consumer_sample.HttpServiceClients
{
    public class SampleHttpServiceClient : HttpServiceClientBase, ISampleHttpServiceClient
    {
        public SampleHttpServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) : base(httpClient, httpContextAccessor)
        {

        }

        protected override void AddDefaultHeaders()
        {
            base.AddDefaultHeaders();
        }
    }
}
