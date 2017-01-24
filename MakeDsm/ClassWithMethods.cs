using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using System;
using Microsoft.CodeAnalysis;

namespace MakeDsm
{
    public class ClassWithMethods: WrapperWithClass
    {
        static readonly string PUBLIC_TOKEN_TEXT;
        private ReadOnlyCollection<MemberDeclarationSyntax> _methodsAndProperties;

        private ReadOnlyCollection<MethodDeclarationSyntax> _methods { get { return this._methodsAndProperties.OfType<MethodDeclarationSyntax>().ToList().AsReadOnly(); } }
        private ReadOnlyCollection<PropertyDeclarationSyntax> _properties { get { return this._methodsAndProperties.OfType<PropertyDeclarationSyntax>().ToList().AsReadOnly(); } }

        public ReadOnlyCollection<MethodDeclarationSyntax> PublicMethods { get { return this._methods.Where(m=> _isModifiersHasPublic(m.Modifiers)).ToList().AsReadOnly(); } }
        public ReadOnlyCollection<PropertyDeclarationSyntax> PublicProperties { get { return this._properties.Where(m=> _isModifiersHasPublic(m.Modifiers)).ToList().AsReadOnly(); } }

        Predicate<SyntaxTokenList> _isModifiersHasPublic = (mod) => mod.Any(m => m.Text == PUBLIC_TOKEN_TEXT);

        static ClassWithMethods()
        {
            var publicModifier = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
            PUBLIC_TOKEN_TEXT = publicModifier.Text;/*public*/
        }
        public ClassWithMethods(ClassDeclarationSyntax @class, List<MemberDeclarationSyntax> methodsAndProperties)
            :base(@class)
        {

            this._methodsAndProperties = methodsAndProperties.AsReadOnly();
            
        }

        public List<string> GetPublicMemberNames()
        {
            var mNames = this.PublicMethods.Select(m => m.Identifier.ValueText);
            var pNames = this.PublicProperties.Select(p => p.Identifier.ValueText);
            //pNames = new List<string>(); //checking if would be better with no properties

            var ret = mNames.Union(pNames).Distinct().ToList(); 
            return ret;
        }

        

    }
}