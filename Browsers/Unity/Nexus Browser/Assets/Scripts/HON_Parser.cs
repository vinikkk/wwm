using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct CommandData
{
	public bool hasParameter;
	public Action function;

	public CommandData(Action cb, bool hp = false)
	{
		function = cb;
		hasParameter = hp;
	}
}

public class HON_Parser : Singleton<HON_Parser>
{
	//Grammar
	Dictionary<string, CommandData> commandCallbackDictionary = new Dictionary<string, CommandData>(); //Store all the callbacks of the expected commands in the parser

	Stack<GameObject> objectStack = new Stack<GameObject>();

	string currentCommand;
	string currentParameter;
	GameObject currentObject;
	SpaceManager currentSpace;

	string vec3Regex = "/\\d+,\\d+,\\d+/gm";
	string wordOrString = "/'\\w+'|\\w+/gm";

	public void Initialize()
	{
		//Objects
		commandCallbackDictionary.Add("space", new CommandData(NewObject));
		commandCallbackDictionary.Add("obj", new CommandData(NewObject));
		commandCallbackDictionary.Add("camera", new CommandData(CameraObject));

		//Components
		commandCallbackDictionary.Add("mesh", new CommandData(AddMesh, true));
		commandCallbackDictionary.Add("logic", new CommandData(AddLogic, true));

		//Complex Components
		commandCallbackDictionary.Add("light", new CommandData(AddLight, true));

		//Parameters
		commandCallbackDictionary.Add("rotation", new CommandData(SetRotation, true));
		commandCallbackDictionary.Add("wpos", new CommandData(SetWPos, true));
		commandCallbackDictionary.Add("scale", new CommandData(SetScale, true));
		commandCallbackDictionary.Add("name", new CommandData(SetName, true));

		//Stack handlers
		//Add to stack last object created
		commandCallbackDictionary.Add("{", new CommandData(StackObject));
		//Remove from stack and Instantiate last object created
		commandCallbackDictionary.Add("}", new CommandData(UnstackObject));
	}

	public void Parse(SpaceManager space, string[] data)
	{
		currentSpace = space;

		for (int i = 0; i < data.Length; i++)
		{
			if (!commandCallbackDictionary.ContainsKey(data[i]))
			{
				Debug.Log($"The command '{data[i]}' does not exist in the grammar.");
				continue;
			}

			currentCommand = data[i];
			if (commandCallbackDictionary[currentCommand].hasParameter)
			{
				currentParameter = data[++i];
			}
			commandCallbackDictionary[currentCommand].function.Invoke();
		}
	}

	public void NewObject()
	{
		currentObject = new GameObject(currentCommand);
	}

	public void CameraObject()
	{
		currentObject = Camera.main.gameObject;
	}

	public void StackObject()
	{
		objectStack.Push(currentObject);
	}

	public void UnstackObject()
	{
		objectStack.Pop();
	}

	public void AddMesh()
	{
		if (currentParameter.Contains("/")) //Means it is a path
		{
			//Prepare to download the gltf and binary files
			string url;

			if (currentParameter.Contains("http")) //Means it is a Full URL
			{
				url = currentParameter.Replace("\"", "") + ".glb";
			}
			else //Means it is a Relative URL
			{
				url = currentSpace.directoryName + "/" + currentParameter + ".glb";
			}

			//Download the description file
			string localFilePath = string.Empty;

			StartCoroutine(Utility.DownloadFile(url, (string p) =>
			{
				localFilePath = p;

				Utility.LoadMesh(localFilePath, (GameObject go, AnimationClip[] anims) =>
				{
					//What to do here?
					go.name = "Mesh";
					go.transform.SetParent(currentObject.transform);
				});
			}));
		}
		else //It is a default mesh
		{
			MeshFilter mf = currentObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
			MeshRenderer mr = currentObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

			mr.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));

			switch (currentParameter)
			{
				case "cube":
					mf.sharedMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
					break;
				case "sphere":
					mf.sharedMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
					break;
				case "plane":
					mf.sharedMesh = Resources.GetBuiltinResource<Mesh>("Plane.fbx");
					break;
			}
		}
	}

	public void AddLogic()
	{
		HOL_Object obj = currentObject.AddComponent(typeof(HOL_Object)) as HOL_Object;
		obj.Initialize(currentParameter);
	}

	public void AddLight()
	{
		Light l = currentObject.AddComponent(typeof(Light)) as Light;

		l.shadows = LightShadows.Soft;

		switch (currentParameter)
		{
			case "directional":
				l.type = LightType.Directional;
				break;
			case "point":
				l.type = LightType.Point;
				break;
			case "spot":
				l.type = LightType.Spot;
				break;
		}
	}

	public void SetRotation()
	{
		currentObject.transform.eulerAngles = Utility.StringToVector3(currentParameter);
	}

	public void SetWPos()
	{
		currentObject.transform.position = Utility.StringToVector3(currentParameter);
	}

	public void SetScale()
	{
		currentObject.transform.localScale = Utility.StringToVector3(currentParameter);
	}

	public void SetName()
	{
		currentObject.name = currentParameter;
	}
}
