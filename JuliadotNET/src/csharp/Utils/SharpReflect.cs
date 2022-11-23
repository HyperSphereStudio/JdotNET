using System;
using System.Linq.Expressions;
using System.Reflection;

namespace runtime.core
{
    public class EmptyClass{private EmptyClass(){}}
    
    public class SharpReflect
    {
        public readonly static BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                             BindingFlags.NonPublic;
        
        public static MethodInfo GetMethod<T>(string methodName, params Type[] types) => typeof(T).GetMethod(methodName, FLAGS, null, types, null);
        public static MethodInfo GetMethod<T, T1>(string methodName) => GetMethod<T>(methodName, typeof(T1));
        public static MethodInfo GetMethod<T, T1, T2>(string methodName) => GetMethod<T>(methodName, typeof(T1), typeof(T2));
        public static MethodInfo GetMethod<T, T1, T2, T3>(string methodName) => GetMethod<T>(methodName, typeof(T1), typeof(T2), typeof(T3));
        public static MethodInfo GetMethod<T, T1, T2, T3, T4>(string methodName) => GetMethod<T>(methodName, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        public static MethodInfo GetMethod<T, T1, T2, T3, T4, T5>(string methodName) => GetMethod<T>(methodName, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        public static MethodInfo GetMethod(Type t, string methodName, params Type[] types) => t.GetMethod(methodName, FLAGS, null, types, null);
         
        
        public static ConstructorInfo GetConstructor<T>(params Type[] types) => typeof(T).GetConstructor(FLAGS, null, types, null);
        public static ConstructorInfo GetConstructor(Type t, params Type[] types) => t.GetConstructor(FLAGS, null, types, null);
        public static ConstructorInfo GetConstructor<T, T1>() => GetConstructor<T>(typeof(T1));
        public static ConstructorInfo GetConstructor<T, T1, T2>() => GetConstructor<T>( typeof(T1), typeof(T2));
        public static ConstructorInfo GetConstructor<T, T1, T2, T3>() => GetConstructor<T>( typeof(T1), typeof(T2), typeof(T3));
        public static ConstructorInfo GetConstructor<T, T1, T2, T3, T4>() => GetConstructor<T>( typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        public static ConstructorInfo GetConstructor<T, T1, T2, T3, T4, T5>() => GetConstructor<T>( typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));


        public static FieldInfo GetField<T>(string name) => typeof(T).GetField(name, FLAGS);
        public static FieldInfo GetField(Type t, string name) => t.GetField(name, FLAGS);

    }

}