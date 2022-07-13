using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using MS.CA.Utilities.CSharp.Generators;
using MS.CA.Utilities.Generators;
using Xunit;

namespace MS.CA.Utilities.Tests
{
    public class CSharpSymbolWriterTests
    {
        private static string NormalizeLineEndings(string input)
        {
            if (input.Contains("\n") && !input.Contains("\r\n"))
            {
                input = input.Replace("\n", "\r\n");
            }

            return input;
        }

        private static async Task<CSharpCompilation> CreateCompilationAsync(string source)
        {
            ImmutableArray<MetadataReference> metadataReferences = await ReferenceAssemblies.Default.ResolveAsync(LanguageNames.CSharp, CancellationToken.None).ConfigureAwait(false);
            return CSharpCompilation.Create("MyAssembly", new[] { SyntaxFactory.ParseSyntaxTree(source, options: new CSharpParseOptions().WithLanguageVersion(LanguageVersion.Preview)) }, references: metadataReferences);
        }

        private static IGeneratorWriter CreateService() => new CSharpGeneratorWriter();

        [Fact]
        public async Task TestWriteNamespace()
        {
            Compilation compilation = await CreateCompilationAsync(@"namespace A.B.C { public class MyClass { } }").ConfigureAwait(false);
            INamedTypeSymbol? type = compilation.GetTypeByMetadataName("A.B.C.MyClass");
            AssertEx.NotNull(type);
            INamespaceSymbol @namespace = type.ContainingNamespace;

            IGeneratorWriter writer = CreateService();
            using (writer.WriteSymbol(@namespace, includeContainingSymbol: true))
            {
                writer.Builder.Append(writer.GetIndentation() + "Custom line 1...\r\n");
                writer.WriteIndented("Custom line 2...\r\n");
            }

            Assert.Equal(NormalizeLineEndings(
@"namespace A.B.C
{
    Custom line 1...
    Custom line 2...
}
"),
writer.Builder.ToString());
        }

        [Fact]
        public async Task TestWriteNestedNamespace()
        {
            Compilation compilation = await CreateCompilationAsync(@"namespace A.B.C { public class MyClass { } }").ConfigureAwait(false);
            INamedTypeSymbol? type = compilation.GetTypeByMetadataName("A.B.C.MyClass");
            AssertEx.NotNull(type);
            INamespaceSymbol @namespace = type.ContainingNamespace;

            IGeneratorWriter writer = CreateService();
            using (writer.WriteSymbol(@namespace, includeContainingSymbol: true))
            {
                using (writer.WriteSymbol(@namespace, includeContainingSymbol: true))
                {
                    writer.WriteIndented("Test1\r\n");
                }

                writer.WriteIndented("Test2\r\n");
            }

            Assert.Equal(NormalizeLineEndings(
@"namespace A.B.C
{
    namespace A.B.C
    {
        Test1
    }
    Test2
}
"),
writer.Builder.ToString());
        }

        [Fact]
        public async Task TestWriteNamedType()
        {
            Compilation compilation = await CreateCompilationAsync(@"namespace A.B.C { public class MyClass { public class MyClassNested { } } }").ConfigureAwait(false);
            INamedTypeSymbol? type = compilation.GetTypeByMetadataName("A.B.C.MyClass+MyClassNested");
            AssertEx.NotNull(type);

            IGeneratorWriter writer = CreateService();
            using (writer.WriteSymbol(type, includeContainingSymbol: true))
            {
                writer.WriteIndented("// Code goes here...\r\n");
            }

            Assert.Equal(NormalizeLineEndings(
@"namespace A.B.C
{
    partial class MyClass
    {
        partial class MyClassNested
        {
            // Code goes here...
        }
    }
}
"),
writer.Builder.ToString());
        }

        [Fact]
        public async Task TestNoContainingSymbol()
        {
            Compilation compilation = await CreateCompilationAsync(@"namespace A.B.C { public class MyClass { public class MyClassNested { } } }").ConfigureAwait(false);
            INamedTypeSymbol? type = compilation.GetTypeByMetadataName("A.B.C.MyClass+MyClassNested");
            AssertEx.NotNull(type);

            IGeneratorWriter writer = CreateService();
            using (writer.WriteSymbol(type, includeContainingSymbol: false))
            {
                writer.WriteIndented("// Code goes here...\r\n");
            }

            Assert.Equal(NormalizeLineEndings(
@"partial class MyClassNested
{
    // Code goes here...
}
"),
writer.Builder.ToString());
        }

        [Fact]
        public async Task TestWriteLinesIndented()
        {
            Compilation compilation = await CreateCompilationAsync(@"namespace A.B.C { public class MyClass { } }").ConfigureAwait(false);
            INamedTypeSymbol? type = compilation.GetTypeByMetadataName("A.B.C.MyClass");
            AssertEx.NotNull(type);

            IGeneratorWriter writer = CreateService();
            using (writer.WriteSymbol(type, includeContainingSymbol: false))
            {
                writer.WriteLinesIndented(@"// Code goes here...
// Second line goes here...");
            }

            Assert.Equal(NormalizeLineEndings(
@"partial class MyClass
{
    // Code goes here...
    // Second line goes here...
}
"),
writer.Builder.ToString());
        }

        [Theory]
        [InlineData("class")]
        [InlineData("record")]
        [InlineData("record class")]
        [InlineData("struct")]
        [InlineData("record struct")]
        [InlineData("interface")]
        public async Task TestGenericType(string type)
        {
            Compilation compilation = await CreateCompilationAsync($@"namespace A.B.C {{ public {type} MyType<T> {{ }} }}").ConfigureAwait(false);
            INamedTypeSymbol? typeSymbol = compilation.GetTypeByMetadataName("A.B.C.MyType`1");
            AssertEx.NotNull(typeSymbol);

            IGeneratorWriter writer = CreateService();
            using (writer.WriteSymbol(typeSymbol, includeContainingSymbol: false))
            {
                writer.WriteLinesIndented(@"// Code goes here...
// Second line goes here...");
            }

            string expectedType = type switch
            {
                "record class" => "record",
                _ => type,
            };
            Assert.Equal(NormalizeLineEndings(
$@"partial {expectedType} MyType<T>
{{
    // Code goes here...
    // Second line goes here...
}}
"),
writer.Builder.ToString());
        }
    }
}
