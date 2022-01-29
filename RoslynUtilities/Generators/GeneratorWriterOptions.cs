using Microsoft.CodeAnalysis.CSharp;

namespace MS.CA.Utilities.Generators
{
    public class GeneratorWriterOptions
    {
        public static GeneratorWriterOptions Default { get; } = new();

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

        public static GeneratorWriterOptions FromCompilation(CSharpCompilation compilation)
        {
            return new GeneratorWriterOptions
            {
                UseFileScopedNamespaces = compilation.LanguageVersion >= LanguageVersionEx.CSharp10,
                UseTabsForIndentation = false,
            };
        }
    }
}
