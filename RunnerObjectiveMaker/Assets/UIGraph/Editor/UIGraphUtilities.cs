using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

public static class UIGraphUtilities
{
	static string nodeNameObjective = "[*]";
	static Type[] nodeTypesObjective = new Type[] { typeof(Canvas), typeof(Image), typeof(GraphNode), typeof(ObjectiveWrapper) };
	public static Vector3 nextPos = Vector3.zero;

	static Vector2 nodeSize = new Vector2(2, 1);

	static Type[] textTypes = new Type[] { typeof(Text) };
	static int textSize = 48;



	static bool isNextPosOk()
	{
		var nodes = UnityEngine.Object.FindObjectsOfType<RectTransform>();
		foreach (var node in nodes)
		{
			var nrt = node.GetComponent<RectTransform>();
			if (nrt == null) continue;
			if (Vector3.Distance(nrt.position, nextPos) < 1.5f)
			{
				return false;
			}
		}
		return true;
	}

	[MenuItem("[Objective System]/Objective Node")]
	public static RectTransform CreateObjective()
	{
		while (!isNextPosOk()) { nextPos += Vector3.right * 2.2f; };

		GameObject newnode = new GameObject(nodeNameObjective, nodeTypesObjective);
		RectTransform transform = newnode.GetComponent<RectTransform>();
		transform.SetParent(ObjectiveManager.instance.transform);
		transform.position = nextPos;
		transform.localScale = Vector3.one;
		transform.sizeDelta = nodeSize;
		nextPos += Vector3.down * 2f;

		return transform;
	}

	//public static void CreateText(Transform parent, string txt) {
	//	GameObject go = new GameObject(textName, textTypes);

	//	RectTransform transform = go.GetComponent<RectTransform>();
	//	transform.SetParent(parent, false);
	//	transform.anchorMin = Vector2.zero;
	//	transform.anchorMax = Vector2.one;
	//	transform.sizeDelta = Vector2.zero;

	//	Text text = go.GetComponent<Text>();
	//	text.alignment = TextAnchor.MiddleCenter;
	//	text.color = Color.black;
	//	text.fontSize = textSize;
	//	text.text = txt;
	//}

	[MenuItem("[Objective System]/Serialize")]
	public static void SerializeObjectives()
	{
		var ows = UnityEngine.Object.FindObjectsOfType<ObjectiveWrapper>();
		var cns = UnityEngine.Object.FindObjectsOfType<Connection>();

		//check for obj data
		var allobjs = new List<ObjectiveWrapper>();
		foreach (var ow in ows)
		{
			//reset objwrapper
			if (ow.Requirements == null)
				ow.Requirements = new List<ObjectiveWrapper>();
			else
				ow.Requirements.Clear();
			ow.Data.RequirementCodes = null;
			ow.HasError = false;
			ow.HasDuplicate = false;

			//check for error
			if (ow.Data != null)
			{
				allobjs.Add(ow);
			}
			else
				ow.HasError = true;
		}

		Debug.Log("Objective Data Checked ... [OK]");

		//sort by unique order key
		allobjs.Sort((a, b) =>
		{
			var A = a.Data.GetUniqueCode();
			var B = b.Data.GetUniqueCode();
			if (A == B) return 0;
			else if (A > B) return 1;
			else return -1;
		});

		//check for duplicate unique codes
		bool duplicateFound = false;
		for (int i = 1; i < allobjs.Count; i++)
		{
			if(allobjs[i].Data.GetUniqueCode()==allobjs[i-1].Data.GetUniqueCode())
			{
				duplicateFound = true;
				allobjs[i].HasDuplicate = true;
				allobjs[i-1].HasDuplicate = true;
			}
		}
		Debug.Log("Duplicate Code Checked ... " + (duplicateFound ? "[Error]" : "[OK]"));
		if (duplicateFound) return;

		//check for connections
		int deletedCns = 0;
		foreach (var cn in cns)
		{
			if (cn != null && cn.target != null && cn.target.Length == 2)
			{
				var to = cn.target[0].GetComponent<ObjectiveWrapper>();
				var from = cn.target[1].GetComponent<ObjectiveWrapper>();

				//reset
				cn.points[0].HasError = false;
				cn.points[1].HasError = false;

				//check for error
				if (to == null || to.Data == null)
				{
					cn.points[0].HasError = true;
				}
				else if (from == null || from.Data == null)
				{
					cn.points[1].HasError = true;
				}
				else
				{
					//add req
					to.Requirements.Add(from);
				}
			}
			else
			{
				deletedCns++;
				//destroy
				UnityEngine.Object.Destroy(cn.gameObject);
			}
		}

		Debug.Log(string.Format("{0} Deleted Connections ... [OK]", deletedCns));
	
		//set requirements
		foreach (var obj in allobjs)
		{
			obj.Data.RequirementCodes = obj.Requirements.Select(x => x.Data.GetUniqueCode()).ToArray();
		}

		Debug.Log("Requirements set ... [OK]");

		//save
		ObjectiveManager.instance.Save(allobjs.Select(x=>x.Data).ToArray());
		Debug.Log("Saved ... [Done]");
	}

	[MenuItem("[Objective System]/Load Objectives")]
	public static void LoadObjectives()
	{
		ObjectiveManager.instance.Load();

		//draw objectives
		foreach (var obj in ObjectiveManager.instance.AllObjectives)
		{
			while (!isNextPosOk()) { nextPos += Vector3.right * 2.2f; };

			GameObject newnode = new GameObject(obj.Name, nodeTypesObjective);
			newnode.GetComponent<ObjectiveWrapper>().Data = obj;
			RectTransform transform = newnode.GetComponent<RectTransform>();
			transform.SetParent(ObjectiveManager.instance.transform);
			transform.position = nextPos;
			transform.localScale = Vector3.one;
			transform.sizeDelta = nodeSize;
			nextPos += Vector3.down * 2f;
		}

		//locate from scene
		var objs = UnityEngine.GameObject.FindObjectsOfType<ObjectiveWrapper>();

		foreach (var obj in objs)
		{
			if (obj.Data.RequirementCodes == null) continue;

			//draw connections
			foreach (var reqCode in obj.Data.RequirementCodes)
			{
				var target = obj.GetComponent<RectTransform>();

				//find requirement (objective wrapper) in scene that matches the unique code
				var ow = objs.First(x => x.Data.GetUniqueCode() == reqCode);

				ConnectionManager.CreateConnection(target, ow.GetComponent<RectTransform>());
			}
		}
	}

}
