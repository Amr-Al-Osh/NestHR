using Domin.Models;
using HRService.GeneralDefinitionService;
using HRService.GeneralDefinitionService.Interfaces;
using HRService.LogHR;
using HRService.LogHR.Interfaces;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NestHR.LanguageSupport;
using System.Globalization;
using System.Reflection;


namespace NestHR
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<NestHrContext>(option =>
            option.UseSqlServer(builder.Configuration.GetConnectionString("HRConn")));

            #region =====================> [Language Support]

            builder.Services.AddSingleton<LanguageService>();
            builder.Services.AddLocalization(options => options.ResourcesPath = "LanguageSupport");
            builder.Services.AddMvc()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = new AssemblyName(typeof(ResourceLang).GetTypeInfo().Assembly.FullName);
                        return factory.Create("ResourceLang", assemblyName.Name);
                    };
                });

            builder.Services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo>
                        {
                            new CultureInfo("en-US"),
                            new CultureInfo("ar-JO"),
                            new CultureInfo("fr-FR")
                        };
                  
                    options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;

                    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
                });

            #endregion [Language Support]

            #region ============> [Scoped Genral]

            builder.Services.AddScoped<IHRDefinitionWrapper, HRDefinitionWrapper>();
            builder.Services.AddScoped<IHrLogWarpper, HrLogWarpper>();

            #endregion [Scoped Genral] 

            builder.Services.AddControllersWithViews()
            .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization();

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

            app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}");

            app.Run();
        }
    }
}
