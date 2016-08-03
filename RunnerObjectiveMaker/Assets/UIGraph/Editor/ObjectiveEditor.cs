using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameObject))]
//[CanEditMultipleObjects]
public class ObjectiveEditor : Editor
{
	public void OnSceneGUI()
	{
		Handles.BeginGUI();
		var go = target as GameObject;
		if (go != null)
		{
			if (GUILayout.Button("Add New Objective After", GUILayout.Height(60), GUILayout.Width(200)))
			{
				bool added = false;
				var rt = go.GetComponent<RectTransform>();
				if (rt != null)
				{
					UIGraphUtilities.nextPos = rt.position + Vector3.down * 2;
					var newrt = UIGraphUtilities.CreateObjective();
					ConnectionManager.CreateConnection(newrt, rt);
					added = true;
				}
				if (!added)
					UIGraphUtilities.CreateObjective();
			}


			if (go.GetComponent<ObjectiveManager>() != null)
			{
				if (GUILayout.Button("Serialize", GUILayout.Height(60), GUILayout.Width(200)))
				{
					UIGraphUtilities.SerializeObjectives();
				}
			}
		}
		Handles.EndGUI();
	}
}