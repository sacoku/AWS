using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace UTIL
{
    delegate void LogWrite(object msg);

    class CommonUtil
    {
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
                Console.WriteLine(e.ToString());
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
                Console.WriteLine(e.ToString());
                return false;
            }
        }
	}
}
