using System;
using System.Collections.Generic;

namespace JULIAdotNET
{

    public class ObjectManager{
        private static object sharp_mem_lock = new object();
        private static List<object> sharp_references = new();
        private static List<int> sharp_freed_references = new();
        private static JLFun _CreateJulia4SharpReferenceF, _FreeJulia4SharpReferenceF, _GetJulia4SharpValueF;

        internal static void init(){
            _CreateJulia4SharpReferenceF = JLModule.Sharp_MemoryManagement.GetFunction("_CreateJulia4SharpReference");
            _FreeJulia4SharpReferenceF = JLModule.Sharp_MemoryManagement.GetFunction("_FreeJulia4SharpReference");
            _GetJulia4SharpValueF = JLModule.Sharp_MemoryManagement.GetFunction("_GetJulia4SharpValue");
        }

        internal static JuliaReference _CreateJulia4SharpReference(JLVal v) => new JuliaReference(v);

        internal static int _CreateSharp4JuliaReference(object o){
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

        internal static void _FreeSharp4JuliaReference(int ptr){
            lock (sharp_mem_lock){
                sharp_references[ptr] = null;

                if (ptr == sharp_references.Count)
                    sharp_references.RemoveAt(ptr);
                else
                    sharp_freed_references.Add(ptr);
            }
        }

        internal static object _GetSharp4JuliaValue(int ptr){
            lock (sharp_mem_lock)
                return sharp_references[ptr];
        }

        public class JuliaReference
        {
            internal int ptr;
            internal bool wasFreed = false;

            public bool Freed { get => wasFreed; internal set => wasFreed = value; }

            internal JuliaReference(JLVal v) => ptr = _CreateJulia4SharpReferenceF.Invoke(v).UnboxInt32();

            ~JuliaReference() => Free();

            public JLVal Value { get => _GetJulia4SharpValueF.Invoke(ptr); }

            public void Free()
            {
                if (!wasFreed){
                    wasFreed = true;
                    if (Julia.IsInitialized)
                        _FreeJulia4SharpReferenceF.Invoke(ptr);
                }
            }
        }
    }


    
}
