using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logger.DbLog;
using Logger.DbLog.extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Logging.Framework.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //services.Configure()
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseWhen(context => !SkipMiddleware(context), appBuilder =>
            {
                var logWriter = new LogWriter(Configuration.GetConnectionString("Logging"));
                loggerFactory.AddDbLogger(Configuration.GetSection("Logging"), logWriter);
                // app.UseAuthorization();
            });
            app.UseHttpsRedirection();

            app.UseRouting();
            
           

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private bool SkipMiddleware(HttpContext context)
        {
            return context.Request.Path.Value.Contains("swagger", StringComparison.OrdinalIgnoreCase) ||
                  context.Request.Path.Value.Contains("ping", StringComparison.OrdinalIgnoreCase) ||
                  context.Request.Path == "/";
        }
    }
}
