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
        private static bool RequestsSearchTermParser(string searchTermInput, Request r, ClientContext clientContext)
        {
            if (string.IsNullOrWhiteSpace(searchTermInput))
            {
                return r.Client.ExternalId == clientContext.ExternalId;
            }

            return r.Client.ExternalId == clientContext.ExternalId && (
                r.RequestNumber.ToString().Contains(searchTermInput) ||
                r.Topic.ToString().Contains(searchTermInput) ||
                r.Client.Email.Contains(searchTermInput));
        }

        [UseProjection]
        public IQueryable<Request> GetRequests([Service] DashboardContext context,
            [GlobalState("ClientContext")] ClientContext clientContext,
            string searchTermInput)
        {
            return context.Requests.Where(r =>
                RequestsSearchTermParser(searchTermInput, r, clientContext)).OrderByDescending(r => r.CreatedDate);
        }

        [UseProjection]
        public IQueryable<Request> GetRequest([Service] DashboardContext context,
            [GlobalState("ClientContext")] ClientContext clientContext, string requestId)
        {
            return context.Requests.Where(r =>
                r.Client.ExternalId == clientContext.ExternalId && r.ExternalId == requestId);
        }
    }
}