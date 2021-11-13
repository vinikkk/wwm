using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct CommandLeaf<T>
{
	public string command;
	public T function;

	public CommandLeaf(string c, T f)
	{
		command = c;
		function = f;
	}
}

public struct CommandNode<T>
{
	public Dictionary<char, GrammarTree<T>> pathList;
	public T pathEnd;
}

public class GrammarTree<T>
{
	public CommandNode<T> node = new CommandNode<T>();

	public GrammarTree()
	{
		node.pathList = new Dictionary<char, GrammarTree<T>>();
	}

	public void AddCommand(string path, string command, T data)
	{
		if (path.Length == 1) //End of the path
		{
			node.pathEnd = data;
			return;
		}
		else
		{
			if (node.pathList.ContainsKey(path[0])) //Has the path, add the rest of the path to the existing path
			{
				node.pathList[path[0]].AddCommand(path.Substring(1), command, data);
				return;
			}
			else
			{
				node.pathList.Add(path[0], new GrammarTree<T>());
				node.pathList[path[0]].AddCommand(path.Substring(1), command, data);
				return;
			}
		}
	}

	public object GetData(string path, string command)
	{
		if (path.Length == 1) //Function Found
		{
			return node.pathEnd;
		}
		else
		{
			if (node.pathList.ContainsKey(path[0])) //Has the path, keep going
			{
				return node.pathList[path[0]].GetData(path.Substring(1), command);
			}
			else
			{
				Debug.Log($"Command Not Found: {command} at remaining path {path}");
				return null;
			}
		}
	}

	public void SetData(string path, T data)
	{
		string command = path;

		if (path.Length == 1) //Function Found
		{
			node.pathEnd = data;
			return;
		}
		else
		{
			if (node.pathList.ContainsKey(path[0])) //Has the path, keep going
			{
				node.pathList[path[0]].SetData(path.Substring(1), data);
				return;
			}
			else
			{
				Debug.Log($"Command Not Found: {command} at remaining path {path}");
				return;
			}
		}
	}

	public bool HasPath(string path)
	{
		if (path.Length == 0) //Function Found
		{
			return true;
		}
		else
		{
			if (node.pathList.ContainsKey(path[0])) //Has the path, keep going
			{
				return node.pathList[path[0]].HasPath(path.Substring(1));
			}
			else
			{
				return false;
			}
		}
	}
}

