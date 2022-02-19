using System;

namespace client.Account
{
    public class AccountDto
    {
        public string ExternalId { get; set; }
        public string Name { get; set; }
        string CreatedBy { get; set; }
        DateTime CreatedAt { get; set; }
    }
}