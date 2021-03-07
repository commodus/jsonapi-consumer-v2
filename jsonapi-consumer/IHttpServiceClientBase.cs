namespace JsonApiConsumer
{
    public interface IHttpServiceClientBase
    {
        T CallHttpService<T>(string overridePath = "", string overrideQueryString = "") where T : class, new();
    }
}
