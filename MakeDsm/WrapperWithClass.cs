using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MakeDsm
{
    public abstract class WrapperWithClass
    {
        public string ClassName { get { return this.ClassDeclarationSyntax.GetClassWithNameSpace(); } }
        internal ClassDeclarationSyntax ClassDeclarationSyntax { get; }

        public WrapperWithClass(ClassDeclarationSyntax @class)
        {
            this.ClassDeclarationSyntax = @class;
        }
    }
}