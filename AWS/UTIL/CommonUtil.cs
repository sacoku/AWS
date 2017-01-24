using AWS.Config;
using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AWS.UTIL
{
    delegate void LogWrite(object msg);

    class CommonUtil
    {
        static ILog iLog = log4net.LogManager.GetLogger("Logger");

        public static ushort ByteChange(ushort data)
        {
            byte[] Temp = System.BitConverter.GetBytes(data);
            byte[] Temp2 = new byte[2];
            Temp2[0] = Temp[1];
            Temp2[1] = Temp[0];

            return System.BitConverter.ToUInt16(Temp2, 0);
        }

        public static T Clone<T>(T source)
        {
            T cloned = (T)Activator.CreateInstance(source.GetType());

            foreach (PropertyInfo curPropInfo in source.GetType().GetProperties())
            {
                if (curPropInfo.Name == "Parent" || curPropInfo.Name == "Name")
                    continue;

                if (curPropInfo.GetGetMethod() != null && curPropInfo.GetSetMethod() != null)
                {
                    // 인덱서가 아닌 프로퍼티
                    if (curPropInfo.Name != "Item")
                    {
                        // 원본으로 부터 프로퍼티 가져옴
                        object getValue = curPropInfo.GetGetMethod().Invoke(source, new object[] { });

                        // 필요시에 복제
                        if (getValue != null && getValue is DependencyObject)
                            getValue = Clone((DependencyObject)getValue);

                        // 프로퍼티 설정
                        if (curPropInfo.Name != "Template" && curPropInfo.Name != "TargetType")
                            curPropInfo.GetSetMethod().Invoke(cloned, new object[] { getValue });
                    }
                    // 인덱서
                    else
                    {
                        // 인덱서 카운트
                        int numberofItemInCollection = (int)curPropInfo.ReflectedType.GetProperty("Count").GetGetMethod().Invoke(source, new object[] { });

                        // 인덱서 순환
                        for (int i = 0; i < numberofItemInCollection; i++)
                        {
                            // 인덱서를 통해 아이템 가져옴
                            object getValue = curPropInfo.GetGetMethod().Invoke(source, new object[] { i });

                            // 필요시에 복제
                            if (getValue != null && getValue is DependencyObject)
                                getValue = Clone((DependencyObject)getValue);

                            // 컬렉션에 아이템 추가                            
                            curPropInfo.ReflectedType.GetMethod("Add").Invoke(cloned, new object[] { getValue });
                        }
                    }
                }
            }

            return cloned;
        }

        public static void checkAccessFile(DateTime receiveDate)
        {
            for (int i = 0; i < AWS.Config.AWSConfig.sCount; i++)
            {
                string folderName = AWSConfig.HOME_PATH + "\\AWSDATA";
                string pathString = System.IO.Path.Combine(folderName, "SubFolder");

                if (!Directory.Exists(folderName))
                    Directory.CreateDirectory(folderName);

                string devName = AWS.Config.AWSConfig.sValue[i].Name;

                folderName += @"\" + devName;

                if (!Directory.Exists(folderName))
                    Directory.CreateDirectory(folderName);

                String year = String.Format("{0:D4}", receiveDate.Year);

                folderName += @"\" + year;

                if (!Directory.Exists(folderName))
                    Directory.CreateDirectory(folderName);

                String month = String.Format("{0:D2}", receiveDate.Month);
                String day = String.Format("{0:D2}", receiveDate.Day);

                folderName += @"\" + month;
                if (!Directory.Exists(folderName))
                    Directory.CreateDirectory(folderName);

                string accessFile = AWSConfig.HOME_PATH + "\\AccessFile\\aws.mdb";

                DateTime today = receiveDate;

                String todayAccessDBFile = folderName + @"\" + "aws_" + String.Format("{0:D4}", today.Year)
                    + String.Format("{0:D2}", today.Month) + String.Format("{0:D2}", today.Day) + ".mdb";

                //오늘
                if (!File.Exists(todayAccessDBFile))
                {
                    System.IO.File.Copy(accessFile, todayAccessDBFile, true);
                }
            }
        }

        public static string ReadReg(string key)
        {
            string regSubkey = "SOFTWARE\\AWS";
            try
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey(regSubkey, true);

                if (rk == null)
                {
                    throw new Exception("HOME_PATH키가 존재하지 않습니다.") ;
                }

                string regStr = rk.GetValue(key) as string;

                return regStr;
            }
            catch(Exception e)
            {
                iLog.Error(e.Message);
                return null;
            }
        }

        public static Boolean WriteReg(string key, string value)
        {
            string regSubkey = "SOFTWARE\\AWS";
            try
            {
                /*
                RegistrySecurity rs = new RegistrySecurity();

                rs.AddAccessRule(new RegistryAccessRule(user,
                    RegistryRights.WriteKey | RegistryRights.ChangePermissions,
                    InheritanceFlags.None, PropagationFlags.None, AccessControlType.Deny));
                */
                RegistryKey rk = Registry.CurrentUser.CreateSubKey(regSubkey);
                if (rk == null)
                {
                    throw new Exception("키를 생성할 수 없습니다.");
                }

                rk.SetValue(key, value);

                return true;
            }
            catch (Exception e)
            {
                iLog.Error(e.Message);
                return false;
            }
        }

        public static byte[] StrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }

		/// <summary>
		/// CheckSumCheck 생성
		/// </summary>
		public static void CreateCheckSum(byte[] data, int HeaderSize, int TailSize)
		{
			data[data.Length - TailSize - 2] = 0;   // Xor
			data[data.Length - TailSize - 1] = 0;   // Add
			for (int i = 0 + HeaderSize; i < data.Length - TailSize - 2; i++)
			{
				data[data.Length - TailSize - 2] ^= data[i];
				data[data.Length - TailSize - 1] += data[i];
			}
		}
	}
}
