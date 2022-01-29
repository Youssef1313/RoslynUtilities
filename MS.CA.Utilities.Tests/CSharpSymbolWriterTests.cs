using Microsoft.CodeAnalysis;
using MS.CA.Utilities.Generators;
using MS.CA.Utilities.Services;
using Xunit;

namespace MS.CA.Utilities.Tests
{
    public class CSharpSymbolWriterTests
    {
        [Fact]
        public void TestWriteNamespace()
        {
            var writer = ServiceProvider.GetLanguageService<IGeneratorWriter>(LanguageNames.CSharp);
        }
    }
}