using RunnerData;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;

public class LevelEditorWindow : EditorWindow
{
	#region Window
	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/Level Maker Window")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		//var ew = GetWindowWithRect<BlockMakerWindow>(new Rect(0, 0, 200, 600));
		//if (ew != null)
		//	ew.Close();
		GetWindow<LevelEditorWindow>();

	}
	#endregion


	//---------------------------------------------------------------------------------------------------------------------

	bool isEnabled = true;

	GameDb _db;
	GameDb Db
	{
		get
		{
			//init objects
			if (_db == null)
			{
				_db = FindObjectOfType<GameDb>();
				if (_db == null)
					_db = new GameObject(".:[Levels]:.").AddComponent<GameDb>();
			}
			return _db;
		}
	}

	List<Block> _blocks = new List<Block>();
	List<Node> selectedNodes = new List<Node>();

	int selectedMajorIndex = 0;
	Major selectedMajor;

	Vector2 levelsScroll;

	Vector2 lastPos = new Vector2(0, 8);
	Vector2 _scrollPosition;
	Vector2 _scrollPositionb;

	NodeSortMethod _sortMethod = NodeSortMethod.Type_X_Y;

	bool _sortBlocks = true;

	GroupTag _selectedGroupTag = 0;
	Group _selectedGroup = 0;

	bool _isBlockFilter = false;
	BlockType _blockFilterBy = BlockType.free;

	bool _isGroupTagFilter = false;
	GroupTag _groupTagFilterBy = GroupTag.Fixed;

	bool _isGizmoGroupFilter = false;
	Group _gizmoGroupFilterBy = Group.G1;
	bool _isGizmoGroupTagFilter = false;
	GroupTag _gizmoGroupTagFilterBy = GroupTag.Fixed;



	#region Block
	//---------------------------------------------------------------------------------------------------------------------
	//	Blocks
	//---------------------------------------------------------------------------------------------------------------------
	Block AddBlock()
	{
		var block = Block.CreateNew(string.Format("block{0:00}", _blocks.Count));
		_blocks.Add(block);
		activeBlock = block;
		correctBlockNumber(block);
		return block;
	}
	private void DeleteBlock(int idx)
	{
		var b = _blocks[idx];
		if (activeBlock == b)
			activeBlock = null;
		_blocks.RemoveAt(idx);
		DestroyImmediate(b.gameObject);
	}
	Block activeBlock
	{
		get
		{
			refreshBlocks();
			return _blocks.FirstOrDefault(x => x.IsSelected);
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
				UpdateCanDrawAllNodes();

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

	void refreshBlocks()
	{
		//reload
		_blocks.Clear();
		var list = FindObjectsOfType<Block>();
		foreach (var item in list)
		{
			_blocks.Add(item);
			Db.CountBlockUsage(item);
		}

		if (!_blocks.Any())
		{
			return;
		}

		//sort
		if (_sortBlocks)
			_blocks.Sort(new BlockNameComparer());

		//select one
		if (_blocks.Count(x => x != null && x.IsSelected) != 1)
		{
			foreach (var block in _blocks)
			{
				if (block != null)
					block.IsSelected = false;
			}
		}

		//correct value
		foreach (var block in _blocks)
		{
			checkCorrectBlockNumber(block);
		}
	}
	void checkCorrectBlockNumber(Block block)
	{
		var sameKindBlocks = _blocks.Where(x => x != block && x.Kind == block.Kind);
		if (!sameKindBlocks.Any())
		{
			block.WrongNumber = false;
			return;
		}
		block.WrongNumber = sameKindBlocks.Any(x => x.Number == block.Number);
	}
	void correctBlockNumber(Block block)
	{
		var sameKindBlocks = _blocks.Where(x => x != block && x.Kind == block.Kind);
		if (!sameKindBlocks.Any())
		{
			return;
		}
		int val = 0;
		while (sameKindBlocks.Any(x => x.Number == val))
		{
			val++;
		}
		block.Number = val;
		GUI.FocusControl("EMPTY");
	}
	void OnSelectionChange()
	{
		Repaint();
	}

	public void UpdateCanDrawAllNodes()
	{
		if (activeBlock == null) return;

		foreach (var node in activeBlock.Nodes)
		{
			if (_isGizmoGroupFilter && _isGizmoGroupTagFilter)
				node.CanDraw = _gizmoGroupTagFilterBy == node.GroupTag && _gizmoGroupFilterBy == node.Group;
			else if (_isGizmoGroupTagFilter)
				node.CanDraw = _gizmoGroupTagFilterBy == node.GroupTag;
			else if (_isGizmoGroupFilter)
				node.CanDraw = _gizmoGroupFilterBy == node.Group;
			else
				node.CanDraw = true;
		}
	}
	#endregion



	//---------------------------------------------------------------------------------------------------------------------
	bool _showNumbersInfo = false;
	void onGUILevels()
	{
		if (Db.Majors == null)
		{
			Db.Init();
		}
		float width = 30;

		//usage statistics
		_showNumbersInfo = EditorGUILayout.BeginToggleGroup("Show Numbers", _showNumbersInfo);

		//numbers
		if (_showNumbersInfo)
		{
			GUILayout.BeginHorizontal();

			//n/a
			GUILayout.BeginVertical(GUILayout.Width(width));
			GUILayout.Label("N/A");
			GUILayout.Label(Db.GetBlockTypeUsageCount().ToString());
			GUILayout.EndVertical();

			//other types
			foreach (var type in Enum.GetValues(typeof(BlockType)))
			{
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical(Helper.evenBkg, GUILayout.Width(width));

				//hdr
				GUILayout.BeginVertical(Helper.oddBkg);
				GUILayout.Label(type.ToString());
				GUILayout.Label(Db.GetBlockTypeUsageCount((BlockType)type).ToString());
				GUILayout.EndVertical();

				var kind = (BlockType)type;
				var nums = _blocks.Where(x => x.Kind == kind).GroupBy(x => x.Number).Select(x => x.First().Number).ToArray();
				var list = Db.GetBlockTypeNumbersWithUsage((BlockType)type, nums);
				foreach (var item in list)
				{
					GUILayout.Label(item);
				}
				GUILayout.EndVertical();
			}

			GUILayout.EndHorizontal();
		}
		else
		{
			//types
			GUILayout.BeginHorizontal(Helper.oddBkg);
			//n/a
			GUILayout.BeginVertical(GUILayout.Width(width));
			GUILayout.Label("N/A");
			GUILayout.Label(Db.GetBlockTypeUsageCount().ToString());
			GUILayout.EndVertical();
			//other types
			foreach (var type in Enum.GetValues(typeof(BlockType)))
			{
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical(GUILayout.Width(width));
				GUILayout.Label(type.ToString());
				GUILayout.Label(Db.GetBlockTypeUsageCount((BlockType)type).ToString());
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}
		EditorGUILayout.EndToggleGroup();


		//majors
		var majorNames = Db.Majors.Select(x => x.Speed.ToString("0.#")).ToArray();
		selectedMajorIndex = GUILayout.SelectionGrid(selectedMajorIndex, majorNames, Db.Majors.Length, GUILayout.Height(40));
		Db.SelectedIndex = selectedMajorIndex;

		if (selectedMajorIndex >= Db.Majors.Length)
			return;

		selectedMajor = Db.Majors[selectedMajorIndex];
		if (selectedMajor == null) return;

		//labels
		GUILayout.BeginHorizontal(Helper.hdrBkg);
		GUILayout.Label(" # ");
		GUILayout.Label("Space");
		GUILayout.FlexibleSpace();
		GUILayout.Label("Kind");
		GUILayout.FlexibleSpace();
		GUILayout.Label("Number");
		GUILayout.EndHorizontal();

		//minors
		levelsScroll = GUILayout.BeginScrollView(levelsScroll);
		for (int i = 0; i < selectedMajor.Minors.Length; i++)
		{
			if (selectedMajor.Minors[i] == null)
			{
				selectedMajor.Minors[i] = new Minor();
			}

			GUILayout.BeginHorizontal(i % 2 == 0 ? Helper.evenBkg : Helper.oddBkg);
			GUILayout.Label((i + 1).ToString());
			selectedMajor.Minors[i].Space = EditorGUILayout.IntField(selectedMajor.Minors[i].Space);
			GUILayout.FlexibleSpace();
			selectedMajor.Minors[i].Kind = (BlockType)EditorGUILayout.EnumPopup(selectedMajor.Minors[i].Kind);
			GUILayout.FlexibleSpace();
			selectedMajor.Minors[i].Number = EditorGUILayout.IntField(selectedMajor.Minors[i].Number);

			//find minor's block
			selectedMajor.Minors[i].Block = _blocks.FirstOrDefault(x => x.Kind == selectedMajor.Minors[i].Kind && x.Number == selectedMajor.Minors[i].Number);
			if (selectedMajor.Minors[i].Block == null)
			{
				GUILayout.Label("N/A", Helper.errBkg);
			}
			else
			{
				GUILayout.Label("OK.", Helper.okBkg);
			}
			if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.MinHeight(16)))
			{
				var tmp = selectedMajor.Minors.ToList();
				tmp.Remove(selectedMajor.Minors[i]);
				selectedMajor.Minors = tmp.ToArray();
				tmp.Clear();
				tmp = null;
			}
			GUILayout.EndHorizontal();
		}

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Add"))
		{
			var tmp = selectedMajor.Minors.ToList();
			tmp.Add(new Minor());
			selectedMajor.Minors = tmp.ToArray();
			tmp.Clear();
			tmp = null;
		}
		GUILayout.EndHorizontal();


		GUILayout.EndScrollView();
	}
	//---------------------------------------------------------------------------------------------------------------------
	void onGUIBlock()
	{
		GUILayout.BeginHorizontal(Helper.evenBkg);
		GUILayout.Space(62);
		GUILayout.Label("All Blocks", EditorStyles.boldLabel);
		if (GUILayout.Button("Add New Block", Helper.w60h24))
		{
			AddBlock();
		}
		_sortBlocks = EditorGUILayout.Toggle("sort|edit names", _sortBlocks);
		GUILayout.FlexibleSpace();
		_isBlockFilter = EditorGUILayout.BeginToggleGroup("Filter Blocks By", _isBlockFilter);
		_blockFilterBy = (BlockType)EditorGUILayout.EnumPopup(_blockFilterBy);
		EditorGUILayout.EndToggleGroup();
		GUILayout.EndHorizontal();


		GUILayout.BeginHorizontal(GUILayout.Height(_sortBlocks ? 125 : 140));

		//headers
		GUILayout.BeginVertical(Helper.evenBkg);
		GUILayout.Label("Selected");
		if (!_sortBlocks)
			GUILayout.Label("Name");
		GUILayout.Label("Class");
		GUILayout.Label("Width");
		GUILayout.Label("Number");
		//GUILayout.Label("table");
		GUILayout.Label("Dup/Del");
		GUILayout.Label("Used#");
		GUILayout.EndVertical();

		//blocks
		_scrollPositionb = GUILayout.BeginScrollView(_scrollPositionb);
		GUILayout.BeginHorizontal(Helper.oddBkg);

		int removingBlockIndex = -1;
		Block duplicatingBlock = null;

		float labelwidthbackup = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 10;

		for (int i = 0; i < _blocks.Count; i++)
		{
			if (_isBlockFilter && _blocks[i].Kind != _blockFilterBy) continue;

			GUILayout.BeginVertical(GUILayout.Width(40));
			var active = GUILayout.Toggle(_blocks[i].IsSelected, _blocks[i].name);
			if (!_sortBlocks)
				_blocks[i].gameObject.name = EditorGUILayout.TextField(_blocks[i].gameObject.name);
			_blocks[i].Kind = (BlockType)EditorGUILayout.EnumPopup(_blocks[i].Kind);
			_blocks[i].Width = EditorGUILayout.IntField(_blocks[i].Width);

			//number
			if (_sortBlocks)
			{
				if (_blocks[i].WrongNumber)
					EditorGUILayout.LabelField(_blocks[i].Number.ToString(), Helper.errBkg);
				else
					EditorGUILayout.LabelField(_blocks[i].Number.ToString());
			}
			else
			{
				if (_blocks[i].WrongNumber)
				{
					GUILayout.BeginHorizontal();
					_blocks[i].Number = EditorGUILayout.IntField(_blocks[i].Number, Helper.errBkg);
					if (GUILayout.Button("Auto"))
					{
						correctBlockNumber(_blocks[i]);
					}
					GUILayout.EndHorizontal();
				}
				else
				{
					_blocks[i].Number = EditorGUILayout.IntField(_blocks[i].Number);
				}
			}

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("+"))
			{
				//mark for duplicate
				duplicatingBlock = _blocks[i];
			}
			if (GUILayout.Button("x"))
			{
				//mark for delete
				removingBlockIndex = i;
			}
			GUILayout.EndHorizontal();

			//used#
			if (_blocks[i].UsedCount == 0)
				EditorGUILayout.LabelField(_blocks[i].UsedCount.ToString(), Helper.errBkg);
			else
				EditorGUILayout.LabelField(_blocks[i].UsedCount.ToString());

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

		EditorGUIUtility.labelWidth = labelwidthbackup;

		//delete block
		if (removingBlockIndex != -1)
		{
			if (EditorUtility.DisplayDialog("Delete?", "Are you sure?", "Delete", "Cancel"))
			{
				DeleteBlock(removingBlockIndex);
				removingBlockIndex = -1;
			}
		}
		//duplicate block
		if (duplicatingBlock != null)
		{
			//create and select new block
			Block newB = AddBlock();

			//copy nodes from old block
			newB.CopyFrom(duplicatingBlock);

			checkCorrectBlockNumber(newB);
			duplicatingBlock = null;
		}
	}


	//---------------------------------------------------------------------------------------------------------------------
	void onGUIAddNode()
	{
		GUILayout.BeginVertical(Helper.oddBkg);
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

		EditorGUILayout.BeginHorizontal(Helper.evenBkg);
		//start
		lastPos = EditorGUILayout.Vector2Field("start next node at:", lastPos, GUILayout.Width(120));
		lastPos = new Vector2(Mathf.Round(lastPos.x * 2) / 2, Mathf.Round(lastPos.y * 4) / 4);
		if (GUILayout.Button("reset\nstart", GUILayout.Width(40), GUILayout.Height(30)))
			lastPos = new Vector2(0, 8);

		GUILayout.Space(15);

		//group
		GUILayout.BeginVertical(GUILayout.Width(250));
		_selectedGroupTag = (GroupTag)EditorGUILayout.EnumPopup("Current Tag:", _selectedGroupTag);
		_selectedGroup = (Group)EditorGUILayout.EnumPopup("Current Group:", _selectedGroup);
		GUILayout.EndVertical();

		EditorGUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}
	GroupTag allGroupTag;
	Group allGroup;
	void onGUIChangeAll()
	{
		EditorGUILayout.BeginVertical(Helper.oddBkg);
		//altogether
		//groups
		GUILayout.Label("Change All", EditorStyles.boldLabel);
		allGroupTag = (GroupTag)EditorGUILayout.EnumPopup(allGroupTag, GUILayout.Width(60));
		if (allGroupTag != GroupTag.Fixed)
			allGroup = (Group)EditorGUILayout.EnumPopup(allGroup, GUILayout.Width(60));
		if (GUILayout.Button("Apply"))
		{
			foreach (var node in selectedNodes)
			{
				node.GroupTag = allGroupTag;
				node.Group = allGroup;
			}
		}
		EditorGUILayout.EndVertical();
	}

	//---------------------------------------------------------------------------------------------------------------------
	void onGUIGizmoFilter()
	{
		//gizmo filter
		GUI.backgroundColor = Color.gray;
		GUILayout.BeginVertical(Helper.evenBkg, GUILayout.MinWidth(200));

		GUILayout.Label("Filter Scene Objects", EditorStyles.boldLabel);

		_isGizmoGroupTagFilter = EditorGUILayout.BeginToggleGroup("Filter Tags", _isGizmoGroupTagFilter);
		_gizmoGroupTagFilterBy = (GroupTag)EditorGUILayout.EnumPopup(_gizmoGroupTagFilterBy);
		EditorGUILayout.EndToggleGroup();

		_isGizmoGroupFilter = EditorGUILayout.BeginToggleGroup("Filter Groups", _isGizmoGroupFilter);
		_gizmoGroupFilterBy = (Group)EditorGUILayout.EnumPopup(_gizmoGroupFilterBy);
		EditorGUILayout.EndToggleGroup();

		GUILayout.EndVertical();

		//update can draw on all nodes
		UpdateCanDrawAllNodes();
	}

	//---------------------------------------------------------------------------------------------------------------------
	void onGUISelectedNodes()
	{
		//title
		GUILayout.BeginHorizontal();
		GUILayout.Label("Selection", EditorStyles.boldLabel);
		GUILayout.FlexibleSpace();
		//filter by group
		GUILayout.BeginHorizontal(Helper.evenBkg);
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
		GUILayout.BeginHorizontal(Helper.hdrBkg);
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
				GUILayout.BeginHorizontal(even ? Helper.evenBkg : Helper.oddBkg);
				even = !even;

				//buttons
				//o
				if (GUILayout.Button("o", Helper.wh16))
				{
					//select
					Selection.instanceIDs = new int[] { node.gameObject.GetInstanceID() };
					break;
				}
				//-
				if (GUILayout.Button("-", Helper.wh16))
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
				pos.x = EditorGUILayout.FloatField(node.transform.position.x, Helper.w40h16);
				if (node.Kind == NodeType.GroundEnemy)
				{
					GUILayout.Space(44);
				}
				else
				{
					pos.y = EditorGUILayout.FloatField(node.transform.position.y, Helper.w40h16);
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
					case NodeType.GroundEnemy:
						node.Variation = (int)(GroundVariation)EditorGUILayout.EnumPopup((GroundVariation)node.Variation, GUILayout.Width(60));
						break;
					case NodeType.AirEnemy:
						node.Variation = (int)(AirGroupVariation)EditorGUILayout.EnumPopup((AirGroupVariation)node.Variation, GUILayout.Width(60));
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
					node.SX = EditorGUILayout.FloatField(node.SX * 10, GUILayout.Width(30)) / 10;
					node.SY = EditorGUILayout.FloatField(node.SY, GUILayout.Width(30));
					node.Probability = Mathf.Round(EditorGUILayout.Slider(node.Probability, 0f, 1f) * 10) / 10f;
				}
				else if (node.Kind == NodeType.GroundEnemy)
				{
					//main person
					node.IsMainTarget = GUILayout.Toggle(node.IsMainTarget, "Main", GUILayout.Width(166));
					node.Probability = Mathf.Round(EditorGUILayout.Slider(node.Probability, 0f, 1f) * 10) / 10f;
				}
				else if (node.Kind == NodeType.Collectible)
				{
					GUILayout.Space(170);
					GUILayout.FlexibleSpace();

					if (node.Probabilities == null)
						node.Probabilities = new float[5];
					for (int i = 0; i < 5; i++)
					{
						node.Probabilities[i] = EditorGUILayout.FloatField(node.Probabilities[i], GUILayout.Width(30));
					}
				}
				else
				{
					GUILayout.Space(170);
					node.Probability = Mathf.Round(EditorGUILayout.Slider(node.Probability, 0f, 1f) * 10) / 10f;
				}

				//u,d,l,r
				if (node.Kind == NodeType.GroundEnemy)
				{
					GUILayout.Space(50);
				}
				else
				{
					//up,down
					GUILayout.Space(10);
					if (GUILayout.Button(Helper.u, Helper.wh16))
					{
						selectedNodes.ForEach(x => x.IsFocused = false);
						node.IsFocused = true;
						node.transform.position += new Vector3(0, .5f, 0);
					}
					if (GUILayout.Button(Helper.d, Helper.wh16))
					{
						selectedNodes.ForEach(x => x.IsFocused = false);
						node.IsFocused = true;
						node.transform.position -= new Vector3(0, .5f, 0);
					}
				}
				//left, right
				if (GUILayout.Button(Helper.l, Helper.wh16))
				{
					selectedNodes.ForEach(x => x.IsFocused = false);
					node.IsFocused = true;
					node.transform.position -= new Vector3(.5f, 0, 0);
				}
				if (GUILayout.Button(Helper.r, Helper.wh16))
				{
					selectedNodes.ForEach(x => x.IsFocused = false);
					node.IsFocused = true;
					node.transform.position += new Vector3(.5f, 0, 0);
				}
				if (GUILayout.Button("X", Helper.wh16))
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

		//lower part
		EditorGUILayout.BeginHorizontal();

		//statistics
		foreach (var group in selectedNodes.GroupBy(x => x.Kind))
		{
			string s = "";
			if (group.Key == NodeType.Coin)
			{
				s = string.Format("{1} {0}", group.Key, group.Sum(x => x.Quantity));
			}
			else
			{
				s = string.Format("{1} {0}", group.Key, group.Count());
			}
			GUILayout.Label(s);
		}

		//selection buttons
		if (GUILayout.Button("Select All", Helper.h30)) { Selection.instanceIDs = activeBlock.GetComponentsInChildren<Node>().Select(x => x.gameObject.GetInstanceID()).ToArray(); }
		if (GUILayout.Button("Deselect All", Helper.h30)) { Selection.instanceIDs = new int[] { }; }
		if (GUILayout.Button("Delete Selected", Helper.h30))
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
		if (activeBlock != null)
			activeBlock.AutoCorrect();
	}

	Block lastSelectedBlock = null;
	void OnGUI()
	{
		Helper.initGUI();

		if (!isEnabled)
		{
			isEnabled = GUILayout.Toggle(isEnabled, "Enabled");
			return;
		}

		EditorGUILayout.BeginHorizontal();
		isEnabled = GUILayout.Toggle(isEnabled, "Enabled");

		//main tab buttons
		GUI.backgroundColor = new Color(1f, 1f, 1f);
		int newShowingMode = GUILayout.SelectionGrid(Helper.CurrentTab, new[] { "Blocks", "Levels" }, 2, GUILayout.Height(40), GUILayout.Width(300));

		//save/load buttons
		GUI.backgroundColor = new Color(1f, .4f, .4f);
		if (GUILayout.Button("Delete All", GUILayout.Height(30)))
		{
			if (EditorUtility.DisplayDialog("Delete Every fucking thing", "Are you absolutely sure? Delete all Blocks and Levels?", "Abso-fucking-lutely Sure!", "گه خوردم"))
			{
				Db.DeleteAll();
				foreach (var item in _blocks)
				{
					DestroyImmediate(item);
				}
				_blocks.Clear();
			}
		}
		GUI.backgroundColor = new Color(1f, 1f, .9f);
		if (GUILayout.Button("refresh blocks", GUILayout.Height(30)))
		{
			refreshBlocks();
		}
		GUI.backgroundColor = new Color(0f, 1f, .9f);
		if (GUILayout.Button("Load Blocks", GUILayout.Height(30)))
		{
			refreshBlocks();
			if (!_blocks.Any(x => x.Nodes.Any()))
			{
				_blocks.ToList().ForEach(x => DestroyImmediate(x.gameObject));
				_blocks.Clear();
				LoadBlocks();
			}
			else
			{
				int result = EditorUtility.DisplayDialogComplex("Overwrite", "Delete Existing Level data?", "بشاش به همه ش", "نرین توش", "گه خوردم");
				//clear بشاش به همه ش
				//(add) نرین توش"
				//(cancel)گه خوردم
				switch (result)
				{
					//clear
					case 0:
						refreshBlocks();
						selectedNodes.Clear();
						Selection.instanceIDs = new int[] { };
						_blocks.ToList().ForEach(x => DestroyImmediate(x.gameObject));
						_blocks.Clear();
						LoadBlocks();
						break;
					//add
					case 1:
						LoadBlocks();
						break;
					//cancel
					case 2:
						break;
					default:
						break;
				}
			}
		}
		if (GUILayout.Button("Load Levels", GUILayout.Height(30)))
		{
			LoadLevels();
		}
		if (GUILayout.Button("Save", GUILayout.Height(30)))
		{
			SaveBlocks();
			SaveLevels();
		}

		//switch tab
		GUI.backgroundColor = new Color(1f, 1f, 1f);
		if (newShowingMode != Helper.CurrentTab)
		{
			Helper.CurrentTab = newShowingMode;
			if (Helper.CurrentTab == 0)
			{
				Db.ShowGizmos = false;
				_blocks.ToList().ForEach(x => x.IsSelected = false);
				if (_blocks.Contains(lastSelectedBlock))
					activeBlock = lastSelectedBlock;
			}
			else
			{
				Db.ShowGizmos = true;
				lastSelectedBlock = activeBlock;
				_blocks.ToList().ForEach(x => x.IsSelected = false);
			}
		}
		EditorGUILayout.EndHorizontal();



		//init
		GUI.SetNextControlName("EMPTY");
		GUILayout.TextField("", GUILayout.Width(0), GUILayout.Height(0));

		if (Helper.CurrentTab == 0)
		{
			refreshBlocks();
			GUI.backgroundColor = new Color(1f, .6f, .6f);
			onGUIBlock();


			GUILayout.BeginHorizontal();
			GUI.backgroundColor = new Color(.6f, 1f, .8f);
			onGUIAddNode();
			GUILayout.FlexibleSpace();
			onGUIChangeAll();
			GUILayout.FlexibleSpace();
			onGUIGizmoFilter();
			GUILayout.EndHorizontal();

			GUILayout.Space(10);

			GUI.backgroundColor = new Color(.6f, .8f, 1f);
			onGUISelectedNodes();
		}
		else
		{
			if (Db.Majors == null)
				Db.Init();
			onGUILevels();
		}
	}





	//---------------------------------------------------------------------------------------------------------------------
	//	Save
	//---------------------------------------------------------------------------------------------------------------------
	private void SaveBlocks()
	{
		if (_blocks == null || !_blocks.Any())
		{
			EditorUtility.DisplayDialog("Not Saved", "Blocks are not saved in Levels.json", "ریدی");
			return;
		}

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
				if (node.Probabilities == null) node.Probabilities = new float[5];
				for (int i = 0; i < 5; i++)
				{
					nbjo.AddField("p" + i, node.Probabilities[i]);
				}
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
			bjo.AddField("width", block.Width);
			bjo.AddField("value", block.Number);
			bjo.AddField("nodes", nodes);

			jo.Add(bjo);
		}

		//save
		var path = System.IO.Path.Combine(Application.persistentDataPath, "Levels.json");
		System.IO.StreamWriter w = new System.IO.StreamWriter(path, false);
		w.Write(jo.ToString());
		w.Close();

		if (!EditorUtility.DisplayDialog("Saved", "Saved to " + path, "ریدم به قیافه عنت", "Open Folder"))
		{
			System.Diagnostics.Process.Start(Application.persistentDataPath);
		}
	}

	//---------------------------------------------------------------------------------------------------------------------
	void SaveLevels()
	{
		if (Db.Majors == null)
		{
			EditorUtility.DisplayDialog("Not Saved", "Levels are not saved in Game.json", "ریدی");
			return;
		}

		//levels
		var jLevels = new JSONObject();
		for (int i = 0; i < Db.Majors.Length; i++)
		{
			jLevels.Add(Db.Majors[i].ToJson());
		}

		//root
		var jo = new JSONObject();
		jo.AddField("levels", jLevels);

		//save
		var path = System.IO.Path.Combine(Application.persistentDataPath, "Game.json");
		System.IO.StreamWriter w = new System.IO.StreamWriter(path, false);
		w.Write(jo.ToString());
		w.Close();
	}
	//---------------------------------------------------------------------------------------------------------------------
	//	Load
	//---------------------------------------------------------------------------------------------------------------------

	public void LoadBlocks()
	{
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
			var block = AddBlock();
			block.gameObject.name = bjo.GetField("name").str;
			block.Width = (int)bjo.GetField("width").f;
			if (bjo.HasField("value"))//remove later
				block.Number = (int)bjo.GetField("value").f;
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
				node.Probabilities = new float[5];
				for (int i = 0; i < 5; i++)
				{
					if (item.HasField("p" + i))
						node.Probabilities[i] = item.GetField("p" + i).f;
				}
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
	//---------------------------------------------------------------------------------------------------------------------
	void LoadLevels()
	{
		//load
		var path = System.IO.Path.Combine(Application.persistentDataPath, "Game.json");
		if (!System.IO.File.Exists(path))
		{
			Db.Init();
			return;
		}

		System.IO.StreamReader r = new System.IO.StreamReader(path);
		var js = r.ReadToEnd();
		r.Close();

		//make json
		var jo = new JSONObject(js);
		var jLevels = jo.GetField("levels").list;

		if (jLevels == null)
		{
			EditorUtility.DisplayDialog("Default", "Levels not found. using default values", "اوکی 2کی");
			Db.Init();
		}
		else
		{
			Db.Majors = new Major[jLevels.Count];
			for (int i = 0; i < jLevels.Count; i++)
			{
				Db.Majors[i] = new Major(jLevels[i]);
			}
		}


		refreshBlocks();
	}

}