using System;
using System.Reflection;

namespace JuliadotNET
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class JModuleAttribute : Attribute {
        internal static readonly ConstructorInfo DEF_CON = typeof(JModuleAttribute).GetConstructor(Type.EmptyTypes);
        public JModuleAttribute(){}
    }
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class JStructAttribute : Attribute {
        internal static readonly ConstructorInfo DEF_CON = typeof(JStructAttribute).GetConstructor(Type.EmptyTypes);
        public JStructAttribute(){}
    }
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class JMutableStructAttribute : Attribute {
        internal static readonly ConstructorInfo DEF_CON = typeof(JMutableStructAttribute).GetConstructor(Type.EmptyTypes);
        public JMutableStructAttribute(){}
    }
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class JAbstractTypeAttribute : Attribute {
        internal static readonly ConstructorInfo DEF_CON = typeof(JAbstractTypeAttribute).GetConstructor(Type.EmptyTypes);
        public JAbstractTypeAttribute(){}
    }
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class JPrimitiveAttribute : Attribute {
        internal static readonly ConstructorInfo DEF_CON = typeof(JPrimitiveAttribute).GetConstructor(Type.EmptyTypes);
        public JPrimitiveAttribute(){}
    }
}