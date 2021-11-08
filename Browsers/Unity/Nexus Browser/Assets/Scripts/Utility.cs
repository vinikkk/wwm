using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
	public static Vector3 StringToVector3(string str)
	{
		string[] parameter = str.Split(',');

		return new Vector3(Int32.Parse(parameter[0]), Int32.Parse(parameter[1]), Int32.Parse(parameter[2]));
	}
}
