using System;
using System.Text;
using Microsoft.CodeAnalysis;
using MS.CA.Utilities.Generators;

namespace MS.CA.Utilities.CSharp.Generators
{
    public sealed class CSharpGeneratorWriter : IGeneratorWriter
    {
        private static readonly string[] s_newLineSeparators = new[] { "\r\n", "\n" };

        private readonly CSharpGeneratorWriterOptions _options;
        private readonly StringBuilder _builder = new();
        private int _indentationLevel;

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

        /// <summary>
        /// Gets a disposable symbol writer that can be used as:
        /// <code>
        /// using (generatorWriter.WriteSymbol(symbolToWrite, includeContainingSymbol)
        /// {
        ///     // Write something inside the symbol here.
        /// }
        /// </code>
        /// </summary>
        /// <param name="symbol">The symbol to write. Currently supporting <see cref="INamespaceSymbol"/>, <see cref="INamedTypeSymbol"/></param>
        /// <param name="includeContainingSymbol">Whether to write the containing symbols.</param>
        /// <exception cref="ArgumentNullException"><paramref name="symbol"/> is null</exception>
        /// <remarks>
        /// Suppose you want to write code for class <c>C3</c> and also <c>C2</c> in
        /// <code>
        /// public class C1
        /// {
        ///     public class C2
        ///     {
        ///         public class C3
        ///         {
        ///         }
        ///     }
        /// }
        /// </code>
        /// Such that the generated code looks like this:
        /// <code>
        /// public class C1
        /// {
        ///     public class C2
        ///     {
        ///         // Code in C2
        ///         public class C3
        ///         {
        ///             // Code in C3
        ///         }
        ///     }
        /// }
        /// </code>
        /// What you'll do is:
        /// <code>
        /// using (generatorWriter.WriteSymbol(c2Symbol, includeContainingSymbol: true)
        /// {
        ///     generatorWriter.WriteIndented("// Code in C2");
        ///     using (generatorWriter.WriteSymbol(c3, includeContainingSymbol: false)
        ///     {
        ///         generatorWriter.WriteIndented("// Code in C3");
        ///     }
        /// }
        /// </code>
        /// That's where <paramref name="includeContainingSymbol"/> can be useful.
        /// </remarks>
        public IDisposable WriteSymbol(ISymbol symbol, bool includeContainingSymbol)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            return GetSymbolWriter(this, symbol, includeContainingSymbol).WriteBegin();
        }

        internal static DisposableWriter GetSymbolWriter(IGeneratorWriter generatorWriter, ISymbol symbol, bool includeContainingSymbol)
        {
            return symbol switch
            {
                INamespaceSymbol namespaceSymbol => new NamespaceWriter(generatorWriter, namespaceSymbol),
                INamedTypeSymbol namedTypeSymbol => new NamedTypeWriter(generatorWriter, namedTypeSymbol, includeContainingSymbol),
                IMethodSymbol methodSymbol => new MethodWriter(generatorWriter, methodSymbol, includeContainingSymbol),
                _ => throw new ArgumentException($"Unexpected symbol type '{symbol.Kind}'", nameof(symbol))
            };
        }

        public IGeneratorWriter WriteIndented(string text)
        {
            _builder.Append(GetIndentation());
            _builder.Append(text);
            return this;
        }

        public IGeneratorWriter WriteLinesIndented(string text)
        {
            text ??= string.Empty;

            string[] lines = text.Split(s_newLineSeparators, StringSplitOptions.None);
            foreach (string line in lines)
            {
                WriteIndented(line).WriteLine();
            }

            return this;
        }

        public IGeneratorWriter WriteLine()
        {
            _builder.Append("\r\n");
            return this;
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}
