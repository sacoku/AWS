using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace AWS.MODEL
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public class LoggerVersionInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Header;				// 	0xFF, 0xFF
        public byte ProtocolVersionYear;
        public byte ProtocolVersionMonth;
        public byte ProtocolVersionDay;      
        public ushort LoggerID;		  // LOGGER ID NUMBER (0 ~ 32767 BINARY)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
        public byte[] content;

        public byte Xor;				// 2 - 11항에 대한 XOR 
        public byte Add;				// 2 - 11항에 대한 ADD    
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Tail;				// 0xFF, 0xFE

         /// <summary>
        /// 생성자
        /// </summary>
        public LoggerVersionInfo()
        {
            this.Header = new byte[2] { 0xCC, 0xCC };
            ProtocolVersionYear = 0xCC;
            ProtocolVersionMonth = 0xCC;
            ProtocolVersionDay = 0xCC;            
            LoggerID = new ushort();		  // LOGGER ID NUMBER (0 ~ 32767 BINARY)            
            this.content = new byte[18] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
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

        public static LoggerVersionInfo SetByte(byte[] Buffer)
        {
            if (Marshal.SizeOf(typeof(LoggerVersionInfo)) != Buffer.Length) throw new Exception("버퍼의 사이즈가 맞지 않습니다.");
            IntPtr ptrStruct = IntPtr.Zero;

            try
            {
                ptrStruct = Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(typeof(LoggerVersionInfo)));
                System.Runtime.InteropServices.Marshal.Copy(Buffer, 0, ptrStruct, Buffer.Length);
                return (LoggerVersionInfo)Marshal.PtrToStructure(ptrStruct, typeof(LoggerVersionInfo));
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
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
