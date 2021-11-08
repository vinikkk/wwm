using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpaceManager : MonoBehaviour
{
	string testData; // = "space { camera { wpos 0,1,-10 } obj { light directional rotation 45,45,0 } obj { mesh sphere wpos 0,1,0 } obj { mesh plane scale 10,1,10 } }";

	string testURL = "https://raw.githubusercontent.com/vinikkk/wwn/main/Browsers/Unity/Nexus%20Browser/Assets/TestData/HON/SimpleSample2.hon";

	private void Awake()
	{
		HON_Parser.Initialize();
		HOL_Parser.Initialize();
	}

	private void Start()
	{
		//Get URL data
		StartCoroutine(GetData(() =>
		{
			//Sanitize data
			testData = testData.Replace("\n", " ").Replace("\t", "");

			//Parse/Build the world
			HON_Parser.Parse(testData.Split(' '));
		}));
	}

	IEnumerator GetData(Action callback)
	{
		UnityWebRequest www = UnityWebRequest.Get(testURL);
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.Log(www.error);
		}
		else
		{
			testData = www.downloadHandler.text;
			callback?.Invoke();
		}
	}
}
