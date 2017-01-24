using MakeDsm;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class RoslynExtentions
{

    public static string GetClassWithNameSpace(this TypeDeclarationSyntax sn)
    {
        string ret = sn.Identifier.ToString();
        NamespaceDeclarationSyntax namespaceDeclarationSyntax = null;
        if (SyntaxNodeHelper.TryGetParentSyntax(sn, out namespaceDeclarationSyntax))
        {
            var namespaceName = namespaceDeclarationSyntax.Name.ToString();
            var fullClassName = namespaceName + "." + ret;
            ret = fullClassName;
        }

        return ret;
    }

    public static ImmutableArray<T> GetAccessibleMembersInThisAndBaseTypes<T>(this ITypeSymbol containingType, ISymbol within) where T : class, ISymbol
    {
        if (containingType == null)
        {
            return ImmutableArray<T>.Empty;
        }

        var types = containingType.GetBaseTypesAndThis();
        return types.SelectMany(x => x.GetMembers().OfType<T>().Where(m => m.IsAccessibleWithin(within)))
                    .ToImmutableArray();
    }

    public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol type)
    {
        var current = type;
        while (current != null)
        {
            yield return current;
            current = current.BaseType;
        }
    }

    public static bool IsAccessibleWithin(
           this ISymbol symbol,
           ISymbol within,
           ITypeSymbol throughTypeOpt = null)
    {
        if (within is IAssemblySymbol)
        {
            return symbol.IsAccessibleWithin((IAssemblySymbol)within, throughTypeOpt);
        }
        else if (within is INamedTypeSymbol)
        {
            return symbol.IsAccessibleWithin((INamedTypeSymbol)within, throughTypeOpt);
        }
        else
        {
            throw new ArgumentException();
        }
    }

    ///// <summary>
    ///// Finds all references.
    ///// </summary>
    ///// <param name="pNode">The node.</param>
    ///// <param name="pSemModel">The semantic model.</param>
    ///// <param name="pSolution">The solution.</param>
    ///// <returns></returns>
    ///// <author>
    ///// Harald Binkle
    ///// </author>
    //public static IEnumerable<CommonSyntaxNode> FindAllReferences(this SyntaxNode pNode, ISemanticModel pSemModel, ISolution pSolution)
    //{
    //    var sym = pSemModel.GetDeclaredSymbol(pNode);
    //    var syms = Enumerable.Repeat(sym, 1);

    //    return FindNodesForSymbols(pSolution, syms);
    //}

    ///// <summary>
    ///// Finds the nodes for symbols.
    ///// </summary>
    ///// <param name="pSolution">The solution.</param>
    ///// <param name="syms">The symbols.</param>
    ///// <returns></returns>
    ///// <author>Harald Binkle</author>
    //private static IEnumerable<CommonSyntaxNode> FindNodesForSymbols(ISolution pSolution, IEnumerable<ISymbol> pSymbols)
    //{
    //    return from symbol in pSymbols
    //           from reference in symbol.FindReferences(pSolution)
    //           from loc in reference.Locations
    //           let position = loc.Location.SourceSpan.Start
    //           select loc.Location.SourceTree.GetRoot().FindToken(position) into token
    //           select token.Parent;
    //}
}

