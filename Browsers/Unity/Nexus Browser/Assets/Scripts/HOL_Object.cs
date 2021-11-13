using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HOL_Object : MonoBehaviour
{
	GenericDataTable variables = new GenericDataTable();

	StatementObject[] updateLogic;

	public void Initialize(string data)
	{
		//Can be a URL or a inline logic
		if (data.Contains("http://") || data.Contains("https://")) //It is URL
		{
			//Load script from web
			StartCoroutine(Utility.GetData(data, (string data) =>
			{
				ParseLogic(data, ref variables, ref updateLogic);
			}));
		}
		else
		{
			//Parse Logic
			ParseLogic(data, ref variables, ref updateLogic);
		}
	}

	private void ParseLogic(string data, ref GenericDataTable vars, ref StatementObject[] statementList)
	{
		//Sanitize
		data = data.Replace("\n", " ").Replace("\t", "").Replace(";", " ;");

		//Parse Logic
		HOL_Parser holParser = new HOL_Parser();
		holParser.ParseStatement(ref vars, ref statementList, data);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (updateLogic == null) return;

		Debug.Log("HOL LOADED!");
	}
}
