using System.Linq;
using Api.Database.MySql;
using client.Account;
using client.User;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace client.Enquiry
{
    [ExtendObjectType(Name = "Query")]
    public class EnquiryQueries
    {
        [UseProjection]
        public IQueryable<Api.Database.Models.Enquiry> GetEnquiries([Service] DashboardContext context,
            [GlobalState("ClientContext")] ClientContext clientContext, string requestId)
        {
            return context.Enquiries.Where(e =>
                e.Request.ExternalId == requestId);
        }
    }

    [ExtendObjectType(typeof(Api.Database.Models.Enquiry))]
    public class EnquiryTypeExtension
    {
        [BindMember(nameof(Api.Database.Models.Enquiry.Account))]
        [UseProjection]
        public AccountDto GetAccount(
            [Service] DashboardContext context, [Parent] Api.Database.Models.Enquiry enquiry)
        {
            return context.Enquiries.Include(e => e.Account).Where(e => e.ExternalId == enquiry.ExternalId).Select(e =>
                new AccountDto()
                {
                    Name = e.Account.Name,
                    Email = e.Account.Email,
                    Website = e.Account.Website,
                    PhoneNumber = e.Account.PhoneNumber,
                    RegisteredDate = e.Account.RegisteredDate,
                    ImageUrl = e.Account.ImageUrl,
                    AreasOfPractice = e.Account.AreasOfPractice.Select(aop => aop.Name).AsEnumerable(),
                }).First();
        }

        [BindMember(nameof(Api.Database.Models.Enquiry.User))]
        [UseProjection]
        public UserDto GetUser(
            [Service] DashboardContext context, [Parent] Api.Database.Models.Enquiry enquiry)
        {
            return context.Enquiries.Include(e => e.User).Where(e => e.ExternalId == enquiry.ExternalId).Select(e =>
                new UserDto()
                {
                    Email = e.User.Email,
                    Name = e.User.Name,
                    PhoneNumber = e.User.PhoneNumber,
                    ImageUrl = e.User.ImageUrl,
                }).First();
        }
    }
}