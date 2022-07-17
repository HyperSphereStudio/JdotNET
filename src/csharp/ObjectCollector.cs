using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuliaInterface
{
    public class ObjectCollector
    {
        internal static object JLLock = new object(), CLock = new object();
        internal static JLArray JLcollector;
        internal static HashSet<object> Ccollector = new HashSet<object>();
        internal static void init() => JLcollector = Julia.Eval("JuliaReferenceCollector = Set{Any}()");

        internal static JLGCStub PushJL(JLVal val){
            lock (JLLock){
                JLcollector.Add(val);
                return new JLGCStub(val.ptr);
            }
        }

        internal static long PushCSharp(object val)
        {
            lock (CLock)
            {
                Ccollector.Add(val);
                return AddressHelper.GetAddress(val).ToInt64();
            }
        }

        public static long JLObjLen { get => JLcollector.Length; }
        public static long CSharpObjLen { get => Ccollector.Count; }

        internal static void Free(){
            JLcollector.Clear();
            Ccollector.Clear();
        }
    }

    public class JLGCStub{
        internal IntPtr val;
        internal bool wasFreed = false;

        public bool Freed { get => wasFreed; internal set => wasFreed = value; }

        internal JLGCStub(IntPtr val) => this.val = val;

        ~JLGCStub() => Free();

        public JLVal Value { get => val; }

        public void Free()
        {
            if (!wasFreed){
                wasFreed = true;
                if(Julia.IsInitialized)
                    ObjectCollector.JLcollector.Remove(val);
            }
        }

    }
}
