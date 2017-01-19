using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using log4net;

namespace AWS.MODEL
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public class KMACommand2
    {
        static ILog iLog = log4net.LogManager.GetLogger("Logger");

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Header;				// 	0xFF, 0xFF
        public byte ProtocolVersionYear;
        public byte ProtocolVersionMonth;
        public byte ProtocolVersionDay;
        public byte Year;			  // 년	1	현재/과거 년 (1980 ~ 2079)
        public byte Month;			  // 월	1	현재/과거 월 (   1 ~   12)
        public byte Day;			  // 일	1	현재/과거 일 (   1 ~   31)
        public byte Hour;			  // 시	1	현재/과거 시 (   0 ~   23)
        public byte Minute;			  // 분	1	현재/과거 분 (   0 ~   59)
        public byte Second;			  // 초	1	현재/과거 초 (   0 ~   59)
        public ushort Password;	  // M : 순간 자료,  O : 최근 1 분,  A : 과거자료, X : 해당날짜의 자료가 없음을 알림.        
        public ushort LoggerID;		  // LOGGER ID NUMBER (0 ~ 32767 BINARY)

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] Command;
        public byte Xor;				// 2 - 11항에 대한 XOR 
        public byte Add;				// 2 - 11항에 대한 ADD    
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Tail;				// 0xFF, 0xFE


        /// <summary>
        /// 생성자
        /// </summary>
        public KMACommand2()
        {
            this.Header = new byte[2] { 0xCC, 0xCC };
            ProtocolVersionYear = 0xCC;
            ProtocolVersionMonth = 0xCC;
            ProtocolVersionDay = 0xCC;
            Year = 0xCC;			  // 년	1	현재/과거 년 (1980 ~ 2079)
            Month = 0xCC;			  // 월	1	현재/과거 월 (   1 ~   12)
            Day = 0xCC;			  // 일	1	현재/과거 일 (   1 ~   31)
            Hour = 0xCC;			  // 시	1	현재/과거 시 (   0 ~   23)
            Minute = 0xCC;			  // 분	1	현재/과거 분 (   0 ~   59)
            Second = 0xCC;			  // 초	1	현재/과거 초 (   0 ~   59)        
            Password = new ushort();
            LoggerID = new ushort();		  // LOGGER ID NUMBER (0 ~ 32767 BINARY)            
            this.Command = new byte[10] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
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

        public static KMACommand2 SetByte(byte[] Buffer)
        {
            if (Marshal.SizeOf(typeof(KMACommand2)) != Buffer.Length) throw new Exception("버퍼의 사이즈가 맞지 않습니다.");
            IntPtr ptrStruct = IntPtr.Zero;

            try
            {
                ptrStruct = Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(typeof(KMACommand2)));
                System.Runtime.InteropServices.Marshal.Copy(Buffer, 0, ptrStruct, Buffer.Length);
                return (KMACommand2)Marshal.PtrToStructure(ptrStruct, typeof(KMACommand2));
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
