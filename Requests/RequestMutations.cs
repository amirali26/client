using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Database.Models;
using Api.Database.MySql;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

// Whats the different between async task & task
namespace client.Requests
{
    [ExtendObjectType(Name = "Mutation")]
    public class RequestMutations
    {
        public async Task<IQueryable<Request>> AddRequest(
            [Service] DashboardContext context, [Service] AWSCognito cognitoHelper, RequestInput requestInput
        )
        {
            // Check if the user exists
           var response =  await cognitoHelper.IsExistingUser(requestInput.Email);
           Client client;
           if (response == null)
           {
               // Create new user
               var createNewUserResponse = await cognitoHelper.CreateNewUser(requestInput.Email, $"+44{requestInput.PhoneNumber}", requestInput.Name);
               Client newClient = new Client()
               {
                   ExternalId = createNewUserResponse.User.Username,
                   Name = requestInput.Name,
                   CreatedAt = createNewUserResponse.User.UserCreateDate,
                   PhoneNumber = $"+44{requestInput.PhoneNumber}",
                   Email = requestInput.Email,
                   DateOfBirth = "15/04/1996"
               };
               var createdClient = await context.Clients.AddAsync(newClient);
               client = createdClient.Entity;
           }
           else
           {
               client = await context.Clients.Where(c => c.ExternalId == response.Username).FirstAsync();
           };
           
           var areaOfPractice = await context.AreasOfPractice.Where(aop => aop.ExternalId == requestInput.Topic).FirstAsync();
           var request = new Request()
           {
               ExternalId = Guid.NewGuid().ToString(),
               Description = requestInput.Description,
               Topic = areaOfPractice,
               CreatedDate = DateTime.Now,
               Region = requestInput.Region,
               PostCode = requestInput.PostCode,
               AreaInRegion = requestInput.AreaInRegion,
               Client = client,
               
           };

           await context.Requests.AddAsync(request);
           await context.SaveChangesAsync();

           return context.Requests.Where(r => r.ExternalId == request.ExternalId);
        }
    }
}