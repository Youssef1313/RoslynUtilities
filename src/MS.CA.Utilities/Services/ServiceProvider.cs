using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;

namespace MS.CA.Utilities.Services
{
    public static class ServiceProvider
    {
        public static T GetLanguageService<T>(string languageName) where T : ILanguageService
        {
            var catalog = new AggregateCatalog();

            try
            {
                var assembly = Assembly.Load("MS.CA.Utilities.CSharp");
                catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            }
            catch (Exception)
            {
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            }

            var container = new CompositionContainer(catalog);
            container.ComposeParts();
            return container.GetExports<T>().Single(s => s.Value.LanguageName == languageName).Value;
        }
    }
}
