using System;
using System.Collections.Generic;
using Api.Database.Models;

namespace client.Account
{
    public class AccountDto
    {
        public string Name { get; set; }
        public string Website { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PostCode { get; set; }
        public string Region { get; set; }
        public string AreaInRegion { get; set; }
        public DateTime RegisteredDate { get; set; }
        public IEnumerable<string> AreasOfPractice { get; set; }
    }
}