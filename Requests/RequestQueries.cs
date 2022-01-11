using System.Linq;
using Api.Database.Models;
using Api.Database.MySql;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;

namespace client.Requests
{
    [ExtendObjectType(Name = "Query")]
    public class RequestQueries
    {
        [UseProjection]
        public IQueryable<Request> GetRequests([Service] DashboardContext context,
            [GlobalState("ClientContext")] ClientContext clientContext)
        {
            return context.Requests.Where(r => r.Client.ExternalId == clientContext.ExternalId).OrderByDescending(r => r.CreatedDate);
        }

        [UseProjection]
        public IQueryable<Request> GetRequest([Service] DashboardContext context,
            [GlobalState("ClientContext")] ClientContext clientContext, string requestId)
        {
            return context.Requests.Where(r => r.Client.ExternalId == clientContext.ExternalId && r.ExternalId == requestId);
        }
    }
}