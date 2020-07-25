using HealthTracker.DAL.Contexts;
using HealthTracker.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Qoollo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors();

            services.AddScoped<IRepository<Controller>, PgControllerRepository>();
            services.AddScoped<IRepository<Sensor>, PgSensorRepository>();
            services.AddScoped<IUnitOfWork, PgUnitOfWork>();

            var influxDb = InfluxDbClient.GetInstance();
            influxDb.SetConfiguration(Configuration);

            services.AddAutoMapper(typeof(ControllerProfile), typeof(SensorProfile));

            services.AddEntityFrameworkNpgsql().AddDbContext<PgDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors(builder => builder.AllowAnyOrigin()
                                          .AllowAnyHeader()
                                          .AllowAnyMethod());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
