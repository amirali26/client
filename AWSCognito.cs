using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.Extensions.Configuration;

namespace client
{
    public class AWSCognito
    {
        private readonly AmazonCognitoIdentityProviderClient client;
        private readonly string appClientId;
        private readonly string userPoolId;

        public AWSCognito(IConfiguration configuration)
        {
            client = new AmazonCognitoIdentityProviderClient();
            appClientId = configuration["CognitoConfiguration:AppClientId"];
            userPoolId = configuration["CognitoConfiguration:UserPoolId"];
        }

        public async Task<AdminGetUserResponse?> IsExistingUser(string email)
        {
            try
            {
                var request = new AdminGetUserRequest()
                {
                    Username = email,
                    UserPoolId = userPoolId
                };

                return await client.AdminGetUserAsync(request);
            }
            catch (UserNotFoundException e)
            {
                return null;
            }
        }

        public async Task<AdminCreateUserResponse> CreateNewUser(string email, string phoneNumebr, string name)
        {
            var phoneNumberAttr = new AttributeType()
            {
                Name = "phone_number",
                Value = phoneNumebr
            };
            var nameAttr = new AttributeType()
            {
                Name = "name",
                Value = name
            };

            var request = new AdminCreateUserRequest()
            {
                Username = email,
                UserAttributes = new List<AttributeType>() {phoneNumberAttr, nameAttr},
                UserPoolId = userPoolId,
                MessageAction = "SUPPRESS",
                DesiredDeliveryMediums = null,
            };

            return await client.AdminCreateUserAsync(request);
        }
    }
}