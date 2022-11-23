using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Base;

namespace JULIAdotNET.Dynamics
{
    public class DyAny : IDynamicMetaObjectProvider {
        internal readonly Any Ptr;
        public DyAny(Any ptr) => Ptr = ptr;
        public DynamicMetaObject GetMetaObject(Expression parameter) => new JuliaDynamic(parameter, this);

        #region Conversions
        public static explicit operator long(DyAny value) => (long) value.Ptr;
        public static explicit operator ulong(DyAny value) => (ulong) value.Ptr;
        public static explicit operator int(DyAny value) => (int) value.Ptr;
        public static explicit operator uint(DyAny value) => (uint) value.Ptr;
        public static explicit operator short(DyAny value) => (short) value.Ptr;
        public static explicit operator ushort(DyAny value) => (ushort) value.Ptr;
        public static explicit operator byte(DyAny value) => (byte) value.Ptr;
        public static explicit operator sbyte(DyAny value) => (sbyte) value.Ptr;
        public static explicit operator string(DyAny value) => (string) value.Ptr;
        public static explicit operator char(DyAny value) => (char) value.Ptr;
        public static explicit operator bool(DyAny value) => (bool) value.Ptr;
        public static explicit operator double(DyAny value) => (double) value.Ptr;
        public static explicit operator float(DyAny value) => (float) value.Ptr;
        public static implicit operator Any(DyAny value) => value.Ptr;
        #endregion
        
        public override string ToString() => Ptr.ToString();
        public override int GetHashCode() => Ptr.GetHashCode();
    }
    
    internal class JuliaDynamic : DynamicMetaObject {
        private static readonly MethodInfo FuncInvokeMi = typeof(Any).GetMethod("Invoke", new[]{typeof(DyAny), typeof(Any)});
        private static readonly ConstructorInfo JuliaDyVci = typeof(DyAny).GetConstructor(new[]{typeof(Any)});
            
        public JuliaDynamic(Expression expression, DyAny any) : base(expression, BindingRestrictions.Empty, any){}
            
        public override DynamicMetaObject BindGetMember(GetMemberBinder binder) {
            var expr = 
                
                Expression.New(JuliaDyVci,
                Expression.Call(Expression.Constant(JPrimitive.GetPropertyF), FuncInvokeMi,
                                Expression.Convert(Expression, typeof(DyAny)),
                                Expression.Constant(JuliaCalls.jl_symbol(binder.Name))));
            
            var restr = BindingRestrictions.GetTypeRestriction(Expression, typeof(DyAny));
            return new DynamicMetaObject(expr, restr);
        }
        
        
    }
}