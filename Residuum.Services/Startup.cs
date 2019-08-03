using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Residuum.Services.Database;

namespace Residuum.Services
{
    public class Startup
    {
        private readonly DataCache _cache;

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

            ServiceConfiguration.DatabaseConnectionString = configuration["ConnectionStrings:MainConnection"];
            ServiceConfiguration.PageUri = configuration["Page:Uri"];

            _cache = new DataCache();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var builder = new DbContextOptionsBuilder<CacheContext>();
            builder.UseSqlServer(ServiceConfiguration.DatabaseConnectionString);

            services.AddDbContext<CacheContext>(
                item => item.UseSqlServer(ServiceConfiguration.DatabaseConnectionString));

            
            _cache.Initialize(new CacheContext(builder.Options));

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
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
