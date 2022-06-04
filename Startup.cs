using System;
using Api.Database.MySql;
using client.AreasOfPractices;
using client.Enquiry;
using client.Requests;
using client.Utilities;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddSingleton<AWSCognito>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var region = Configuration["CognitoConfiguration:Region"];
                    var userPoolId = Configuration["CognitoConfiguration:UserPoolId"];
                    options.TokenValidationParameters = new TokenValidationParameters {ValidateAudience = false};
                    options.Authority = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}";
                    options.RequireHttpsMetadata = false;
                });
            services.AddAuthorization();
            services.AddScoped<IValidator<RequestInput>, RequestInputValidator>();
            services.AddErrorFilter<GraphQLErrorFilter>();
            services
                .AddCors(options =>
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder.WithOrigins("http://localhost:3001",
                                    "http://localhost:3002",
                                    "http://localhost:3000",
                                    "https://dev-solicitor.helpmycase.co.uk",
                                    "https://solicitor.helpmycase.co.uk",
                                    "https://dev-solicitor.helpmycase.co.uk",
                                    "https://forms.helpmycase.co.uk",
                                    "https://dev-forms.helpmycase.co.uk",
                                    "https://client.helpmycase.co.uk",
                                    "https://dev-client.helpmycase.co.uk")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                        }
                    )
                )
                .AddDbContext<DashboardContext>(
                    options => options.UseMySQL(Configuration.GetConnectionString("DefaultConnection"))
                        .LogTo(Console.WriteLine, LogLevel.Information)
                )
                .AddGraphQLServer()
                .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true)
                .AddProjections()
                .AddHttpRequestInterceptor<HttpRequestInterceptor>()
                .AddAuthorization()
                .AddQueryType(d => d.Name("Query"))
                .AddType<RequestQueries>()
                .AddType<EnquiryQueries>()
                .AddTypeExtension<EnquiryTypeExtension>()
                .AddType<AreasOfPracticeQueries>()
                .AddMutationType(d => d.Name("Mutation"))
                .AddType<RequestMutations>();
        }
     // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors();
            app.UseSerilogRequestLogging();
            
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapGraphQL();
            });
        }
    }
}