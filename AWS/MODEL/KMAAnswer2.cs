using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace AWS.MODEL
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public class KMAAnswer2
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Header;				// 	0xFF, 0xFF

        public byte ProtocolVersionYear;
        public byte ProtocolVersionMonth;
        public byte ProtocolVersionDay;    
        public ushort LoggerID;		  // LOGGER ID NUMBER (0 ~ 32767 BINARY)
        public byte Type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Answer;
        public byte Xor;				// 2 - 11항에 대한 XOR 
        public byte Add;				// 2 - 11항에 대한 ADD    
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Tail;				// 0xFF, 0xFE

        public KMAAnswer2()
        {
            this.Header = new byte[2] { 0xCC, 0xCC };
            ProtocolVersionYear = 0xCC;
            ProtocolVersionMonth = 0xCC;
            ProtocolVersionDay = 0xCC;
            LoggerID = new ushort();		  // LOGGER ID NUMBER (0 ~ 32767 BINARY)

            Answer = new byte[4] { 0xCC, 0xCC, 0xCC, 0xCC };

            this.Type = 0xCC;

            Xor = 0xCC;				// 2 - 11항에 대한 XOR 
            Add = 0xCC;				// 2 - 11항에 대한 ADD    

            this.Tail = new byte[2] { 0xCC, 0xCC };
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

        public static KMAAnswer2 SetByte(byte[] Buffer)
        {
            if (Marshal.SizeOf(typeof(KMAAnswer2)) != Buffer.Length) throw new Exception("버퍼의 사이즈가 맞지 않습니다.");
            IntPtr ptrStruct = IntPtr.Zero;
            try
            {
                ptrStruct = Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(typeof(KMAAnswer2)));
                System.Runtime.InteropServices.Marshal.Copy(Buffer, 0, ptrStruct, Buffer.Length);
                return (KMAAnswer2)Marshal.PtrToStructure(ptrStruct, typeof(KMAAnswer2));
            }
            catch (Exception E)
            {
                Debug.WriteLine(E.Message);
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
