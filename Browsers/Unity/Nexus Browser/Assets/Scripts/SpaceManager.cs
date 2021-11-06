using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;

public class SpaceManager : MonoBehaviour
{
	private static readonly HttpClient client = new HttpClient();

	string testData = "space { camera { wpos 0,1,-10 } obj { light directional rotation 45,45,0 } obj { mesh sphere wpos 0,1,0 } obj { mesh plane scale 10,1,10 } }";

	string testURL = "https://raw.githubusercontent.com/vinikkk/wwn/main/Browsers/Unity/Nexus%20Browser/Assets/TestData/HON/SimpleSample.hon";

	private void Awake()
	{
		HON_Parser.Initialize();
	}

	private void Start()
	{
		//Get URL data
		testData = client.GetStringAsync(testURL).Result;

		//Sanitize data
		testData = testData.Replace("\n", " ").Replace("\t","");

		//Parse/Build the world
		HON_Parser.Parse(testData.Split(' '));
	}
}
