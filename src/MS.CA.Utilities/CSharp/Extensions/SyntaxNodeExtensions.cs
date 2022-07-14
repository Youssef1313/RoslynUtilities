using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MS.CA.Utilities.CSharp.Extensions
{
    internal static class SyntaxNodeExtensions
    {
        public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2)
        {
            SyntaxKind kind = node.Kind();
            return kind == kind1 ||
                kind == kind2;
        }
    }
}
