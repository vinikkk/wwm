using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpaceManager : MonoBehaviour
{

	/*
 * 	TEST CODE
 * 	
		Vector3 newRotation = this.rotation.y + Vector3(0,1,0);
		this.rotation = newRotation;
*/

	//string UrlHON = "https://raw.githubusercontent.com/vinikkk/wwn/main/Browsers/Unity/Nexus%20Browser/Assets/TestData/HON/SimpleSample2.hon";
	string UrlHON = "https://raw.githubusercontent.com/vinikkk/wwn/main/Browsers/Unity/Nexus%20Browser/Assets/TestData/HON/MeshSample.hon";
	string UrlHOL = "https://raw.githubusercontent.com/vinikkk/wwn/main/Browsers/Unity/Nexus%20Browser/Assets/TestData/HOL/Update_SpinY.hol";

	private void Awake()
	{
		HON_Parser.Instance.Initialize();
	}

	private void Start()
	{
		//Get HON URL data
		StartCoroutine(Utility.GetData(UrlHON, (object result) =>
		{
			//Sanitize data
			string data = (result as string).Replace("\n", " ").Replace("\t", "");

			//Parse/Build the world
			HON_Parser.Instance.Parse(data.Split(' '));
		}));

		/*
		//Get HOL URL data
		StartCoroutine(Utility.GetData(UrlHOL, (string result) =>
		{
			//Sanitize data
			string data = result.Replace("\n", " ").Replace("\t", "").Replace(";", " ;");

			//Parse
			//FunctionDefinition fd = HOL_Parser.Parse(data);
			HOL_Parser2 holParser = new HOL_Parser2();
			holParser.ParseStatement(data);
			Debug.Log("hey");
		}));
		*/
	}
}
