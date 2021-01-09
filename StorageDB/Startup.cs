using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LiteDB;
using StorageDB.Data;
using StorageDB.Services;

namespace StorageDB
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
            services.AddCors();
            services.Configure<LiteDbOptions>(Configuration.GetSection("LiteDbOptions"));
            services.AddSingleton<ILiteDbContext, LiteDbContext>();
            services.AddTransient<ILiteDbCustomerRepository, LiteDbCustomerRepository>();
            services.AddTransient<ILiteDbDeliveryRepository, LiteDbDeliveryRepository>();
            services.AddTransient<ILiteDbItemRepository, LiteDbItemRepository>();
            services.AddTransient<ILiteDbReservationRepository, LiteDbReservationRepository>();
            services.AddTransient<ILiteDbStorageRepository, LiteDbStorageRepository>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IItemService, ItemService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IStorageService, StorageService>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x =>
                x.AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
