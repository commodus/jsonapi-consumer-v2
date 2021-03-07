namespace JsonApiConsumer.Enumerations
{
    public static class HttpStatusCode
    {
        public const int OK = 200;
        public const int BadRequest = 400;
        public const int UnAuthorized = 401;
        public const int Forbidden = 403;
        public const int NotFound = 404;
        public const int Conflict = 409;
        public const int PreConditionFailed = 412;
        public const int UnprocessableEntity = 422;
    }
}
