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

        internal override string GetEndText()
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

            _generatorWriter.DecreaseIndentationLevel();

            string indentation = _generatorWriter.GetIndentation();

            return indentation + "}\r\n";
        }

        internal override string GetStartText()
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

            string indentation = _generatorWriter.GetIndentation();
            _generatorWriter.IncreaseIndentationLevel();
            return $"{indentation}namespace {Symbol.ToDisplayString()}\r\n{indentation}{{\r\n";
        }
    }
}
