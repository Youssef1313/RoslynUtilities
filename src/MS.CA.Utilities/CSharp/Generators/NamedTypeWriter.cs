using System;
using Microsoft.CodeAnalysis;
using MS.CA.Utilities.CSharp.Extensions;
using MS.CA.Utilities.Generators;

namespace MS.CA.Utilities.CSharp.Generators
{
    internal sealed class NamedTypeWriter : SymbolWriter<INamedTypeSymbol>
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

        public NamedTypeWriter(IGeneratorWriter generatorWriter, INamedTypeSymbol symbol, bool includeContainingSymbol) : base(generatorWriter, symbol)
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
            // TODO: Generic classes, interfaces with variance, prefix keyword-like named types with @
            string? parentResult = _parentWriter?.GetStartText();
            string indentation = _generatorWriter.GetIndentation();
            _generatorWriter.IncreaseIndentationLevel();
            return $"{parentResult}{indentation}{GetNamedTypeHeader(Symbol)}\r\n{indentation}{{\r\n";
        }

        private static string GetNamedTypeHeader(INamedTypeSymbol namedType)
        {
            if (namedType.TypeKind is not (TypeKind.Class or TypeKind.Struct or TypeKind.Interface))
            {
                throw new ArgumentException("Only classes and structs are the supported named types.");
            }

            bool isRecord = IsRecord(namedType);
            string typeKind = (namedType.TypeKind, isRecord) switch
            {
                (TypeKind.Class, true) => "record",
                (TypeKind.Class, false) => "class",
                (TypeKind.Struct, true) => "record struct",
                (TypeKind.Struct, false) => "struct",
                (TypeKind.Interface, false) => "interface",
                (TypeKind.Interface, true) => throw new InvalidOperationException("TypeKind.Interface cannot be a record."),
                _ => throw new InvalidOperationException($"Unexpected (TypeKind, IsRecord) pair ('{namedType.TypeKind}', '{isRecord}')."),
            };


            return $"partial {typeKind} {namedType.ToDisplayString(s_format)}";
        }

        private static bool IsRecord(INamedTypeSymbol namedType)
        {
#if CODEANALYSIS_3_9_OR_GREATER && false
            return namedType.IsRecord;
#else
            // TODO: Fallback to Lightup-based (reflection) IsRecord check?
            // This will be done besides this check, not alone.
            // This is because records were introduced in 3.7, but IsRecord API introduced in 3.9.
            // Since there is no Lightup (yet) in this project, see:
            // https://github.com/dotnet/roslyn-analyzers/tree/7caefcebccfa2563ae4f316bd7d350094da4e780/src/Utilities/Compiler/Lightup
            return (namedType.DeclaringSyntaxReferences.Length > 0 && namedType.DeclaringSyntaxReferences[0].GetSyntax().IsKind(SyntaxKindEx.RecordDeclaration, SyntaxKindEx.RecordStructDeclaration)) ||
                !namedType.GetMembers("<Clone>$").IsEmpty;
#endif
        }
    }
}
