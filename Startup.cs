using payment_api.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using payment_api.Infrastructure.Database;
using payment_api.Models.Service;

namespace payment_api
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
            var serverDbOptions = new DbContextOptionsBuilder<ServerDbContext>();
            serverDbOptions.UseNpgsql(Configuration.GetSection("ServerDbSettings")["ConnectionString"]);

            services.AddSingleton<ServerDbContext>(new ServerDbContext(serverDbOptions.Options));

            services.TryAddSingleton<IPaymentDbService, PaymentDbService>();

            services.TryAddSingleton<IPaymentInstallmentDbService, PaymentInstallmentDbService>();

            services.TryAddSingleton<IAnticipationService, AnticipationService>();

            services.TryAddSingleton<IPaymentProcessService, PaymentProcessService>();

            services.TryAddSingleton<IResultService, ResultService>();

            services.AddControllers();

            services.AddSwaggerConfiguration();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwaggerConfiguration();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
