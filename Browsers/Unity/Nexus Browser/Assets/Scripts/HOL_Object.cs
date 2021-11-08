using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HOL_Object : MonoBehaviour
{
	string testHOL = "rotation 45,45,0";
	string[] commandList;

	private void Start()
	{
		commandList = testHOL.Split(' ');
		HOL_Parser.grammar.GetCommandFunction(commandList[0], commandList[0]).Invoke(this.gameObject, commandList[1]);
	}

	private void Update()
	{
	}
}
