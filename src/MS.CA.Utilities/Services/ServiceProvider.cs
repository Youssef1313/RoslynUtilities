using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;

namespace MS.CA.Utilities.Services
{
    public static class ServiceProvider
    {
        private static readonly CompositionContainer s_container;

        static ServiceProvider()
        {
            var catalog = new AggregateCatalog();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            }

            s_container = new CompositionContainer(catalog);
            s_container.ComposeParts();
        }

        public static T GetLanguageService<T>(string languageName) where T : ILanguageService
        {
            return s_container.GetExports<T>().Single(s => s.Value.LanguageName == languageName).Value;
        }
    }
}
