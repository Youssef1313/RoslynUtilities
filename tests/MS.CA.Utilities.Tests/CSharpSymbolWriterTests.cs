﻿using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MS.CA.Utilities.CSharp.Generators;
using MS.CA.Utilities.Generators;
using Xunit;

namespace MS.CA.Utilities.Tests
{
    public class CSharpSymbolWriterTests
    {
        private static string NormalizeLineEndings(string input)
        {
            if (input.Contains('\n', StringComparison.Ordinal) && !input.Contains("\r\n", StringComparison.Ordinal))
            {
                input = input.Replace("\n", "\r\n", StringComparison.Ordinal);
            }

            return input;
        }

        private static CSharpCompilation CreateCompilation(string source)
        {
            return CSharpCompilation.Create("MyAssembly", new[] { SyntaxFactory.ParseSyntaxTree(source) });
        }

        private static IGeneratorWriter CreateService() => new CSharpGeneratorWriter();

        [Fact]
        public void TestWriteNamespace()
        {
            Compilation compilation = CreateCompilation(@"namespace A.B.C { public class MyClass { } }");
            INamedTypeSymbol type = compilation.GetTypeByMetadataName("A.B.C.MyClass");
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
        public void TestWriteNestedNamespace()
        {
            Compilation compilation = CreateCompilation(@"namespace A.B.C { public class MyClass { } }");
            INamedTypeSymbol type = compilation.GetTypeByMetadataName("A.B.C.MyClass");
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
        public void TestWriteNamedType()
        {
            Compilation compilation = CreateCompilation(@"namespace A.B.C { public class MyClass { public class MyClassNested { } } }");
            INamedTypeSymbol type = compilation.GetTypeByMetadataName("A.B.C.MyClass+MyClassNested");

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
        public void TestNoContainingSymbol()
        {
            Compilation compilation = CreateCompilation(@"namespace A.B.C { public class MyClass { public class MyClassNested { } } }");
            INamedTypeSymbol type = compilation.GetTypeByMetadataName("A.B.C.MyClass+MyClassNested");

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
        public void TestWriteLinesIndented()
        {
            Compilation compilation = CreateCompilation(@"namespace A.B.C { public class MyClass { } }");
            INamedTypeSymbol type = compilation.GetTypeByMetadataName("A.B.C.MyClass");

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
    }
}
