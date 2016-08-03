using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HelpWindow : EditorWindow
{
	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/Level Editor Help")]
	public static void ShowWindow()
	{
		GetWindow<HelpWindow>();
	}
	void OnGUI()
	{
		GUILayout.Label("Selection : list of all selected nodes from the selected block", EditorStyles.boldLabel);
		GUILayout.Label("Keep sorted : keep selection sorted (first by x left to right, then by y top to bottom)");
		GUILayout.Space(10);
		GUILayout.Label("o : select only this node, (deselect other nodes)");
		GUILayout.Label("- : remove this node from selection");
		GUILayout.Label("[...] : x of this node (x>0)");
		GUILayout.Label("[...] : y of this node (ground=0 , air=2,4,6)");
		GUILayout.Label("arrows : move this node");
		GUILayout.Label("x : delete this node");
		GUILayout.Space(10);
		GUILayout.Label("Select All : select all nodes in selected block");
		GUILayout.Label("Deselect All : select none");
		GUILayout.Space(10);
		GUILayout.Label("Modify Selection : actions will affect every selected node", EditorStyles.boldLabel);
		GUILayout.Label("arrows : move selected nodes 0.5 pixels");
		GUILayout.Label("upper lane : move selected nodes up 1 lane");
		GUILayout.Label("<<< : move selected nodes left 5 pixels");
		GUILayout.Label(">>> : move selected nodes right 5 pixels");
		GUILayout.Label("lower lane : move selected nodes down 1 lane");

		GUILayout.Label("Copy : copy selected nodes");
		GUILayout.Label("Paste : paste selected nodes after Next Object X");
		GUILayout.Label("Delete : delete selected nodes from this block");
	}
}
