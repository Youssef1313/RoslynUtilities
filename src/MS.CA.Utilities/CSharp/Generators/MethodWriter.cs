using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MS.CA.Utilities.Generators;

namespace MS.CA.Utilities.CSharp.Generators
{
    internal sealed class MethodWriter : SymbolWriter<IMethodSymbol>
    {
        private readonly IGeneratorWriter _generatorWriter;
        private readonly DisposableWriter? _parentWriter;

        private static readonly SymbolDisplayFormat s_format = new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance,
            memberOptions:
                SymbolDisplayMemberOptions.IncludeParameters |
                SymbolDisplayMemberOptions.IncludeType |
                SymbolDisplayMemberOptions.IncludeRef |
                SymbolDisplayMemberOptions.IncludeContainingType,
            kindOptions:
                SymbolDisplayKindOptions.IncludeMemberKeyword,
            parameterOptions:
                SymbolDisplayParameterOptions.IncludeName |
                SymbolDisplayParameterOptions.IncludeType |
                SymbolDisplayParameterOptions.IncludeParamsRefOut |
                SymbolDisplayParameterOptions.IncludeDefaultValue,
            localOptions: SymbolDisplayLocalOptions.IncludeType,
            miscellaneousOptions:
                SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
                SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
                SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

        public MethodWriter(IGeneratorWriter generatorWriter, IMethodSymbol symbol, bool includeContainingSymbol) : base(generatorWriter, symbol)
        {
            _generatorWriter = generatorWriter;
            if (includeContainingSymbol)
            {
                _parentWriter = CSharpGeneratorWriter.GetSymbolWriter(generatorWriter, symbol.ContainingSymbol, includeContainingSymbol: true);
            }
        }

        internal override string GetEndText()
        {
            _generatorWriter.DecreaseIndentationLevel();
            string indentation = _generatorWriter.GetIndentation();
            return indentation + $"}}\r\n{_parentWriter?.GetEndText()}";
        }

        internal override string GetStartText()
        {
            if (Symbol.MethodKind != MethodKind.Ordinary)
            {
                throw new NotSupportedException($"'{Symbol.MethodKind}' is not a supported method kind yet.");
            }

            // TODO: Generic methods, prefix keyword-like names with @, methods with parameters, abstract or extern shouldn't have body, explicit interface implementation, etc...
            string? parentResult = _parentWriter?.GetStartText();
            string indentation = _generatorWriter.GetIndentation();
            _generatorWriter.IncreaseIndentationLevel();
            return $"{parentResult}{indentation}{GetMethodHeader(Symbol)}\r\n{indentation}{{\r\n";
        }

        private static string GetMethodHeader(IMethodSymbol method)
        {
            string result = SyntaxFacts.GetText(method.DeclaredAccessibility);

            if (method.IsStatic)
            {
                appendToResult("static");
            }

            if (method.IsExtern)
            {
                appendToResult("extern");
            }

            if (method.IsVirtual)
            {
                appendToResult("virtual");
            }

            if (method.IsAbstract)
            {
                appendToResult("abstract");
            }

            if (method.IsSealed)
            {
                appendToResult("sealed");
            }

            if (method.IsOverride)
            {
                appendToResult("override");
            }

            if (method.IsReadOnly)
            {
                appendToResult("readonly");
            }

            if (method.IsAsync)
            {
                appendToResult("async");
            }

            if (method.IsPartialDefinition)
            {
                appendToResult("partial");
            }

            appendToResult(method.Name);
            appendToResult("()");
            return result;

            void appendToResult(string text)
            {
                if (result.Length > 0)
                {
                    result += " " + text;
                }
                else
                {
                    result = text;
                }
            }
        }
    }
}
