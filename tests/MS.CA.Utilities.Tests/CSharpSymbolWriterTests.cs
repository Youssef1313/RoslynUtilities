using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MS.CA.Utilities.Generators;
using MS.CA.Utilities.Services;
using Xunit;

namespace MS.CA.Utilities.Tests
{
    public class CSharpSymbolWriterTests
    {
        private CSharpCompilation CreateCompilation(string source)
        {
            return CSharpCompilation.Create("MyAssembly", new[] { SyntaxFactory.ParseSyntaxTree(source) });
        }

        [Fact]
        public void TestWriteNamespace()
        {
            var compilation = CreateCompilation(@"namespace A.B.C { public class MyClass { } }");
            var type = compilation.GetTypeByMetadataName("A.B.C.MyClass");
            var @namespace = type.ContainingNamespace;

            var writer = ServiceProvider.GetLanguageService<IGeneratorWriter>(LanguageNames.CSharp);
            using (writer.WriteSymbol(@namespace))
            {
                writer.Builder.AppendLine(writer.GetIndentation() + "Custom line 1...");
                writer.WriteIndented("Custom line 2...\r\n");
            }

            Assert.Equal(
@"namespace A.B.C
{
    Custom line 1...
    Custom line 2...
}
",
writer.Builder.ToString());
        }

        [Fact]
        public void TestWriteNestedNamespace()
        {
            var compilation = CreateCompilation(@"namespace A.B.C { public class MyClass { } }");
            var type = compilation.GetTypeByMetadataName("A.B.C.MyClass");
            var @namespace = type.ContainingNamespace;

            var writer = ServiceProvider.GetLanguageService<IGeneratorWriter>(LanguageNames.CSharp);
            using (writer.WriteSymbol(@namespace))
            {
                using (writer.WriteSymbol(@namespace))
                {
                    writer.WriteIndented("Test1\r\n");
                }

                writer.WriteIndented("Test2\r\n");
            }

            Assert.Equal(
@"namespace A.B.C
{
    namespace A.B.C
    {
        Test1
    }
    Test2
}
",
writer.Builder.ToString());
        }
    }
}