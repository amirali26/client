using System.Linq;
using Api.Database.MySql;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;

namespace client.Enquiry
{
    [ExtendObjectType(Name = "Query")]
    public class EnquiryQueries
    {
        [UseProjection]
        public IQueryable<Api.Database.Models.Enquiry> GetEnquiries([Service] DashboardContext context,
            [GlobalState("ClientContext")] ClientContext clientContext, string requestId)
        {
            return context.Enquiries.Where(e => e.Request.ExternalId == requestId && e.Request.Client.ExternalId == requestId);
        }
    }
}