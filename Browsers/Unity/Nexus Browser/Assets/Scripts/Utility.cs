using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Utility
{
	public static Vector3 StringToVector3(string str)
	{
		string[] parameter = str.Split(',');

		return new Vector3(Int32.Parse(parameter[0]), Int32.Parse(parameter[1]), Int32.Parse(parameter[2]));
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
