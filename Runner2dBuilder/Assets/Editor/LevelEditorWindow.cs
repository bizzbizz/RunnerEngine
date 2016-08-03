using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using RunnerData;
public class LevelEditorWindow : EditorWindow
{
	#region Window
	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/Level Editor Window")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		var ew = GetWindowWithRect<LevelEditorWindow>(new Rect(0, 0, 200, 600));
		if (ew != null)
			ew.Close();
		GetWindow<LevelEditorWindow>();
	}
	[MenuItem("Window/Load Levels")]
	public static void LoadAll()
	{
		var instance = GetWindow<LevelEditorWindow>();
		instance.Load();
	}
	#endregion

	//---------------------------------------------------------------------------------------------------------------------

	bool isEnabled = true;
	Vector2 lastPos = new Vector2(0, 8);
	Vector2 _scrollPosition;
	Vector2 _scrollPositionb;
	NodeSortMethod _sortMethod = NodeSortMethod.Type_X_Y;

	bool _isGroupTagFilter = false;
	GroupTag _groupTagFilterBy = GroupTag.Fixed;
	bool _isGroupFilter = false;
	Group _groupFilterBy = Group.G1;

	bool _isBlockFilter = false;
	BlockType _blockFilterBy = BlockType.normal;

	GroupTag _selectedGroupTag = 0;
	Group _selectedGroup = 0;

	List<Node> selectedNodes = new List<Node>();

	#region Block
	//---------------------------------------------------------------------------------------------------------------------
	//	Blocks
	//---------------------------------------------------------------------------------------------------------------------
	List<Block> _blocks = new List<Block>();
	Block activeBlock
	{
		get
		{
			refreshBlocks();
			return _blocks.First(x => x.IsSelected);
		}
		set
		{
			foreach (var block in _blocks)
			{
				block.IsSelected = (false);
			}
			if (value != null)
			{
				value.IsSelected = (true);

				lastPos = new Vector2(0, 8);

				//reset groups
				_selectedGroup = Group.G1;
				_selectedGroupTag = GroupTag.Fixed;

				//reset selection
				Selection.instanceIDs = new int[] { };
				_blocks.ForEach(x => x.Nodes.ToList().ForEach(y => EditorUtility.SetDirty(y)));

				//refresh
				Repaint();
			}
		}
	}

	void refreshBlocks(bool createNewIfEmpty = true)
	{
		_blocks = GameObject.FindGameObjectsWithTag("blk").Select(x => x.GetComponent<Block>()).ToList();
		if (!_blocks.Any())
		{
			if (createNewIfEmpty)
				AddBlock();
		}
		else if (_blocks.Count(x => x != null && x.IsSelected) == 1)
		{
			//leave them be
		}
		else
		{
			foreach (var block in _blocks)
			{
				block.IsSelected = false;
			}
			if (_blocks.Any())
				_blocks.First().IsSelected = true;
		}
	}
	void OnSelectionChange()
	{
		Repaint();
	}
	Block AddBlock(bool addDefaultGroup = true)
	{
		var block = Block.CreateNew("block" + _blocks.Count, addDefaultGroup);
		_blocks.Add(block);
		activeBlock = block;
		return block;
	}
	#endregion

	void autoCorrectAll()
	{

		foreach (var block in _blocks)
		{
			if (block == null) continue;
			var children = block.GetComponentsInChildren<Transform>(true);
			foreach (var child in children)
			{
				var node = child.GetComponent<Node>();
				if (node == null)
				{
					//block should not be in another object
					if (child.GetComponent<Block>() != null)
					{
						child.parent = null;
					}
				}
				else
				{
					//flatten inner children
					var childrenOfNode = node.GetComponentsInChildren<Transform>(true);
					foreach (var item in childrenOfNode)
					{
						item.parent = child;
					}
					//reset block reference
					node.Block = block;
					//round position
					if (node.Kind == NodeType.Person)
					{
						node.transform.position = new Vector3(
							Mathf.Max(0, Mathf.Round(2 * node.transform.position.x) / 2),
							0);

					}
					else
					{
						node.transform.position = new Vector3(
							Mathf.Max(0, Mathf.Round(2 * node.transform.position.x) / 2),
							Mathf.Clamp(Mathf.Round(2 * node.transform.position.y) / 2, 2, 8));

					}
				}
			}
		}
	}

	//---------------------------------------------------------------------------------------------------------------------
	#region Style
	Texture u { get { if (_u == null) _u = Resources.Load("u") as Texture; return _u; } }
	Texture d { get { if (_d == null) _d = Resources.Load("d") as Texture; return _d; } }
	Texture l { get { if (_l == null) _l = Resources.Load("l") as Texture; return _l; } }
	Texture r { get { if (_r == null) _r = Resources.Load("r") as Texture; return _r; } }
	Texture _u, _d, _l, _r;
	GUILayoutOption h30 = GUILayout.Height(30);
	GUILayoutOption[] w60h24 = new GUILayoutOption[] { GUILayout.Height(18), GUILayout.Width(120) };
	GUILayoutOption[] w40h16 = new GUILayoutOption[] { GUILayout.Height(16), GUILayout.Width(40) };
	GUILayoutOption[] wh16 = new GUILayoutOption[] { GUILayout.Height(16), GUILayout.Width(16) };
	GUILayoutOption[] w40h30 = new GUILayoutOption[] { GUILayout.Height(30), GUILayout.Width(40) };
	GUIStyle gsTest = new GUIStyle();

	GUIStyle focusedBkg = new GUIStyle();
	GUIStyle evenBkg = new GUIStyle();
	GUIStyle oddBkg = new GUIStyle();
	GUIStyle hdrBkg = new GUIStyle();

	void initGUI()
	{
		gsTest.normal.background = MakeTex(new Color(0, 0, 0, 0.2f));
		focusedBkg.normal.background = MakeTex(new Color(.5f, .75f, 1f, .5f));
		evenBkg.normal.background = MakeTex(new Color(1, 1, 1, .1f));
		oddBkg.normal.background = MakeTex(new Color(0, 0, 0, 0.5f));
		hdrBkg.normal.background = MakeTex(new Color(1, 1, 1, 0.2f));
		hdrBkg.padding = new RectOffset(0, 0, 0, 0);
		hdrBkg.fixedHeight = 16;
		EditorGUIUtility.labelWidth = 85;
	}
	#endregion

	//---------------------------------------------------------------------------------------------------------------------
	void onGUIBlock()
	{

		GUILayout.BeginHorizontal(evenBkg);
		GUILayout.Space(62);
		GUILayout.Label("All Blocks", EditorStyles.boldLabel);
		if (GUILayout.Button("Add New Block", w60h24))
		{
			AddBlock();
		}
		GUILayout.FlexibleSpace();
		_isBlockFilter = EditorGUILayout.BeginToggleGroup("Filter Blocks By", _isBlockFilter);
		_blockFilterBy = (BlockType)EditorGUILayout.EnumPopup(_blockFilterBy);
		EditorGUILayout.EndToggleGroup();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal(GUILayout.Height(110));

		//headers
		GUILayout.BeginVertical(evenBkg);
		GUILayout.Label("Selected");
		GUILayout.Label("Name");
		GUILayout.Label("Class");
		GUILayout.Label("Width");
		//GUILayout.Label("table");
		GUILayout.Label("Delete");
		GUILayout.EndVertical();

		//blocks
		_scrollPositionb = GUILayout.BeginScrollView(_scrollPositionb);
		GUILayout.BeginHorizontal(oddBkg);

		int removingIndex = -1;
		//int duplicatingIndex = -1;

		for (int i = 0; i < _blocks.Count; i++)
		{
			if (_isBlockFilter && _blocks[i].Kind != _blockFilterBy) continue;

			GUILayout.BeginVertical(GUILayout.Width(40));
			var active = GUILayout.Toggle(_blocks[i].IsSelected, _blocks[i].name);
			_blocks[i].gameObject.name = EditorGUILayout.TextField(_blocks[i].gameObject.name);
			_blocks[i].Kind = (BlockType)EditorGUILayout.EnumPopup(_blocks[i].Kind);
			_blocks[i].Width = EditorGUILayout.IntField(_blocks[i].Width);
			//if (GUILayout.Button("..."))
			//{
			//	BlockValuesWindow.ShowWindow(_blocks[i]);
			//}
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("x"))
			{
				//mark for delete
				removingIndex = i;
			}
			//if (GUILayout.Button("+"))
			//{
			//	//mark for duplicate
			//	duplicatingIndex = i;
			//}
			GUILayout.EndHorizontal();

			if (active && !_blocks[i].IsSelected)
			{
				activeBlock = _blocks[i];
			}
			GUILayout.EndVertical();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndScrollView();
		GUILayout.EndHorizontal();

		//delete block
		if (removingIndex != -1)
		{
			if (EditorUtility.DisplayDialog("Delete?", "Are you sure?", "Delete", "Cancel"))
			{
				var b = _blocks[removingIndex];
				if (activeBlock == b)
					activeBlock = null;
				_blocks.RemoveAt(removingIndex);
				removingIndex = -1;
				DestroyImmediate(b.gameObject);
			}
		}
		//duplicate block
		//if (duplicatingIndex != -1)
		//{
		//	var b = _blocks[duplicatingIndex];
		//	var newB = AddBlock();
		//	newB.CopyFrom(b);
		//	_blocks.Insert(duplicatingIndex, newB);
		//	duplicatingIndex = -1;
		//}
	}

	//---------------------------------------------------------------------------------------------------------------------
	void onGUIAddNode()
	{
		GUILayout.BeginVertical(oddBkg);
		GUILayout.Label("Add Node", EditorStyles.boldLabel);

		EditorGUILayout.BeginHorizontal();
		//add node buttons
		for (int i = 0; i < System.Enum.GetValues(typeof(NodeType)).Length; i++)
		{
			if (GUILayout.Button(((NodeType)i).ToString(), GUILayout.Height(24)))
			{
				var newNode = activeBlock.AddNode(i, 0, lastPos, _selectedGroup, _selectedGroupTag);
				while (true)
				{
					var overlap = activeBlock.FirstOverlappedNode(newNode);
					if (overlap != null)
					{
						newNode.transform.position += Vector3.right;
					}
					else break;
				}
				lastPos = newNode.transform.position;
				var list = Selection.instanceIDs.ToList();
				list.Add(newNode.gameObject.GetInstanceID());
				Selection.instanceIDs = list.ToArray();
			}
		}
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(5);

		EditorGUILayout.BeginHorizontal(evenBkg);
		//start
		lastPos = EditorGUILayout.Vector2Field("start next node at:", lastPos, GUILayout.Width(120));
		lastPos = new Vector2(Mathf.Round(lastPos.x * 2) / 2, Mathf.Round(lastPos.y * 2) / 2);
		if (GUILayout.Button("reset\nstart", GUILayout.Width(40), GUILayout.Height(30)))
			lastPos = new Vector2(0, 8);

		GUILayout.Space(15);

		//group
		GUILayout.BeginVertical();
		_selectedGroupTag = (GroupTag)EditorGUILayout.EnumPopup("Current Tag:", _selectedGroupTag);
		_selectedGroup = (Group)EditorGUILayout.EnumPopup("Current Group:", _selectedGroup);
		GUILayout.EndVertical();

		EditorGUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}

	//---------------------------------------------------------------------------------------------------------------------
	void onGUISelectedNodes()
	{
		//title
		GUILayout.BeginHorizontal();
		GUILayout.Label("Selection", EditorStyles.boldLabel);
		GUILayout.FlexibleSpace();
		//filter by group
		GUILayout.BeginHorizontal(evenBkg);
		_isGroupTagFilter = EditorGUILayout.BeginToggleGroup("Filter By GroupTag", _isGroupTagFilter);
		GUILayout.BeginHorizontal();
		_groupTagFilterBy = (GroupTag)EditorGUILayout.EnumPopup(_groupTagFilterBy);
		if (GUILayout.Button("Select All"))
		{
			Selection.instanceIDs = activeBlock.Nodes.Where(x => x.GroupTag == _groupTagFilterBy).Select(x => x.gameObject.GetInstanceID()).ToArray();
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndToggleGroup();
		EditorGUILayout.EndHorizontal();
		//sort
		_sortMethod = (NodeSortMethod)EditorGUILayout.EnumPopup("Sort Method", _sortMethod, GUILayout.Width(200));
		GUILayout.EndHorizontal();

		bool even = false;
		if (Event.current.type == EventType.Layout)
		{
			//list selected nodes
			selectedNodes = Selection.gameObjects.Select(x => x.GetComponent<Node>()).Where(x => x != null).ToList();
			if (selectedNodes.Any())
			{
				//filter by block
				Selection.instanceIDs = selectedNodes.Where(x => x.Block == activeBlock).Select(x => x.gameObject.GetInstanceID()).ToArray();
				//filter by group
				if (_isGroupTagFilter)
				{
					selectedNodes = selectedNodes.Where(x => x.GroupTag == _groupTagFilterBy).ToList();
				}
				//sort by x,y
				switch (_sortMethod)
				{
					case NodeSortMethod.Type_X_Y:
						selectedNodes.Sort(new TXYComparer());
						break;
					case NodeSortMethod.Type_Variation:
						selectedNodes.Sort(new TVComparer());
						break;
					case NodeSortMethod.X_Y:
						selectedNodes.Sort(new XYComparer());
						break;
					default:
						break;
				}
			}
			else
			{
				//no nodes selected
				if (Selection.gameObjects.Length == 1)
				{
					var blk = Selection.gameObjects.First().GetComponent<Block>();
					if (blk != null)
						activeBlock = blk;
				}
			}
		}

		//backup padding
		var backupPadding = GUI.skin.button.padding;
		GUI.skin.button.padding = new RectOffset(0, 0, 0, 0);

		//header
		GUILayout.BeginHorizontal(hdrBkg);
		GUILayout.Space(48);
		GUILayout.Label("tag           group        x         y          type             variation    qty    dist    ss    sx    sy     probability");
		GUILayout.EndHorizontal();

		//selection nodes
		_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
		foreach (var node in selectedNodes)
		{
			if (node.tag == "dyn")
			{
				//line colors
				if (node.IsFocused)
					GUI.contentColor = Color.cyan;
				GUILayout.BeginHorizontal(even ? evenBkg : oddBkg);
				even = !even;

				//buttons
				//o
				if (GUILayout.Button("o", wh16))
				{
					//select
					Selection.instanceIDs = new int[] { node.gameObject.GetInstanceID() };
					break;
				}
				//-
				if (GUILayout.Button("-", wh16))
				{
					//remove from selection
					var list = Selection.instanceIDs.ToList();
					list.Remove(node.gameObject.GetInstanceID());
					Selection.instanceIDs = list.ToArray();
				}

				//groups
				node.GroupTag = (GroupTag)EditorGUILayout.EnumPopup(node.GroupTag, GUILayout.Width(60));
				if (node.GroupTag == GroupTag.Fixed)
					GUILayout.Space(62);
				else
					node.Group = (Group)EditorGUILayout.EnumPopup(node.Group, GUILayout.Width(60));

				//x,y
				var pos = node.transform.position;
				pos.x = EditorGUILayout.FloatField(node.transform.position.x, w40h16);
				if (node.Kind == NodeType.Person)
				{
					GUILayout.Space(44);
				}
				else
				{
					pos.y = EditorGUILayout.FloatField(node.transform.position.y, w40h16);
				}
				node.transform.position = pos;

				//kind,variation
				node.Kind = (NodeType)EditorGUILayout.EnumPopup(node.Kind, GUILayout.Width(80));
				switch (node.Kind)
				{
					case NodeType.Collectible:
						node.Variation = (int)(CollectibleVariation)EditorGUILayout.EnumPopup((CollectibleVariation)node.Variation, GUILayout.Width(60));
						break;
					case NodeType.Food:
						node.Variation = (int)(FoodVariation)EditorGUILayout.EnumPopup((FoodVariation)node.Variation, GUILayout.Width(60));
						break;
					case NodeType.Person:
						node.Variation = (int)(PersonVariation)EditorGUILayout.EnumPopup((PersonVariation)node.Variation, GUILayout.Width(60));
						break;
					case NodeType.Eagle:
						node.Variation = (int)(EagleVariation)EditorGUILayout.EnumPopup((EagleVariation)node.Variation, GUILayout.Width(60));
						break;
					case NodeType.Coin:
					default:
						GUILayout.Space(64);
						break;
				}

				//qty, dist, main
				if (node.Kind == NodeType.Coin)
				{
					node.Quantity = EditorGUILayout.IntField(node.Quantity, GUILayout.Width(30));
					node.Distance = EditorGUILayout.FloatField(node.Distance, GUILayout.Width(30));
					node.SS = EditorGUILayout.FloatField(node.SS / 1.57f, GUILayout.Width(30)) * 1.57f;
					node.SX = EditorGUILayout.FloatField(node.SX * 10, GUILayout.Width(30))/10;
					node.SY = EditorGUILayout.FloatField(node.SY, GUILayout.Width(30));
				}
				else if (node.Kind == NodeType.Person)
				{
					//main person
					node.IsMainTarget = GUILayout.Toggle(node.IsMainTarget, "Main", GUILayout.Width(166));
				}
				else
					GUILayout.Space(170);
				node.Probability = EditorGUILayout.Slider(node.Probability, 0f, 1f);

				//u,d,l,r
				if (node.Kind == NodeType.Person)
				{
					GUILayout.Space(50);
				}
				else
				{
					//up,down
					GUILayout.Space(10);
					if (GUILayout.Button(u, wh16))
					{
						selectedNodes.ForEach(x => x.IsFocused = false);
						node.IsFocused = true;
						node.transform.position += new Vector3(0, .5f, 0);
					}
					if (GUILayout.Button(d, wh16))
					{
						selectedNodes.ForEach(x => x.IsFocused = false);
						node.IsFocused = true;
						node.transform.position -= new Vector3(0, .5f, 0);
					}
				}
				//left, right
				if (GUILayout.Button(l, wh16))
				{
					selectedNodes.ForEach(x => x.IsFocused = false);
					node.IsFocused = true;
					node.transform.position -= new Vector3(.5f, 0, 0);
				}
				if (GUILayout.Button(r, wh16))
				{
					selectedNodes.ForEach(x => x.IsFocused = false);
					node.IsFocused = true;
					node.transform.position += new Vector3(.5f, 0, 0);
				}
				if (GUILayout.Button("X", wh16))
				{
					var list = Selection.instanceIDs.ToList();
					list.Remove(node.gameObject.GetInstanceID());
					Selection.instanceIDs = list.ToArray();
					DestroyImmediate(node.gameObject);
				}

				GUILayout.EndHorizontal();

				//node.IsHover = Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);

				GUI.contentColor = Color.white;

			}
		}
		EditorGUILayout.EndScrollView();

		//selection buttons
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Select All", h30)) { Selection.instanceIDs = activeBlock.GetComponentsInChildren<Node>().Select(x => x.gameObject.GetInstanceID()).ToArray(); }
		if (GUILayout.Button("Deselect All", h30)) { Selection.instanceIDs = new int[] { }; }
		if (GUILayout.Button("Delete Selected", h30))
		{
			if (EditorUtility.DisplayDialog("Delete", "Are you sure?", "Delete", "Cancel"))
			{
				Selection.gameObjects.ToList().ForEach(x => DestroyImmediate(x));
				Selection.instanceIDs = new int[] { };
			}
		}
		EditorGUILayout.EndHorizontal();

		GUI.skin.button.padding = backupPadding;

	}


	//---------------------------------------------------------------------------------------------------------------------
	//	Unity Editor Methods
	//---------------------------------------------------------------------------------------------------------------------
	void Update()
	{
		autoCorrectAll();
	}
	void OnGUI()
	{
		initGUI();

		if (!isEnabled)
		{
			isEnabled = GUILayout.Toggle(isEnabled, "Enabled");
			return;
		}
		//try
		{
			//init
			refreshBlocks();

			GUI.SetNextControlName("EMPTY");
			GUILayout.TextField("", GUILayout.Width(0), GUILayout.Height(0));

			EditorGUILayout.BeginHorizontal();
			isEnabled = GUILayout.Toggle(isEnabled, "Enabled");
			GUI.backgroundColor = new Color(0f, 1f, .9f);
			if (GUILayout.Button("Save", GUILayout.Height(30)))
			{
				Save();
			}
			EditorGUILayout.EndHorizontal();

			GUI.backgroundColor = new Color(1f, .6f, .6f);
			onGUIBlock();

			GUI.backgroundColor = new Color(.6f, 1f, .8f);
			onGUIAddNode();

			GUILayout.Space(10);

			GUI.backgroundColor = new Color(.6f, .8f, 1f);
			onGUISelectedNodes();

		}
		//catch (System.Exception ex)
		//{
		//	GUILayout.Label("ERROR");
		//	GUILayout.Label(ex.Message);
		//	var ww = new GUIStyle();
		//	ww.wordWrap = true;
		//	GUILayout.Label(ex.StackTrace, ww);
		//}
	}
	private Texture2D MakeTex(Color col)
	{
		Color[] pix = new Color[1];

		pix[0] = col;

		Texture2D result = new Texture2D(1, 1);
		result.SetPixels(pix);
		result.Apply();

		return result;
	}


	//---------------------------------------------------------------------------------------------------------------------
	//	Save / Load
	//---------------------------------------------------------------------------------------------------------------------
	private void Save()
	{
		foreach (var block in _blocks)
		{
			var overlappedNode = block.FirstOverlappedNode();
			if (overlappedNode != null)
			{
				if (EditorUtility.DisplayDialog("Warning", "Was NOT Saved!\n" + block.name + " has some overlapped nodes at " + (Vector2)overlappedNode.transform.position
					+ "\n node type = " + overlappedNode, "Select it!", "Cancel"))
				{
					if (activeBlock != overlappedNode.Block)
						activeBlock = overlappedNode.Block;

					Selection.instanceIDs = new int[] { overlappedNode.gameObject.GetInstanceID() };
				}
				return;
			}
		}

		//make json
		var jo = new JSONObject();
		foreach (var block in _blocks.Where(x => x.Nodes.Any()))
		{
			var bjo = new JSONObject();

			//make nodes
			var nodes = new JSONObject();
			foreach (var node in block.Nodes)
			{
				var nbjo = new JSONObject();
				nbjo.AddField("kind", (int)node.Kind);
				nbjo.AddField("variation", node.Variation);
				nbjo.AddField("x", node.transform.position.x);
				nbjo.AddField("y", node.transform.position.y);
				nbjo.AddField("group", (int)node.Group);
				nbjo.AddField("tag", (int)node.GroupTag);

				nbjo.AddField("prob", node.Probability);
				//person
				nbjo.AddField("main", node.IsMainTarget);
				//coin
				nbjo.AddField("qty", node.Quantity);
				nbjo.AddField("dist", node.Distance);
				nbjo.AddField("ss", node.SS);
				nbjo.AddField("sx", node.SX);
				nbjo.AddField("sy", node.SY);

				nodes.Add(nbjo);
			}

			//add block info
			bjo.AddField("name", block.gameObject.name);
			bjo.AddField("kind", (int)block.Kind);
			bjo.AddField("nodes", nodes);
			bjo.AddField("width", block.Width);

			jo.Add(bjo);
		}

		//save
		var path = System.IO.Path.Combine(Application.persistentDataPath, "Levels.json");
		System.IO.StreamWriter w = new System.IO.StreamWriter(path, false);
		w.Write(jo.ToString());
		w.Close();

		if (!EditorUtility.DisplayDialog("Saved", "Saved to " + path, "Awesome!", "Open Folder"))
		{
			System.Diagnostics.Process.Start(Application.persistentDataPath);
		}
	}


	//---------------------------------------------------------------------------------------------------------------------
	public void Load()
	{
		refreshBlocks(false);
		if (_blocks.Any())
		{
			if (_blocks.Any(x => x.Nodes.Any()))
			{
				if (!EditorUtility.DisplayDialog("Overwrite", "Delete Existing Level data?", "Keep existing", "Delete existing"))
				{
					selectedNodes.Clear();
					Selection.instanceIDs = new int[] { };
					_blocks.ToList().ForEach(x => DestroyImmediate(x.gameObject));
					_blocks.Clear();
				}
			}
			else
			{
				_blocks.ToList().ForEach(x => DestroyImmediate(x.gameObject));
				_blocks.Clear();
			}
		}
		//load
		var path = System.IO.Path.Combine(Application.persistentDataPath, "Levels.json");
		System.IO.StreamReader r = new System.IO.StreamReader(path);
		var js = r.ReadToEnd();
		r.Close();

		//make json
		var jo = new JSONObject(js);
		foreach (var bjo in jo.list)
		{
			//add block and weight
			var block = AddBlock(false);
			block.gameObject.name = bjo.GetField("name").str;
			block.Width = (int)bjo.GetField("width").f;
			block.Kind = (BlockType)((int)bjo.GetField("kind").f);

			//add nodes
			var list = bjo.GetField("nodes").list;
			foreach (var item in list)
			{
				//node basics
				var kind = (int)item.GetField("kind").f;
				var variation = (int)item.GetField("variation").f;
				var x = item.GetField("x").f;
				var y = item.GetField("y").f;
				var pos = new Vector3(x, y);
				var group = (Group)(int)item.GetField("group").f;
				var tag = (GroupTag)(int)item.GetField("tag").f;
				var node = block.AddNode(kind, variation, pos, group, tag);

				node.Probability = item.GetField("prob").f;
				//person
				node.IsMainTarget = item.GetField("main").b;
				//coin
				node.Quantity = (int)item.GetField("qty").f;
				node.Distance = item.GetField("dist").f;
				node.SS = item.GetField("ss").f;
				node.SX = item.GetField("sx").f;
				node.SY = item.GetField("sy").f;
			}
		}
	}
}