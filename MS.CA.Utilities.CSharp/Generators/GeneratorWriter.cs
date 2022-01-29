using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using Microsoft.CodeAnalysis;
using MS.CA.Utilities.Generators;

namespace MS.CA.Utilities.CSharp.Generators
{
    [Export(typeof(IGeneratorWriter))]
    public sealed class GeneratorWriter : IGeneratorWriter
    {
        private readonly GeneratorWriterOptions _options = GeneratorWriterOptions.Default;
        private int _indentationLevel;

        [ImportingConstructor]
        public GeneratorWriter()
        {
        }

        public string LanguageName => LanguageNames.CSharp;

        private string GetIndentation()
        {
            if (_options.UseTabsForIndentation)
            {
                return new string('\t', _indentationLevel);
            }

            return new string(' ', _indentationLevel * 4);
        }
    }
}
