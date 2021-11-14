using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class Utility
{
	public static Vector3 StringToVector3(string str)
	{
		string[] parameter = str.Split(',');

		return new Vector3(Int32.Parse(parameter[0]), Int32.Parse(parameter[1]), Int32.Parse(parameter[2]));
	}

	public static object GetPropValue(object src, string propName)
	{
		Type t = src.GetType();
		PropertyInfo p = t.GetProperty(propName);
		object o = p.GetValue(src);

		return o;
	}

	public static R ExecuteMethod<R>(object src, string methodName, object[] parameters)
	{
		MethodInfo method = src.GetType().GetMethod(methodName);
		object result = method.Invoke(src, parameters);
		return (R)result;
	}

	public static IEnumerator GetData(string url, Action<string> callback)
	{
		UnityWebRequest www = UnityWebRequest.Get(url);
		www.useHttpContinue = false;
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.Log(www.error);
		}
		else
		{
			callback?.Invoke(www.downloadHandler.text);
		}
	}
}
