using Elfie.Serialization;
using Microsoft.Extensions.Localization;
using System.Reflection;

namespace NestHR.LanguageSupport
{
    public class ResourceLang
    {
    }

    public class LanguageService
    {
        private readonly IStringLocalizer _localizer;

        public LanguageService(IStringLocalizerFactory factory)
        {
            var type = typeof(ResourceLang);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("ResourceLang", assemblyName.Name);
        }

        public LocalizedString Getkey(string key)
        {
            return _localizer[key];
        }
    }
}
