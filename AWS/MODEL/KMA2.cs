using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace AWS.MODEL
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public class KMA2
    {
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

        public byte DataDivision;	  // M : 순간 자료,  O : 최근 1 분,  A : 과거자료, X : 해당날짜의 자료가 없음을 알림.
        public byte DataNo;

        public ushort LoggerID;		  // LOGGER ID NUMBER (0 ~ 32767 BINARY)

        public ushort Sensor_0_Datas; //1분 평균 기온    
        public ushort Sensor_1_Datas; //1분 평균 풍향      
        public ushort Sensor_2_Datas; //1분 평균 풍속     
        public ushort Sensor_3_Datas; //1분 돌풍 풍향       
        public ushort Sensor_4_Datas; //1분 돌풍 풍속       
        public ushort Sensor_5_Datas; //00시부터 현재까지 누적강수량       
        public ushort Sensor_6_Datas; //1분 평균 현지기압       
        public ushort Sensor_7_Datas; //1분 강수유무
        public ushort Sensor_8_Datas; //10분 누적적설
        public ushort Sensor_9_Datas; //1분 평균 상대습도
        public ushort Sensor_10_Datas; //00시부터 현재까지 누적강수량

        public ushort Spare_Sensor_0_Datas;
        public ushort Spare_Sensor_1_Datas;
        public ushort Spare_Sensor_2_Datas;
        public ushort Spare_Sensor_3_Datas;
        public ushort Spare_Sensor_4_Datas;
        public ushort Spare_Sensor_5_Datas;
        public ushort Spare_Sensor_6_Datas;
        public ushort Spare_Sensor_7_Datas;
        public ushort Spare_Sensor_8_Datas;
        public ushort Spare_Sensor_9_Datas;

        public ushort Sensor0_0_Datas;
        public ushort Sensor1_1_Datas; //일조
        public ushort Sensor2_2_Datas;
        public ushort Sensor3_3_Datas;
        public ushort Sensor4_4_Datas;
        public ushort Sensor5_5_Datas;
        public ushort Sensor6_6_Datas;
        public ushort Sensor7_7_Datas;
        public ushort Sensor8_8_Datas;
        public ushort Sensor9_9_Datas;
        public ushort Sensor10_10_Datas;
        public ushort Sensor11_11_Datas;
        public ushort Sensor12_12_Datas;

        public ushort Spare0_Sensor_0_Datas; 
        public ushort Spare1_Sensor_1_Datas; //시정
        public ushort Spare2_Sensor_2_Datas; //현천
        public ushort Spare3_Sensor_3_Datas;
        public ushort Spare4_Sensor_4_Datas;
        public ushort Spare5_Sensor_5_Datas;
        public ushort Spare6_Sensor_6_Datas;
        public ushort Spare7_Sensor_7_Datas;
        public ushort Spare8_Sensor_8_Datas;
        public ushort Spare9_Sensor_9_Datas;

        public byte Voltage;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] SensorStatus1;

        public byte Xor;				// 2 - 11항에 대한 XOR 
        public byte Add;				// 2 - 11항에 대한 ADD    
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Tail;				// 0xFF, 0xFE


        /// <summary>
        /// 생성자
        /// </summary>
        public KMA2()
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

            DataDivision = 0xCC;	  // M : 순간 자료,  O : 최근 1 분,  A : 과거자료, X : 해당날짜의 자료가 없음을 알림.
            DataNo = 0xCC;

            LoggerID = new ushort();		  // LOGGER ID NUMBER (0 ~ 32767 BINARY)

            Sensor_0_Datas = new ushort(); //1분 평균 기온               
            Sensor_1_Datas = new ushort(); //1분 평균 풍향      
            Sensor_2_Datas = new ushort(); //1분 평균 풍속     
            Sensor_3_Datas = new ushort(); //1분 돌풍 풍향       
            Sensor_4_Datas = new ushort(); //1분 돌풍 풍속       
            Sensor_5_Datas = new ushort(); //00시부터 현재까지 누적강수량       
            Sensor_6_Datas = new ushort(); //1분 평균 현지기압       
            Sensor_7_Datas = new ushort(); //1분 강수유무
            Sensor_8_Datas = new ushort(); //10분 누적적설
            Sensor_9_Datas = new ushort(); //1분 평균 상대습도
            Sensor_10_Datas = new ushort(); //00시부터 현재까지 누적강수량


            Sensor_0_Datas = 0;
            Sensor_1_Datas = 0;
            Sensor_2_Datas = 0;
            Sensor_3_Datas = 0;
            Sensor_4_Datas = 0;
            Sensor_5_Datas = 0;
            Sensor_6_Datas = 0;
            Sensor_7_Datas = 0;
            Sensor_8_Datas = 0;
            Sensor_9_Datas = 0;
            Sensor_10_Datas = 0;

            Spare_Sensor_0_Datas = new ushort();
            Spare_Sensor_1_Datas = new ushort();
            Spare_Sensor_2_Datas = new ushort();
            Spare_Sensor_3_Datas = new ushort();
            Spare_Sensor_4_Datas = new ushort();
            Spare_Sensor_5_Datas = new ushort();
            Spare_Sensor_6_Datas = new ushort();
            Spare_Sensor_7_Datas = new ushort();
            Spare_Sensor_8_Datas = new ushort();
            Spare_Sensor_9_Datas = new ushort();

            Spare_Sensor_0_Datas = 0;
            Spare_Sensor_1_Datas = 0;
            Spare_Sensor_2_Datas = 0;
            Spare_Sensor_3_Datas = 0;
            Spare_Sensor_4_Datas = 0;
            Spare_Sensor_5_Datas = 0;
            Spare_Sensor_6_Datas = 0;
            Spare_Sensor_7_Datas = 0;
            Spare_Sensor_8_Datas = 0;
            Spare_Sensor_9_Datas = 0;

            Sensor0_0_Datas = new ushort();
            Sensor1_1_Datas = new ushort();
            Sensor2_2_Datas = new ushort();
            Sensor3_3_Datas = new ushort();
            Sensor4_4_Datas = new ushort();
            Sensor5_5_Datas = new ushort();
            Sensor6_6_Datas = new ushort();
            Sensor7_7_Datas = new ushort();
            Sensor8_8_Datas = new ushort();
            Sensor9_9_Datas = new ushort();
            Sensor10_10_Datas = new ushort();
            Sensor11_11_Datas = new ushort();
            Sensor12_12_Datas = new ushort();

            Sensor0_0_Datas = 0;
            Sensor1_1_Datas = 0;
            Sensor2_2_Datas = 0;
            Sensor3_3_Datas = 0;
            Sensor4_4_Datas = 0;
            Sensor5_5_Datas = 0;
            Sensor6_6_Datas = 0;
            Sensor7_7_Datas = 0;
            Sensor8_8_Datas = 0;
            Sensor9_9_Datas = 0;
            Sensor10_10_Datas = 0;
            Sensor11_11_Datas = 0;
            Sensor12_12_Datas = 0;

            Spare0_Sensor_0_Datas = new ushort();
            Spare1_Sensor_1_Datas = new ushort();
            Spare2_Sensor_2_Datas = new ushort();
            Spare3_Sensor_3_Datas = new ushort();
            Spare4_Sensor_4_Datas = new ushort();
            Spare5_Sensor_5_Datas = new ushort();
            Spare6_Sensor_6_Datas = new ushort();
            Spare7_Sensor_7_Datas = new ushort();
            Spare8_Sensor_8_Datas = new ushort();
            Spare9_Sensor_9_Datas = new ushort();

            Spare0_Sensor_0_Datas = 0;
            Spare1_Sensor_1_Datas = 0;
            Spare2_Sensor_2_Datas = 0;
            Spare3_Sensor_3_Datas = 0;
            Spare4_Sensor_4_Datas = 0;
            Spare5_Sensor_5_Datas = 0;
            Spare6_Sensor_6_Datas = 0;
            Spare7_Sensor_7_Datas = 0;
            Spare8_Sensor_8_Datas = 0;
            Spare9_Sensor_9_Datas = 0;

            Voltage = 0xCC;
            SensorStatus1 = new byte[2] { 0xCC, 0xCC };

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

        public static KMA2 SetByte(byte[] Buffer)
        {
            if (Marshal.SizeOf(typeof(KMA2)) != Buffer.Length) throw new Exception("버퍼의 사이즈가 맞지 않습니다.");
            IntPtr ptrStruct = IntPtr.Zero;
            try
            {
                ptrStruct = Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(typeof(KMA2)));
                System.Runtime.InteropServices.Marshal.Copy(Buffer, 0, ptrStruct, Buffer.Length);
                return (KMA2)Marshal.PtrToStructure(ptrStruct, typeof(KMA2));
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
