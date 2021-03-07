using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Threading.Tasks;

namespace jsonapi_consumer_sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateWebHostBuilder();
            await host.RunAsync();
        }

        public static IWebHost CreateWebHostBuilder()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            #region Config

            string environment = config.GetValue<string>("Environment");

            string httpUrl = config.GetValue<string>("Kestrel:Http:Url");
            int httpPort = config.GetValue<int>("Kestrel:Http:Port");

            #endregion

            AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);

            return new WebHostBuilder()
                    .UseConfiguration(config)
                    .UseStartup<Startup>()
                    .UseKestrel(options =>
                    {
                        options.AddServerHeader = false;
                        options.AllowSynchronousIO = true;
                        if (!string.IsNullOrEmpty(httpUrl) && httpPort != 0)
                        {
                            options.Listen(IPAddress.Parse(httpUrl), httpPort);
                        }

                    })
                    .UseEnvironment(environment)
                    .UseShutdownTimeout(TimeSpan.FromSeconds(2))
                    .Build();
        }
    }
}
