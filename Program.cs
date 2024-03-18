using Domin.Models;
using HRService.GeneralDefinitionService;
using HRService.GeneralDefinitionService.Interfaces;
using Microsoft.EntityFrameworkCore;


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

            #region ============> [Scoped Genral]

            builder.Services.AddScoped<IHRDefinitionWrapper, HRDefinitionWrapper>();


            #endregion [Scoped Genral] 

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
