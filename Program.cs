using Domin.Models;
using HRService.GeneralDefinition.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NestHR.BusinessHR.GeneralDefinition;

namespace NestHR
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<NestHrContext>(option =>
            option.UseSqlServer(builder.Configuration.GetConnectionString("HRConn")));


            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IAreaService, AreaService>();

            builder.Services.AddAuthentication().AddJwtBearer();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}");

            app.Run();
        }
    }
}
