using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Residuum.Services
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            ServiceConfiguration.BlizzardAuthentication = new Authentication
            {
                ClientId = configuration["Blizzard:ClientId"],
                Secret = configuration["Blizzard:Secret"]
            };

            ServiceConfiguration.GuildName = configuration["Guild:Name"];
            ServiceConfiguration.RealmName = configuration["Guild:Realm"];
            ServiceConfiguration.OverrideRaidProgressionSummary = configuration["Guild:OverrideRaidProgressionSummary"].Equals("True", StringComparison.InvariantCultureIgnoreCase);

            ServiceConfiguration.PageUri = configuration["Page:Uri"];
            
            DataAccessLayer.Initialize();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
