using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static JULIAdotNET.ObjectManager;

namespace JULIAdotNET
{
    public class SharpInputStream : TextWriter
    {
        private static MethodInfo write_method = typeof(SharpInputStream).GetMethod("Write", new Type[] { typeof(string) });
        private static MethodInfo close_method = typeof(SharpInputStream).GetMethod("CloseStream", new Type[] {typeof(bool)});
        private JuliaReference stream;
        private bool wasClosed = false;
        public override Encoding Encoding => Encoding.UTF8;

        public SharpInputStream()
        {
            var write = NativeSharp.GetSharpMethodInstance(write_method, this);
            var close = NativeSharp.GetSharpMethodInstance(close_method, this);
            stream = JLModule.JULIAdotNET.GetType("SharpOutputStream").Create(write, close).Reference();
        }

        ~SharpInputStream() => CloseStream(true);

        public override void Write(string value) => stream.Value.Write(value);

        public override void Close()
        {
            if (!wasClosed)
            {
                base.Close();
                stream = null;
                wasClosed = true;
            }
        }

        public bool CloseStream(bool actually_close)
        {
            if (actually_close)
                Close();
            return wasClosed;
        }
    }

    public class SharpOutputStream : StreamReader{
        private static MethodInfo read_method = typeof(SharpOutputStream).GetMethod("ReadLine");
        private static MethodInfo close_method = typeof(SharpOutputStream).GetMethod("CloseStream");
        private object write_lock;
        private bool wasClosed = false;
        private JuliaReference stream;

        public SharpOutputStream() : base(new MemoryStream())
        {
            var read = NativeSharp.GetSharpMethodInstance(read_method, this);
            var close = NativeSharp.GetSharpMethodInstance(close_method, this);
            stream = JLModule.JULIAdotNET.GetType("SharpInputStream").Create(read, close).Reference();

        }

        ~SharpOutputStream() => CloseStream(true);

        private unsafe void WriteToInternalStream(byte* bytes, int len)
        {
       
        }

        public override int Read()
        {
            lock (write_lock) return base.Read();
        }

        public override int Read([In][Out] char[] buffer, int index, int count)
        {
            lock (write_lock) return base.Read(buffer, index, count);
        }

        public override int ReadBlock([In][Out] char[] buffer, int index, int count)
        {
            lock (write_lock) return base.ReadBlock(buffer, index, count);
        }

        public override void Close()
        {
            if (!wasClosed)
            {
                wasClosed = true;
                stream = null;
                base.Close();
            }
        }

        public bool CloseStream(bool actually_close)
        {
            if (actually_close && !wasClosed)
                Close();
            return wasClosed;
        }
    }
}
