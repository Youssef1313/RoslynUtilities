using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MS.CA.Utilities.Generators;
using MS.CA.Utilities.Services;


// ExportProviderFactory, MefV1HostServices, https://github.com/dotnet/roslyn/issues/34994
// https://csharp.hotexamples.com/examples/System.ComponentModel.Composition.Hosting/CompositionContainer/GetExports/php-compositioncontainer-getexports-method-examples.html

//var x = new Lazy<IExportProviderFactory>(
//                () =>
//                {
//                    var discovery = new AttributedPartDiscovery(Resolver.DefaultInstance, isNonPublicSupported: true);
//                    var parts = Task.Run(() => discovery.CreatePartsAsync(typeof(IGeneratorWriter).Assembly)).GetAwaiter().GetResult();
//                    var catalog = ComposableCatalog.Create(Resolver.DefaultInstance).AddParts(parts);
//                    var configuration = CompositionConfiguration.Create(catalog);
//                    var runtimeComposition = RuntimeComposition.CreateRuntimeComposition(configuration);
//                    return runtimeComposition.CreateExportProviderFactory();
//                },
//                LazyThreadSafetyMode.ExecutionAndPublication);

//var exports = x.Value.CreateExportProvider().GetExports<IGeneratorWriter>();

//AggregateCatalog catalog = new AggregateCatalog();
//catalog.Catalogs.Add(new AssemblyCatalog(typeof(IGeneratorWriter).Assembly));

ServiceProvider.GetLanguageService<IGeneratorWriter>(LanguageNames.CSharp);



namespace MS.CA.Utilities.Generators
{
    public interface IGeneratorWriter : ILanguageService
    {
    }

    [Export(typeof(IGeneratorWriter))]
    public sealed class GeneratorWriter : IGeneratorWriter
    {
        private readonly GeneratorWriterOptions _options = GeneratorWriterOptions.Default;
        private int _indentationLevel;

        [ImportingConstructor]
        public GeneratorWriter()
        {
        }

        public string LanguageName => LanguageNames.CSharp;

        private string GetIndentation()
        {
            if (_options.UseTabsForIndentation)
            {
                return new string('\t', _indentationLevel);
            }

            return new string(' ', _indentationLevel * 4);
        }
    }

    public class GeneratorWriterOptions
    {
        public static GeneratorWriterOptions Default { get; } = new();

        /// <summary>
        /// Determines whether <see cref="GeneratorWriter"/> will use file-scoped namespaces.
        /// The default is <see langword="false"/> (use block namespaces).
        /// </summary>
        public bool UseFileScopedNamespaces { get; init; }

        /// <summary>
        /// Determines whether <see cref="GeneratorWriter"/> will use tabs for indentation.
        /// The default is <see langword="false"/> (use spaces).
        /// </summary>
        public bool UseTabsForIndentation { get; init; }

        public static GeneratorWriterOptions FromCompilation(CSharpCompilation compilation)
        {
            return new GeneratorWriterOptions
            {
                UseFileScopedNamespaces = compilation.LanguageVersion >= LanguageVersionEx.CSharp10,
                UseTabsForIndentation = false,
            };
        }
    }
}
