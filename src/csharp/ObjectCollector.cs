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
        internal static List<object> Ccollector = new List<object>();

        internal static void init() => JLcollector = new JLArray(JLType.JLAny, 0);

        internal static JLGCStub PushJL(JLVal val){
            lock (JLLock){
                JLcollector.Add(val);
                return new JLGCStub((int) JLcollector.Length);
            }
        }

        internal static int PushCSharp(object val)
        {
            lock (CLock)
            {
                Ccollector.Add(val);
                return Ccollector.Count - 1;
            }
        }

        public static long JLObjLen { get => JLcollector.Length; }
        public static long CSharpObjLen { get => Ccollector.Count; }
    }

    public class JLGCStub{
        internal int idx;
        internal bool wasFreed = false;

        public bool Freed { get => wasFreed; internal set => wasFreed = value; }

        internal JLGCStub(int idx) => this.idx = idx;

        ~JLGCStub() => Free();

        public JLVal Value { get => ObjectCollector.JLcollector[idx]; set => ObjectCollector.JLcollector[idx] = value; }

        public void Free()
        {
            if (!wasFreed)
            {
                wasFreed = true;
                ObjectCollector.JLcollector.RemoveAt(idx);
            }
        }

    }
}
