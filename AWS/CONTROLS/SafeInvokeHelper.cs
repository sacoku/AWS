//-----------------------------------------------------------------------
// <copyright file="SafeInvokeHelper.cs" company="[Company Name]">
//     Copyright (c) [Company Name] Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace AWS.CONTROL
{
    public class SafeInvokeHelper
    {
        static readonly ModuleBuilder builder;
        static readonly AssemblyBuilder myAsmBuilder;
        static readonly Hashtable methodLookup;

        static SafeInvokeHelper()
        {
            AssemblyName name = new AssemblyName();
            name.Name = "temp";
            myAsmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            builder = myAsmBuilder.DefineDynamicModule("TempModule");
            methodLookup = new Hashtable();
        }

        /// <summary>
        /// 폼 이름, 폼내의 메소드(public), 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public static object Invoke(System.Windows.Forms.Control obj, string methodName, params object[] paramValues)
        {
           
            Delegate del = null;
            string key = obj.GetType().Name + "." + methodName;
            Type tp;
            if (methodLookup.Contains(key))
                tp = (Type)methodLookup[key];
            else
            {
                Type[] paramList = new Type[obj.GetType().GetMethod(methodName).GetParameters().Length];
                int n = 0;
                foreach (ParameterInfo pi in obj.GetType().GetMethod(methodName).GetParameters()) paramList[n++] = pi.ParameterType;
                TypeBuilder typeB = builder.DefineType("Del_" + obj.GetType().Name + "_" + methodName, TypeAttributes.Class | TypeAttributes.AutoLayout | TypeAttributes.Public | TypeAttributes.Sealed, typeof(MulticastDelegate), PackingSize.Unspecified);
                ConstructorBuilder conB = typeB.DefineConstructor(MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new Type[] { typeof(object), typeof(IntPtr) });
                conB.SetImplementationFlags(MethodImplAttributes.Runtime);
                MethodBuilder mb = typeB.DefineMethod("Invoke", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig, obj.GetType().GetMethod(methodName).ReturnType, paramList);
                mb.SetImplementationFlags(MethodImplAttributes.Runtime);
                tp = typeB.CreateType();
                methodLookup.Add(key, tp);                
            }

            del = MulticastDelegate.CreateDelegate(tp, obj, methodName);
            return obj.Invoke(del, paramValues);
        }
    }


}
