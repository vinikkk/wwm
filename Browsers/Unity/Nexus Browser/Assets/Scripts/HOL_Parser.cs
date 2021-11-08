using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct CommandLeaf
{
	public string command;
	public Action<object, string> function;

	public CommandLeaf(string c, Action<object, string> f)
	{
		command = c;
		function = f;
	}
}

public struct CommandNode
{
	public Dictionary<char, CommandBranch> pathList;
	public Action<object, string> pathEnd;
}

public class CommandBranch
{
	public CommandNode node = new CommandNode();

	public CommandBranch()
	{
		node.pathList = new Dictionary<char, CommandBranch>();
	}

	public void AddCommand(string path, string command, Action<object, string> function)
	{
		if(path.Length == 1) //End of the path
		{
			node.pathEnd = function;
			return;
		}
		else
		{
			if(node.pathList.ContainsKey(path[0])) //Has the path, add the rest of the path to the existing path
			{
				node.pathList[path[0]].AddCommand(path.Substring(1), command, function);
				return;
			}
			else
			{
				node.pathList.Add(path[0], new CommandBranch());
				node.pathList[path[0]].AddCommand(path.Substring(1), command, function);
				return;
			}
		}
	}

	public Action<object, string> GetCommandFunction(string path, string command)
	{
		if(path.Length == 1) //Function Found
		{
			return node.pathEnd;
		}
		else
		{
			if(node.pathList.ContainsKey(path[0])) //Has the path, keep going
			{
				return node.pathList[path[0]].GetCommandFunction(path.Substring(1), command);
			}
			else
			{
				Debug.Log($"Command Not Found: {command} at remaining path {path}");
				return null;
			}
		}
	}
}

public class HOL_Parser
{
	public static CommandBranch grammar = new CommandBranch();

	public static void Initialize()
	{
		grammar.AddCommand("rotation", "rotation", Rotation);
	}

	public static void Rotation(object caller, string val)
	{
		(caller as GameObject).transform.eulerAngles = Utility.StringToVector3(val);
	}
}

public class HOL_Interpreter
{

}
