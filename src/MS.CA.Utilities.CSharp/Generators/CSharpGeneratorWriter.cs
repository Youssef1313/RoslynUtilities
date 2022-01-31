using System;
using System.Text;
using Microsoft.CodeAnalysis;
using MS.CA.Utilities.Generators;

namespace MS.CA.Utilities.CSharp.Generators
{
    public sealed class CSharpGeneratorWriter : IGeneratorWriter
    {
        private readonly CSharpGeneratorWriterOptions _options;
        private int _indentationLevel;
        private readonly StringBuilder _builder = new();

        public CSharpGeneratorWriter()
        {
            _options = CSharpGeneratorWriterOptions.Default;
        }

        public CSharpGeneratorWriter(CSharpGeneratorWriterOptions options)
        {
            _options = options;
        }

        public StringBuilder Builder => _builder;

        public GeneratorWriterOptions Options => _options;

        public string GetIndentation()
        {
            if (_options.UseTabsForIndentation)
            {
                return new string('\t', _indentationLevel);
            }

            return new string(' ', _indentationLevel * 4);
        }

        public void IncreaseIndentationLevel()
        {
            _indentationLevel++;
        }

        public void DecreaseIndentationLevel()
        {
            if (_indentationLevel == 0)
            {
                throw new InvalidOperationException();
            }

            _indentationLevel--;
        }

        public IDisposable WriteSymbol(ISymbol symbol)
        {
            return symbol switch
            {
                INamespaceSymbol namespaceSymbol => new NamespaceWriter(this, namespaceSymbol).WriteBegin(),
                _ => throw new ArgumentException("Unexpected symbol type", nameof(symbol))
            };
        }

        public void WriteIndented(string text)
        {
            _builder.Append(GetIndentation());
            _builder.Append(text);
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}
