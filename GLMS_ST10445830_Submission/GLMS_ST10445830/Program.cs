    using Microsoft.EntityFrameworkCore;
using GLMS_ST10445830.Data;
using GLMS_ST10445830.Services;
namespace GLMS_ST10445830
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Adds the different service (that are patterns) to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient(); 

            builder.Services.AddScoped<IServiceRequestFactory, ServiceRequestFactory>();
            
            builder.Services.AddScoped<FixedRateStrategy>();

            builder.Services.AddScoped<CurrencyConverter>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var logger = provider.GetRequiredService<ILogger<CurrencyConverter>>();
                var config = provider.GetRequiredService<IConfiguration>();
                var fallback = provider.GetRequiredService<FixedRateStrategy>();

                return new CurrencyConverter(httpClientFactory, logger, config, fallback);
            });

            builder.Services.AddScoped<ICurrencyConverter>(provider =>
                provider.GetRequiredService<CurrencyConverter>());

            // fall back if api doesnt work / builder.Services.AddScoped<ICurrencyConverter, FixedRateStrategy>();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
               
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}