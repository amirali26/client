using System;
using Amazon.CloudWatchLogs;
using client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.AwsCloudWatch;

namespace client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var client = new AmazonCloudWatchLogsClient();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.AmazonCloudWatch(
                    logGroup: "dev-HandleMyCaseEcsSetup-ClientLogGroup94520E53-1aKmWNVBWqxU",
                    logStreamPrefix: DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"),
                    // The AWS CloudWatch client to use
                    cloudWatchClient: client)
                .WriteTo.Console()
                .CreateLogger();
            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}