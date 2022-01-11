using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Http;

namespace client
{
    public class ClientContext
    {
        public string ExternalId{ get; set; }
    }
    public class HttpRequestInterceptor : DefaultHttpRequestInterceptor
    {
        public override ValueTask OnCreateAsync(HttpContext context,
            IRequestExecutor requestExecutor, IQueryRequestBuilder requestBuilder,
            CancellationToken cancellationToken)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            requestBuilder.SetProperty("ClientContext", new ClientContext()
            {
                // ExternalId = "d28ae761-0f3c-4a43-8fc9-136624045e58",
                ExternalId = userId,
            });

            return base.OnCreateAsync(context, requestExecutor, requestBuilder,
                cancellationToken);
        }
    }
}