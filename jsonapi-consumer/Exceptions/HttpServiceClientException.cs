using JsonApiConsumer.Enumerations;

namespace JsonApiConsumer.Exceptions
{
    public class HttpServiceClientException : ExceptionBase
    {
        public string subStatusCode { get; set; }
        public string[] parameters { get; set; }

        public HttpServiceClientException(int statusCode, string resourceKey = "", bool isLocalized = true, params string[] parameters) : base(resourceKey.Split(';'), statusCode)
        {
            this.parameters = parameters;
        }

        public HttpServiceClientException(int statusCode, string[] resourceKeys, bool isLocalized = true, params string[] parameters) : base(resourceKeys, statusCode)
        {
            this.parameters = parameters;
        }

        public HttpServiceClientException(string subStatusCode, string resourceKey = "", bool isLocalized = true, params string[] parameters) : base(resourceKey.Split(';'), HttpStatusCode.UnprocessableEntity)
        {
            this.subStatusCode = subStatusCode;
            this.parameters = parameters;
        }
    }
}