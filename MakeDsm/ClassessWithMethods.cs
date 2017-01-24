using System.Collections.Generic;
using System.Linq;

namespace MakeDsm
{
    public class ClassessWithMethods
    {
        public readonly List<ClassWithMethods> classessWithMethods;

        public ClassessWithMethods(List<ClassWithMethods> classessWithMethods)
        {
            this.classessWithMethods = classessWithMethods;
        }

        public ClassWithMethods GetClassMethods(string classname)
        {
            var ret =  this.classessWithMethods.FirstOrDefault(cm => cm.ClassName == classname);
            return ret;
        }


    }
}