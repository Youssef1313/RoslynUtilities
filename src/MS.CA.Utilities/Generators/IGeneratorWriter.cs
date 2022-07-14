using System;
using System.Text;
using Microsoft.CodeAnalysis;

namespace MS.CA.Utilities.Generators
{
    public interface IGeneratorWriter
    {
        StringBuilder Builder { get; }

        GeneratorWriterOptions Options { get; }

        void IncreaseIndentationLevel();

        void DecreaseIndentationLevel();

        string GetIndentation();

        IDisposable WriteSymbol(ISymbol symbol, bool includeContainingSymbol);

        IGeneratorWriter WriteIndented(string text);

        IGeneratorWriter WriteLinesIndented(string text);
        IGeneratorWriter WriteLine();
    }
}
