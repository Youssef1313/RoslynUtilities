using System;
using System.Text;
using Microsoft.CodeAnalysis;

namespace MS.CA.Utilities.Generators
{
    /// <summary>
    /// Base class for all symbol writers.
    /// </summary>
    /// <remarks>
    /// Each symbol writer will override <see cref="DisposableWriter.GetStartText"/> and
    /// <see cref="DisposableWriter.GetEndText"/>, and is responsible for manipulating
    /// the indentation if necessary. The returned start/end text should use the existing indentation
    /// level, meaning that manipulating indentation happens after start/end text is already calculated.
    /// </remarks>
    internal abstract class SymbolWriter<T> : DisposableWriter where T : ISymbol
    {
        public T Symbol { get; }

        public SymbolWriter(IGeneratorWriter writer, T symbol) : base(writer)
        {
            Symbol = symbol;
        }
    }
}
