using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace AWS.MODEL
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)] 
    public class KMALoggerObservation
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Date;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Time;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] SensorStatus;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] BoardStatus;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] CommandStatus;

        public KMALoggerObservation()
        {
            this.Date = new byte[2] { 0xCC, 0xCC };
            this.Time = new byte[2] { 0xCC, 0xCC };
            this.SensorStatus = new byte[2] { 0xCC, 0xCC };
            this.BoardStatus = new byte[2] { 0xCC, 0xCC };
            this.CommandStatus = new byte[2] { 0xCC, 0xCC };
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

        public static KMALoggerObservation SetByte(byte[] Buffer)
        {
            if (Marshal.SizeOf(typeof(KMALoggerObservation)) != Buffer.Length) throw new Exception("버퍼의 사이즈가 맞지 않습니다.");
            IntPtr ptrStruct = IntPtr.Zero;

            try
            {
                ptrStruct = Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(typeof(KMALoggerObservation)));
                System.Runtime.InteropServices.Marshal.Copy(Buffer, 0, ptrStruct, Buffer.Length);
                return (KMALoggerObservation)Marshal.PtrToStructure(ptrStruct, typeof(KMALoggerObservation));
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
