using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpaceManager : MonoBehaviour
{
	public string directoryName;
	public string fileName;

	/*
 * 	TEST CODE
 * 	
		Vector3 newRotation = this.rotation.y + Vector3(0,1,0);
		this.rotation = newRotation;
*/

	//string UrlHON = "raw.githubusercontent.com/vinikkk/wwn/main/Browsers/Unity/Nexus%20Browser/Assets/TestData/HON/SimpleSample2.hon";
	string UrlHON = "raw.githubusercontent.com/vinikkk/wwn/main/Browsers/Unity/Nexus%20Browser/Assets/TestData/HON/MeshSample_Warehouse.hon";
	string UrlHOL = "raw.githubusercontent.com/vinikkk/wwn/main/Browsers/Unity/Nexus%20Browser/Assets/TestData/HOL/Update_SpinY.hol";

	private void Awake()
	{
		HON_Parser.Instance.Initialize();
	}

	private void Start()
	{
		//Setup directory URL for relative pathing
		directoryName = UrlHON.Substring(0, UrlHON.LastIndexOf('/'));
		fileName = Path.GetFileName(UrlHON);

		//Get HON URL data
		StartCoroutine(Utility.GetData("https://" + directoryName + "/" + fileName, (object result) =>
		{
			//Sanitize data
			string data = (result as string).Replace("\n", " ").Replace("\t", "");

			//Parse/Build the world
			HON_Parser.Instance.Parse(this, data.Split(' '));
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
