using HotChocolate;

namespace client.Utilities
{
    public class GraphQLErrorFilter : IErrorFilter
    {
        public IError OnError(IError error)
        {
            return error.Exception != null ? error.WithMessage(error.Exception.Message) : error;
        }
    }
}