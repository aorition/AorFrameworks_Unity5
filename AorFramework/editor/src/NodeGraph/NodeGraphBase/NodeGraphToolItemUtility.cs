using System;
using System.Reflection;
using UnityEngine;

namespace AorFramework.NodeGraph
{
    public class NodeGraphToolItemUtility
    {

        public static void CallToolItemDefinedMethod(string FullClassNameAtCallData, Vector2 inputPos)
        {
            string[] dataSp = FullClassNameAtCallData.Split('@');
            CallToolItemDefinedMethod(dataSp[0], dataSp[1], inputPos);
        }

        public static void CallToolItemDefinedMethod (string FullClassName, string CallData, Vector2 inputPos)
        {

            Type theType = Assembly.GetExecutingAssembly().GetType(FullClassName);
            if (theType == null)
            {
                Debug.LogError("NodeGraphToolItemUtility.CallToolItemDefinedMethod Error :: can not find " + FullClassName + " in " + Assembly.GetExecutingAssembly().FullName);
                return;
            }

            MethodInfo info = theType.GetMethod(CallData, BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod);
            if (info == null)
            {
                Debug.LogError("NodeGraphToolItemDefinder.CallTool Error :: Can not Find Method : " + CallData);
                return;
            }
            info.Invoke(null, new object[] { inputPos });
        }

    }
}
