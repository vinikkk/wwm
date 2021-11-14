using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public struct DataTableObject
{
	public Type type;
	public string name;
	public object data;

	public DataTableObject(Type t, string n, object d)
	{
		type = t;
		name = n;
		data = d;
	}
}

public class GenericDataTable
{
	public Dictionary<Type, Dictionary<string, object>> table = new Dictionary<Type, Dictionary<string, object>>();

	public void SetData(Type type, string name, object value)
	{
		if (!table.ContainsKey(type))
		{
			table.Add(type, new Dictionary<string, object>());
		}

		if (!table[type].ContainsKey(name))
		{
			table[type].Add(name, null);
		}

		table[type][name] = value;
	}

	public object GetData(Type type, string name)
	{
		return table[type][name];
	}

	public DataTableObject GetDataByName(string name)
	{
		DataTableObject result = new DataTableObject();

		foreach (KeyValuePair<Type, Dictionary<string, object>> typeTable in table)
		{
			if(typeTable.Value.ContainsKey(name))
			{
				result.type = typeTable.Key;
				result.name = name;
				result.data = typeTable.Value[name];
				break;
			}
		}

		return result;
	}

	public bool ContainsData(Type type, string dataName)
	{
		return table.ContainsKey(type) && table[type].ContainsKey(dataName);
	}
}

public class Constants
{
	//Markups
	public const char CODEBLOCK_START = '{';
	public const char CODEBLOCK_END = '}';
	public const char PARAMETER_START = '(';
	public const char PARAMETER_END = ')';
	public const char STATEMENT_END = ';';
	public const char RECEIVE = '=';
	public const char ACCESS_DOT = '.';

	//Datatypes
	public static List<Type> DATATYPES = new List<Type> { typeof(void), typeof(int), typeof(float), typeof(bool), typeof(string), typeof(Vector3) };

	//Standard Functions
	//Scope: FunctionExternal AND StatementExternal
	public const string START_FUNCTION = "Start";
	public const string UPDATE_FUNCTION = "Update";

	//Operators
	//Scope: FunctionInternal AND StatementInternal
	public static List<char> OPERATOR_FUNCTIONS = new List<char>() { '+', '-', '*', '/' };

	//Standard Statements
	//Scope: FunctionInternal AND StatementExternal
	public const string IF_STATEMENT = "if";
	public const string ELSE_STATEMENT = "else";

	//Custom Data Types
	public struct Variable { };
	public struct CompoundVariable { };
	public struct Constructor { };
	public struct Function { };
	public struct OperatorFunction { };
	public struct StatementEnd { };
}

public enum FunctionBuildingState
{
	FunctionExternal,
	FunctionInternal
}

public enum StatementBuildingState
{
	StatementExternal,
	StatementInternal
}

public struct StatementObject
{
	public Type type;
	public object data;

	public StatementObject(Type t, object d)
	{
		type = t;
		data = d;
	}
}

public class StatementDefinition
{
	public Type type;
	public string name;
	public List<StatementObject> instructions;
}

public class HOL_Parser
{

	List<string> statementList;

