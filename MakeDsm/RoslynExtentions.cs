using MakeDsm;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
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

