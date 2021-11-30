using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HOL_Object : MonoBehaviour
{
	Dictionary<string, object> variables = new Dictionary<string, object>();

	StatementObject[] updateLogic;

	public void Initialize(string data)
	{
		//Can be a URL or a inline logic
		if (data.Contains("http://") || data.Contains("https://")) //It is URL
		{
			//Load script from web
			StartCoroutine(Utility.GetData(data, (object data) =>
			{
				ParseLogic((data as string), ref variables, ref updateLogic);
			}));
		}
		else
		{
			//Parse Logic
			ParseLogic(data, ref variables, ref updateLogic);
		}
	}

	private void ParseLogic(string data, ref Dictionary<string, object> vars, ref StatementObject[] statementList)
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

		string currentVariableName;

		for (int i = 0; i < updateLogic.Length; i++)
		{
			//Varibles
			if(updateLogic[i].type == typeof(Constants.Variable))
			{
				currentVariableName = updateLogic[i].data as string;
				continue;
			}

			//Compound Variables
			if(updateLogic[i].type == typeof(Constants.CompoundVariable))
			{
				string[] s = (updateLogic[i].data as string).Split('.');

				object result = null;
				object caller = null;
				if(s[0] == "this")
				{
					caller = this;
				}
				else
				{
					caller = variables[s[0]];
				}

				for (int j = 1; j < s.Length; j++)
				{
					caller = Utility.GetPropValue(caller, s[j]);
				}

				Type t = caller.GetType();  

				continue;
			}
		}
	}

	public void ExecuteStatemenet(StatementObject statementObject)
	{

	}
}
