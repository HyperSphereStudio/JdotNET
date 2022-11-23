using System;
using System.Collections.Generic;

namespace JULIAdotNET
{

    public class ObjectManager{
        private static object sharp_mem_lock = new object();
        private static List<object> sharp_references = new List<object>();
        private static List<int> sharp_freed_references = new List<int>();
        private static JLFun _MallocJuliaObject, _FreeJuliaObject, _DereferenceJuliaObject;
        internal static JLFun _CreateSafeJuliaSharpReference;

        internal static void init(){
            _MallocJuliaObject = JLModule.Sharp_MemoryManagement.GetFunction("_MallocJuliaObject");
            _FreeJuliaObject = JLModule.Sharp_MemoryManagement.GetFunction("_FreeJuliaObject");
            _DereferenceJuliaObject = JLModule.Sharp_MemoryManagement.GetFunction("_DereferenceJuliaObject");

            _CreateSafeJuliaSharpReference = JLModule.Sharp_Native.GetFunction("sharpref");
        }

        
        internal static int _MallocSharpObject(object o){
            lock (sharp_mem_lock){
                if(sharp_freed_references.Count != 0){
                    var idx = sharp_freed_references.Count - 1;
                    sharp_freed_references.RemoveAt(idx);
                    sharp_references[idx] = o;
                    return idx;
                }else{
                    int idx = sharp_references.Count;
                    sharp_references.Add(o);
                    return idx;
                }
            }
        }

        internal static void _FreeSharpObject(int ptr){
            lock (sharp_mem_lock){
                sharp_references[ptr] = null;
                if (ptr == sharp_references.Count)
                    sharp_references.RemoveAt(ptr);
                else
                    sharp_freed_references.Add(ptr);
            }
        }

        internal static object _DereferenceSharpObject(int ptr){
            lock (sharp_mem_lock)
                return sharp_references[ptr];
        }

    }
}
