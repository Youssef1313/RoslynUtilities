using System;
using System.Text;
using Microsoft.CodeAnalysis;

namespace MS.CA.Utilities.Generators
{
    internal abstract class SymbolWriter<T> : DisposableWriter where T : ISymbol
    {
        public T Symbol { get; }

        public SymbolWriter(IGeneratorWriter writer, T symbol) : base(writer)
        {
            Symbol = symbol;
        }
    }
}
