using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Composition;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.VisualStudio.Composition;
using MS.CA.Utilities.Generators;


// ExportProviderFactory, MefV1HostServices, https://github.com/dotnet/roslyn/issues/34994

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

Console.WriteLine();


namespace MS.CA.Utilities.Generators
{
    public interface IGeneratorWriter
    {

    }

    [Export(typeof(IGeneratorWriter))]
    public class GeneratorWriter : IGeneratorWriter
    {
        private readonly GeneratorWriterOptions _options;
        private int _indentationLevel;

        public GeneratorWriter(GeneratorWriterOptions options)
        {
            _options = options;
        }

        public GeneratorWriter(CSharpCompilation compilation)
        {
            _options = new GeneratorWriterOptions
            {
                UseFileScopedNamespaces = compilation.LanguageVersion >= LanguageVersionEx.CSharp10,
                UseTabsForIndentation = false,
            };
        }

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
    }
}
