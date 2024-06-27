using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;


public class Helpers
{
    public static TEnum ParseEnum<TEnum>(string item, bool ignorecase = default(bool))
        where TEnum : struct
    {
        TEnum tenumResult = default(TEnum);
        return Enum.TryParse<TEnum>(item, ignorecase, out tenumResult) ?
            tenumResult : default(TEnum);
    }
    
   
    public static bool IsApproximately(float a, float b, float tolerance)
    {
        return Mathf.Abs(a - b) < tolerance;
    }

    public static bool IsVectorApproximately(Vector3 a, Vector3 b, float tolerance)
    {
        if (Mathf.Abs(a.x - b.x) < tolerance
            && Mathf.Abs(a.y - b.y) < tolerance
            && Mathf.Abs(a.z - b.z) < tolerance)
        {
            return true;
        }

        return false;
    }

    public static float Map01(float value, float min, float max)
    {
        return (value - min) * 1f / (max - min);
    }

    public static bool IsOdd(int value)
    {
        return value % 2 != 0;
    }


    #region create classes with reflection
    public static I CreateInstance<I>(string namespaceName, string name) where I : class
    {
        var typeClass = System.Type.GetType(namespaceName + name);
        return Activator.CreateInstance(typeClass) as I;
    }

    public static I CreateInstance<I>(string namespaceName, string name, object[] someParams) where I : class
    {
        var typeClass = System.Type.GetType(namespaceName + name);
        return Activator.CreateInstance(typeClass, someParams) as I;
    }

    public static object CreateInstance(string strFullyQualifiedName)
    {
        var t = Type.GetType(strFullyQualifiedName);
        return Activator.CreateInstance(t);
    }

    public static object CreateInstance(string strFullyQualifiedName, object[] someParams)
    {
        var t = Type.GetType(strFullyQualifiedName);
        return Activator.CreateInstance(t, someParams);
    }
    
    #endregion



    
}