	public void ParseStatement(ref Dictionary<string, object> variables, ref StatementObject[] logicList, string data)
	{
		statementList = new List<string>(Regex.Split(data, $"({Constants.RECEIVE})|({Constants.STATEMENT_END})"));

		FunctionBuildingState functionBuildingState = FunctionBuildingState.FunctionExternal;
		StatementBuildingState statementBuildingState = StatementBuildingState.StatementExternal;

		List<StatementObject> currentStatementObjects = new List<StatementObject>();

		//Trim spaces and sanitize
		for (int i = 0; i < statementList.Count; i++)
		{
			statementList[i] = statementList[i].Trim();
		}

		//Temporarily starting inside functions, since there is not function structure yet
		functionBuildingState = FunctionBuildingState.FunctionInternal;

		//Process each line
		for (int statementIndex = 0; statementIndex < statementList.Count; statementIndex++)
		{
			string[] codeFragmentList = statementList[statementIndex].Split();

			Type currentType = null;

			for (int codeFragmentIndex = 0; codeFragmentIndex < codeFragmentList.Length; codeFragmentIndex++)
			{
				if (functionBuildingState == FunctionBuildingState.FunctionInternal &&
				   statementBuildingState == StatementBuildingState.StatementExternal)
				{
					int typeIndex = Constants.DATATYPES.FindIndex(x => x.ToString().Contains(codeFragmentList[codeFragmentIndex]));
					if (typeIndex != -1)
					{
						currentType = Constants.DATATYPES[typeIndex];
						//currentStatementObjects.Add(new StatementObject(Constants.DATATYPES[typeIndex], codeFragmentList[codeFragmentIndex]));
						continue;
					}

					//Need to check if it not any language standard function like if/else/else if,for, while, dowhile, foreach, switch
					//TODOOOOOOOOOOOOOOOOOOOOOOOOOO

					//Check if it is a compound variable or a normal one
					if (codeFragmentList[codeFragmentIndex].Contains(Constants.ACCESS_DOT.ToString())) //compound variable
					{
						currentStatementObjects.Add(new StatementObject(typeof(Constants.CompoundVariable), codeFragmentList[codeFragmentIndex]));
						continue;
					}
					else if (Regex.IsMatch(codeFragmentList[codeFragmentIndex], $"(^\\d+)")) //Its raw int value
					{
						currentStatementObjects.Add(new StatementObject(typeof(int), int.Parse(codeFragmentList[codeFragmentIndex])));
						continue;
					}
					else if (Regex.IsMatch(codeFragmentList[codeFragmentIndex], $"(^\\w+)")) //Its a variable
					{
						currentStatementObjects.Add(new StatementObject(typeof(Constants.Variable), codeFragmentList[codeFragmentIndex]));
						//Add a variable into the object data table

						//variables.SetData(currentType, codeFragmentList[codeFragmentIndex], null);
						variables.Add(codeFragmentList[codeFragmentIndex], null);

						continue;
					}

					//Here we should get the RECEIVE for statement start
					if (codeFragmentList[codeFragmentIndex][0] == Constants.RECEIVE)
					{
						currentStatementObjects.Add(new StatementObject(typeof(Constants.OperatorFunction), codeFragmentList[codeFragmentIndex][0]));
						statementBuildingState = StatementBuildingState.StatementInternal;
						continue;
					}
				}

				if (functionBuildingState == FunctionBuildingState.FunctionInternal &&
				   statementBuildingState == StatementBuildingState.StatementInternal)
				{
					//Break instructions into pieces here

					//Protection
					if (codeFragmentList[codeFragmentIndex] == string.Empty) continue;

					//Check if its any standard operator function
					if (Constants.OPERATOR_FUNCTIONS.Contains(codeFragmentList[codeFragmentIndex][0]))
					{	
						currentStatementObjects.Add(new StatementObject(typeof(Constants.OperatorFunction), codeFragmentList[codeFragmentIndex][0]));
						continue;
					}

					//Check if end of statement
					if(codeFragmentList[codeFragmentIndex][0] == Constants.STATEMENT_END)
					{
						currentStatementObjects.Add(new StatementObject(typeof(Constants.OperatorFunction), codeFragmentList[codeFragmentIndex][0]));
						continue;
					}

					//Check if it is a compound variable or a normal one
					//Can be variables, values or constructors!
					if (codeFragmentList[codeFragmentIndex].Contains(Constants.ACCESS_DOT.ToString())) //compound variable
					{
						currentStatementObjects.Add(new StatementObject(typeof(Constants.CompoundVariable), codeFragmentList[codeFragmentIndex]));
						continue;
					}
					else if (Regex.IsMatch(codeFragmentList[codeFragmentIndex], $"(^\\d+)")) //Its raw int value
					{
						currentStatementObjects.Add(new StatementObject(typeof(int), int.Parse(codeFragmentList[codeFragmentIndex])));
						continue;
					}
					else if (Regex.IsMatch(codeFragmentList[codeFragmentIndex], $"\\(|\\)")) //Its either a function or a constructor
					{
						//find if its a constructor	or function
						string[] s = codeFragmentList[codeFragmentIndex].Split('(');

						int typeIndex = Constants.DATATYPES.FindIndex(x => x.ToString().Contains(s[0]));
						if (typeIndex != -1) //Its a constructor
						{
							currentStatementObjects.Add(new StatementObject(typeof(Constants.Constructor), codeFragmentList[codeFragmentIndex]));
							continue;
						}
						else //Its a function
						{
							currentStatementObjects.Add(new StatementObject(typeof(Constants.Function), codeFragmentList[codeFragmentIndex]));
							continue;
						}
					}
					else if (Regex.IsMatch(codeFragmentList[codeFragmentIndex], $"(^\\w+)")) //Its a variable
					{
						currentStatementObjects.Add(new StatementObject(typeof(Constants.Variable), codeFragmentList[codeFragmentIndex]));
						continue;
					}

				}

				Debug.Log("he");
			}
		}

		logicList = currentStatementObjects.ToArray();
	}

	//Auxiliary Functions

}
