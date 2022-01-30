using System;
using System.Text;
using Microsoft.CodeAnalysis;
using MS.CA.Utilities.Services;

namespace MS.CA.Utilities.Generators
{
    public interface IGeneratorWriter : ILanguageService
    {
        StringBuilder Builder { get; }

        GeneratorWriterOptions Options { get; }

        void IncreaseIndentationLevel();

        void DecreaseIndentationLevel();

        string GetIndentation();

        IDisposable WriteSymbol(ISymbol symbol);

        void WriteIndented(string text);
    }
}
