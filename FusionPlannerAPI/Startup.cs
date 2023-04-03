using FusionPlannerAPI.Gateways;
using FusionPlannerAPI.Gateways.Interfaces;
using FusionPlannerAPI.Infrastructure;
using FusionPlannerAPI.Services;
using FusionPlannerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace FusionPlannerAPI;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        GenerateDbContext(services);

        services.AddTransient<ICardGateway, CardGateway>();
        services.AddTransient<IColumnGateway, ColumnGateway>();
        services.AddTransient<IBoardGateway, BoardGateway>();

        services.AddTransient<ICardService, CardService>();
        services.AddTransient<IColumnService, ColumnService>();
        services.AddTransient<IBoardService, BoardService>();

        services.AddCors();
        services.AddControllers();
    }

    private static void GenerateDbContext(IServiceCollection services)
    {
        //var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        var connectionString = "Host=localhost;Port=5432;Database=FusionPlanner;Username=postgres;Password=password";

        services.AddDbContext<PlannerDbContext>(
                opt => opt
                    .UseLazyLoadingProxies()
                    .UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}