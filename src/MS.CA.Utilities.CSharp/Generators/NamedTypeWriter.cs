using Microsoft.CodeAnalysis;
using MS.CA.Utilities.Generators;

namespace MS.CA.Utilities.CSharp.Generators
{
    internal sealed class NamedTypeWriter : SymbolWriter<INamedTypeSymbol>
    {
        private readonly IGeneratorWriter _generatorWriter;
        private readonly DisposableWriter _parentWriter;

        public NamedTypeWriter(IGeneratorWriter generatorWriter, INamedTypeSymbol symbol) : base(generatorWriter, symbol)
        {
            _generatorWriter = generatorWriter;
            _parentWriter = CSharpGeneratorWriter.GetSymbolWriter(generatorWriter, symbol.ContainingSymbol);
        }

        internal override string GetEndText()
        {
            _generatorWriter.DecreaseIndentationLevel();
            string indentation = _generatorWriter.GetIndentation();
            return indentation + "}\r\n" + _parentWriter?.GetEndText();
        }

        internal override string GetStartText()
        {
            // TODO: Generic classes, interfaces with variance, prefix keyword-like named types with @
            string parentResult = _parentWriter.GetStartText();
            string indentation = _generatorWriter.GetIndentation();
            _generatorWriter.IncreaseIndentationLevel();
            return $"{parentResult}{indentation}partial class {Symbol.Name}\r\n{indentation}{{\r\n";
        }
    }
}
