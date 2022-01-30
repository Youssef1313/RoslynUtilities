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
    /// the indentation if necessary. The returned start text should use the existing indentation
    /// level, meaning that manipulating indentation happens after start text is already calculated.
    /// For end text, indentation manipulation should happen first. For example,
    /// <code>
    /// [write_start]
    /// public class C
    /// {
    /// [indent]
    ///     // Code inside
    /// [deindent]
    /// [write_end]
    /// }
    /// </code>
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
