using Microsoft.CodeAnalysis;
using MS.CA.Utilities.Generators;

namespace MS.CA.Utilities.CSharp.Generators
{
    internal sealed class NamespaceWriter : SymbolWriter<INamespaceSymbol>
    {
        private readonly IGeneratorWriter _generatorWriter;

        public NamespaceWriter(IGeneratorWriter generatorWriter, INamespaceSymbol symbol) : base(generatorWriter, symbol)
        {
            _generatorWriter = generatorWriter;
        }

        protected override string GetEndText()
        {
            if (Symbol.IsGlobalNamespace)
            {
                return string.Empty;
            }

            var options = (CSharpGeneratorWriterOptions)_generatorWriter.Options;
            if (options.UseFileScopedNamespaces)
            {
                return string.Empty;
            }

            var indentation = _generatorWriter.GetIndentation();
            _generatorWriter.DecreaseIndentationLevel();

            return indentation + "}\r\n";
        }

        protected override string GetStartText()
        {
            if (Symbol.IsGlobalNamespace)
            {
                return string.Empty;
            }

            var options = (CSharpGeneratorWriterOptions)_generatorWriter.Options;
            if (options.UseFileScopedNamespaces)
            {
                return $"namespace {Symbol.ToDisplayString()};\r\n";
            }

            var indentation = _generatorWriter.GetIndentation();
            _generatorWriter.IncreaseIndentationLevel();
            return $"{indentation}namespace {Symbol.ToDisplayString()}\r\n{{\r\n";
        }
    }
}
