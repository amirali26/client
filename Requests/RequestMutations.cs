using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Database.Models;
using Api.Database.MySql;
using FluentValidation;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

// Whats the different between async task & task
namespace client.Requests
{
    [ExtendObjectType(Name = "Mutation")]
    public class RequestMutations
    {
        private readonly IConfiguration _configuration;
        public RequestMutations(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IQueryable<Request>> AddRequest(
            [Service] DashboardContext context, [Service] AWSCognito cognitoHelper, RequestInput requestInput,
            [Service] IValidator<RequestInput> validator
        )
        {
            var validationResponse = await validator.ValidateAsync(requestInput);

            if (!validationResponse.IsValid)
            {
                throw new ValidationException(validationResponse.Errors);
            }
            
            // Check if the user exists
            var response = await cognitoHelper.IsExistingUser(requestInput.Email);
            Client client;
            if (response == null)
            {
                // Create new user
                var createNewUserResponse = await cognitoHelper.CreateNewUser(requestInput.Email,
                    $"+44{requestInput.PhoneNumber}", requestInput.Name);
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
            }

            var areaOfPractice = await context.AreasOfPractice.Where(aop => aop.ExternalId == requestInput.Topic)
                .FirstAsync();
            var lastRequestNumber = await context.Requests.OrderByDescending(r => r.RequestNumber)
                .Select(r => r.RequestNumber).FirstOrDefaultAsync();
            if (lastRequestNumber == 0)
            {
                lastRequestNumber = 4897;
            }

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
                RequestNumber = lastRequestNumber + 1,
                ShowPhoneNumber = requestInput.ShowPhoneNumber,
            };

            await context.Requests.AddAsync(request);
            await context.SaveChangesAsync();

            dynamic messageBody = new
            {
                RequestEmail = client.Email,
                Name = client.Name,
                Url = _configuration["Url"],
            };

            await AWSHelper.SendEmail(JsonConvert.SerializeObject(messageBody), "RequestSubmission",
                request.ExternalId, _configuration["AWS:SQS:URL"]);
            return context.Requests.Where(r => r.ExternalId == request.ExternalId);
        }

        public async Task<Api.Database.Models.Enquiry> RequestCallback([Service] DashboardContext context, string enquiryId)
        {
            var enquiry = await context.Enquiries.Include(e => e.User)
                .Where(e => e.ExternalId == enquiryId).FirstAsync();
            
            
            dynamic messageBody = new
            {
                RequestEmail = enquiry.User.Email,
                SolicitorResponseNumber = enquiry.EnquiryNumber,
                Url = _configuration["Url"],
            };

            await AWSHelper.SendEmail(JsonConvert.SerializeObject(messageBody), "RequestCallback",
                enquiryId, _configuration["AWS:SQS:URL"]);

            return enquiry;
        }
    }
}