namespace JsonApiConsumer.Exceptions
{
    public abstract class ExceptionBase : System.Exception
    {
        public int StatusCode { get; set; }
        public string[] ResourceKeys { get; }

        public ExceptionBase(string[] resourceKeys, int statusCode = -1) : base(string.Join(";", resourceKeys))
        {
            StatusCode = statusCode;
            ResourceKeys = resourceKeys;
        }
    }
}