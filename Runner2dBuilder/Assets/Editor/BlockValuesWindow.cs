//using UnityEditor;
//using UnityEngine;

//public class BlockValuesWindow : EditorWindow
//{
//	Block _block;
//	public static void ShowWindow(Block block)
//	{
//		var w = GetWindow<BlockValuesWindow>();
//		w._block = block;
//		w.minSize = new Vector2(200, 250);
//	}
//	void OnGUI()
//	{
//		EditorGUIUtility.labelWidth = 80;
//		GUI.backgroundColor = new Color(0, 0, .5f);

//		if (_block != null && _block.Values != null && _block.Values.Length == 9)
//		{
//			for (int i = 0; i < 9; i++)
//			{
//				GUI.contentColor = new Color(i / 9f, 2f - i / 4f, 1f - i / 9f);
//				_block.Values[i] = EditorGUILayout.Slider("Speed " + (i + 3).ToString(), _block.Values[i], 0, 1);
//				GUILayout.Space(5);
//			}
//		}
//		GUI.contentColor = new Color(0, 1f, 1f);
//		GUI.backgroundColor = new Color(.5f, .7f, 1f);
//		if (GUILayout.Button("Close", GUILayout.Height(40)))
//		{
//			this.Close();
//		}
//	}
//}
