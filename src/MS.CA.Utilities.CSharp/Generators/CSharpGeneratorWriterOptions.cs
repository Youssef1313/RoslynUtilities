using System;
using Microsoft.CodeAnalysis.CSharp;
using MS.CA.Utilities.Generators;

namespace MS.CA.Utilities.CSharp.Generators
{
    public class CSharpGeneratorWriterOptions : GeneratorWriterOptions
    {
        public static CSharpGeneratorWriterOptions Default { get; } = new();

        /// <summary>
        /// Determines whether <see cref="GeneratorWriter"/> will use file-scoped namespaces.
        /// The default is <see langword="false"/> (use block namespaces).
        /// </summary>
        public bool UseFileScopedNamespaces { get; init; }

        /// <summary>
        /// Determines whether <see cref="GeneratorWriter"/> will use tabs for indentation.
        /// The default is <see langword="false"/> (use spaces).
        /// </summary>
        public bool UseTabsForIndentation { get; init; }

        public static CSharpGeneratorWriterOptions FromCompilation(CSharpCompilation compilation)
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            return new CSharpGeneratorWriterOptions
            {
                UseFileScopedNamespaces = compilation.LanguageVersion >= LanguageVersionEx.CSharp10,
                UseTabsForIndentation = false,
            };
        }
    }
}
