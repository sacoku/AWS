using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace AWS.MODEL
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public class Reseponse
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Answer;

        public Reseponse()
        {
            Answer = new byte[4] { 0xCC, 0xCC, 0xCC, 0xCC };
        }

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

        public static Reseponse SetByte(byte[] Buffer)
        {
            if (Marshal.SizeOf(typeof(Reseponse)) != Buffer.Length) throw new Exception("버퍼의 사이즈가 맞지 않습니다.");
            IntPtr ptrStruct = IntPtr.Zero;

            try
            {
                ptrStruct = Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(typeof(Reseponse)));
                System.Runtime.InteropServices.Marshal.Copy(Buffer, 0, ptrStruct, Buffer.Length);
                return (Reseponse)Marshal.PtrToStructure(ptrStruct, typeof(Reseponse));
            }
            catch (Exception E)
            {	
                throw E;
            }
            finally
            {
                if (ptrStruct != IntPtr.Zero)
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(ptrStruct);
            }
        }

    }
}
