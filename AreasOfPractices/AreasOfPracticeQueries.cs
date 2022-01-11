using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Database.Models;
using Api.Database.MySql;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace client.AreasOfPractices
{
    [ExtendObjectType(Name = "Query")]
    public class AreasOfPracticeQueries
    {
        public async Task<List<AreasOfPractice>> GetAreasOfPractices([Service] DashboardContext context)
        {
            return await context.AreasOfPractice.Select(x => x).ToListAsync();
        }
    }
}