using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using log4net;

namespace AWS.MODEL
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public class CommandProtocol
    {
        // [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        static ILog iLog = log4net.LogManager.GetLogger("Logger");

        public byte[] GetByte()
        {
            byte[] Data = new byte[Marshal.SizeOf(this)];
            IntPtr ptrStruct = IntPtr.Zero;
            try
            {
                ptrStruct = Marshal.AllocHGlobal(Marshal.SizeOf(this));
                Marshal.StructureToPtr(this, ptrStruct, true);
                System.Runtime.InteropServices.Marshal.Copy(ptrStruct, Data, 0, Marshal.SizeOf(this));
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
            }
            finally
            {
                if (ptrStruct != IntPtr.Zero)
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(ptrStruct);
            }
            return Data;
        }

        public static CommandProtocol SetByte(byte[] Buffer)
        {
            if (Marshal.SizeOf(typeof(CommandProtocol)) != Buffer.Length) throw new Exception("버퍼의 사이즈가 맞지 않습니다.");
            IntPtr ptrStruct = IntPtr.Zero;

            try
            {
                ptrStruct = Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(typeof(CommandProtocol)));
                System.Runtime.InteropServices.Marshal.Copy(Buffer, 0, ptrStruct, Buffer.Length);
                return (CommandProtocol)Marshal.PtrToStructure(ptrStruct, typeof(CommandProtocol));
            }
            catch (Exception E)
            {
                iLog.Debug(E.Message);
                return null;
            }
            finally
            {
                if (ptrStruct != IntPtr.Zero)
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(ptrStruct);
            }
        }


    }
}
