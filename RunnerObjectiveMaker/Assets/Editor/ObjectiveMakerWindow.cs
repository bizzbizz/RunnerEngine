using UnityEngine;
using UnityEditor;
using RunnerData;
using System.IO;
using System.Linq;
public class ObjectiveMakerWindow : EditorWindow
{
	#region Window
	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/Objective Maker Window")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		//var ew = GetWindowWithRect<BlockMakerWindow>(new Rect(0, 0, 200, 600));
		//if (ew != null)
		//	ew.Close();
		GetWindow<ObjectiveMakerWindow>();

	}
	#endregion


	ObjectiveManager _manager;
	ObjectiveManager Manager
	{
		get
		{
			if (_manager == null)
			{
				_manager = FindObjectOfType<ObjectiveManager>();
				if (_manager == null)
					_manager = new GameObject("_Objective Manager_").AddComponent<ObjectiveManager>();
			}
			return _manager;
		}
	}


	void AddObjective(Objective o)
	{
		var go = new GameObject(MakeNameForObjective(o)).AddComponent<ObjectiveWrapper>();
		go.transform.parent = Manager.transform;
		go.Data = o;
	}

	ObjectiveWrapper selectedOW = null;

	void Update()
	{
		if (Selection.activeGameObject == null) return;

		var ow = Selection.activeGameObject.GetComponent<ObjectiveWrapper>();
		if (ow != selectedOW && ow != null)
		{
			selectedOW = ow;
			EditorUtility.SetDirty(ow);
			Repaint();
		}
	}
	void OnGUI()
	{

		GUI.skin.button.fixedHeight = 30;

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Add"))
		{
			AddObjective(new Objective());
		}
		if (GUILayout.Button("Auto-Rename All"))
		{
			AutoRenameAll();
		}
		if (GUILayout.Button("Sort All"))
		{
			SortObjectivesByName();
		}
		if (GUILayout.Button("Load"))
		{
			Load();
		}
		if (GUILayout.Button("Save"))
		{
			Save();
		}
		EditorGUILayout.EndHorizontal();


		if (selectedOW != null && selectedOW.Data != null)
		{
			var o = selectedOW.Data;

			GUILayout.Label("Info", EditorStyles.boldLabel);
			o.Name = EditorGUILayout.TextField("Name", o.Name);
			o.Slot = (ObjectiveSlot)GUILayout.SelectionGrid((int)o.Slot, System.Enum.GetNames(typeof(ObjectiveSlot)), System.Enum.GetValues(typeof(ObjectiveSlot)).Length);
			o.Index = EditorGUILayout.IntField("Index", o.Index);
			o.Value = EditorGUILayout.FloatField("Value", o.Value);
			o.Reward = EditorGUILayout.IntField("XP", o.Reward);

			GUILayout.Space(30);

			GUILayout.Label("Target", EditorStyles.boldLabel);
			o.Target.Value = EditorGUILayout.IntField("Value", o.Target.Value);
			o.Target.Kind = (ObjectiveKind)EditorGUILayout.EnumPopup("Kind", o.Target.Kind);
			if (o.Target.Kind == ObjectiveKind.HitSpecificAirEnemy)
				o.Target.DetailAir = (AirVariation)EditorGUILayout.EnumPopup("Air", o.Target.DetailAir);
			if (o.Target.Kind == ObjectiveKind.HitSpecificGroundEnemy)
				o.Target.DetailGround = (GroundVariation)EditorGUILayout.EnumPopup("Ground", o.Target.DetailGround);
			if (o.Target.Kind == ObjectiveKind.CollectCollectible)
				o.Target.DetailCollectible = (CollectibleVariation)EditorGUILayout.EnumPopup("Collectible", o.Target.DetailCollectible);
			if (o.Target.Kind == ObjectiveKind.UseConsumable)
				o.Target.DetailConsumable = (ObjectiveRewardType)EditorGUILayout.EnumPopup("Consumable", o.Target.DetailConsumable);

			GUILayout.Space(30);

			GUILayout.Label("Condition", EditorStyles.boldLabel);
			o.Target.Scope = (ObjectiveScope)GUILayout.SelectionGrid((int)o.Target.Scope, System.Enum.GetNames(typeof(ObjectiveScope)), System.Enum.GetValues(typeof(ObjectiveScope)).Length);
			o.Target.Condition = (ObjectiveCondition)EditorGUILayout.EnumPopup("Condition", o.Target.Condition);
			if (o.Target.Condition == ObjectiveCondition.WithCollectible)
				o.Target.DetailCollectible = (CollectibleVariation)EditorGUILayout.EnumPopup("Collectible", o.Target.DetailCollectible);


			GUILayout.Space(30);

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Duplicate"))
			{
				AddObjective(o.Clone());
			}
			if (GUILayout.Button("Auto-Rename"))
			{
				selectedOW.name = MakeNameForObjective(o);
			}
			GUILayout.EndHorizontal();

		}

	}


	//-------------------------------------------------------------------------------------------------
	bool err = false;
	public void Save()
	{
		//check for errors
		err = false;
		AutoRenameAll();
		if (err)
		{
			if (!EditorUtility.DisplayDialog("Error", "Some indices are incorrect", "به کیرم", "چقدر بد! :("))
			{
				return;
			}
		}

		//make json
		var jo = new JSONObject();

		//add objectives
		var ows = SortObjectivesByName();
		foreach (var sceneOW in ows)
		{
			jo.Add(sceneOW.GetComponent<ObjectiveWrapper>().Data.ToJSONObject());
		}

		//save
		var path = Path.Combine(Application.persistentDataPath, "objs.json");
		StreamWriter w = new StreamWriter(path, false);
		w.Write(jo.ToString());
		w.Close();

		if (!EditorUtility.DisplayDialog("Saved", "Saved to " + path, "ریدم به قیافه عنت", "Open Folder"))
		{
			System.Diagnostics.Process.Start(Application.persistentDataPath);
		}
	}

	public void Load()
	{
		var path = Path.Combine(Application.persistentDataPath, "objs.json");
		if (!File.Exists(path))
		{
			return;
		}

		StreamReader r = new StreamReader(path);
		var js = r.ReadToEnd();
		r.Close();

		//make json
		var jo = new JSONObject(js);

		//delete
		var sceneOWs = FindObjectsOfType<ObjectiveWrapper>();
		if (sceneOWs.Any())
		{
			if (EditorUtility.DisplayDialog("Overwrite", "Delete current objectives?", "ها بابا", "گه خوردم!"))
			{
				foreach (var sceneOW in sceneOWs)
				{
					DestroyImmediate(sceneOW.gameObject);
				}
			}
			else
				return;
		}

		//add
		foreach (var j in jo.list)
		{
			var data = new Objective(j);
			AddObjective(data);
		}
	}

	string MakeNameForObjective(Objective o)
	{
		var ows = FindObjectsOfType<ObjectiveWrapper>();
		string errStr = "";
		if (ows.Any(x => x.Data != o && x.Data.Slot == o.Slot && x.Data.Index == o.Index))
		{
			err = true;
			errStr = "ERROR ";
		}
		return string.Format("{6}{0}{5} - {2}({4}) {3} in {1}",
			o.Slot,
			o.Target.Scope,
			o.Target.Kind,
			o.Target.Condition == ObjectiveCondition.None ? "" : o.Target.Condition.ToString(),
			o.Target.Value, o.Index,
			errStr);
	}
	void AutoRenameAll()
	{
		var list = FindObjectsOfType<ObjectiveWrapper>();
		foreach (var item in list)
		{
			item.name = MakeNameForObjective(item.Data);
		}
	}
	Transform[] SortObjectivesByName()
	{
		// Build a list of all the Transforms in this player's hierarchy
		Transform[] children = new Transform[Manager.transform.childCount];
		for (int i = 0; i < children.Length; i++)
			children[i] = Manager.transform.GetChild(i);

		bool sorted = false;
		// Perform a bubble sort on the objects
		while (sorted == false)
		{
			sorted = true;
			for (int i = 0; i < children.Length - 1; i++)
			{
				// Compare the two strings to see which is sooner
				int comparison = compare(children[i].name, children[i + 1].name);

				if (comparison > 0) // 1 means that the current value is larger than the last value
				{
					children[i].transform.SetSiblingIndex(children[i + 1].GetSiblingIndex());
					sorted = false;
				}
			}

			// resort the list to get the new layout
			for (int i = 0; i < children.Length; i++)
				children[i] = Manager.transform.GetChild(i);
		}

		return children;
	}
	int compare(string a, string b)
	{
		try
		{
			int g = a[0].CompareTo(b[0]);
			if (g == 0)
				return System.Convert.ToInt32(a.Substring(1).Split(' ')[0]).CompareTo(System.Convert.ToInt32(b.Substring(1).Split(' ')[0]));
			else
				return g;
		}
		catch
		{
			return 0;
		}
	}
}
