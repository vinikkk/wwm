using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using Siccity.GLTFUtility;

public class Utility
{
	public static Vector3 StringToVector3(string str)
	{
		string[] parameter = str.Split(',');

		return new Vector3(float.Parse(parameter[0]), float.Parse(parameter[1]), float.Parse(parameter[2]));
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

	public static IEnumerator GetData(string url, Action<object> callback)
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

	/// <summary>
	/// Download a file into the disk
	/// </summary>
	/// <param name="url"></param>
	/// <param name="callback"></param>
	/// <returns></returns>
	public static IEnumerator DownloadFile(string url, Action<string> callback)
	{
		string fileName = url.Substring(url.LastIndexOf('/') + 1);
		string fileDirectory = Application.persistentDataPath + "/Data/";

		Debug.Log($"Downloading file:\n{fileName} => {fileDirectory}");

		//If exists return and invoke the callback, if not proceed to download
		if (System.IO.File.Exists(fileDirectory + fileName))
		{
			callback?.Invoke(fileDirectory + fileName);
			yield break;
		}

		//Create directory if it does not exist
		System.IO.Directory.CreateDirectory(fileDirectory);

		//Download File
		UnityWebRequest www = UnityWebRequest.Get(url);
		www.useHttpContinue = false;
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.Log(www.error);
		}
		else
		{
			System.IO.File.WriteAllBytes(fileDirectory + fileName, www.downloadHandler.data);

			callback?.Invoke(fileDirectory + fileName);
		}
	}

	public static void LoadMesh(string path, Action<GameObject, AnimationClip[]> callback)
	{
		Importer.ImportGLBAsync(path, new ImportSettings(), callback);
	}
}
