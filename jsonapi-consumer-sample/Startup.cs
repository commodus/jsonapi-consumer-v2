using jsonapi_consumer_sample.HttpServiceClients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace jsonapi_consumer_sample
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpResponseFeature, HttpResponseFeature>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            #region Endpoints

            var sampleHttpServiceClientEndpoint = Configuration.GetValue<string>("HttpClients:SampleHttpServiceClient:Url");

            #endregion

            #region Timeouts

            var sampleHttpServiceClientTimeout = Configuration.GetValue<int>("HttpClients:SampleHttpServiceClient:Timeout");

            #endregion

            #region HttpMessageHandlers

            var bypassCertValidation = true;

            Func<HttpMessageHandler> configureHandler = () =>
            {
                var handler = new HttpClientHandler();

                if (bypassCertValidation)
                {
                    handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, x509Certificate2, x509Chain, sslPolicyErrors) =>
                    {
                        return true;
                    };
                }
                return handler;
            };

             #endregion

            #region HttpClients

            services.AddHttpClient<ISampleHttpServiceClient, SampleHttpServiceClient>(x => { x.BaseAddress = new Uri(sampleHttpServiceClientEndpoint); x.Timeout = TimeSpan.FromSeconds(sampleHttpServiceClientTimeout); })
            .ConfigurePrimaryHttpMessageHandler(configureHandler);

            #endregion

            #region CORS

            bool isCorsEnabled = Configuration.GetValue<bool>("Cors:Enable");
            bool allowAll = Configuration.GetValue<bool>("Cors:AllowAll");
            string allowedOrigins = Configuration.GetValue<string>("Cors:AllowedOrigins");

            if (isCorsEnabled)
            {
                services.AddCors(options => options.AddPolicy("CorsPolicy", b =>
                {
                    b.AllowAnyMethod()
                     .AllowAnyHeader()
                     .AllowCredentials();

                    if (allowAll)
                        b.AllowAnyOrigin();
                    else
                        b.WithOrigins(allowedOrigins.Split(";"));
                }));
            }

            #endregion

            services.AddMvcCore();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            #region CORS

            bool isCorsEnabled = Configuration.GetValue<bool>("Cors:Enable");
            if (isCorsEnabled)
                app.UseCors("CorsPolicy");

            #endregion

            app.UseMvc();
        }
    }
}
