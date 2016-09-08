using UnityEngine;
using System.Linq;
using RunnerData;
using System.Collections.Generic;

public class Block : MonoBehaviour
{
	Vector3 center { get { return new Vector3(Width / 2, 4, 0); } }
	Vector3 size { get { return new Vector3(Width, 8, 0.1f); } }
	Vector3 playcenter { get { return new Vector3(Width / 2, 4.25f, 0); } }
	Vector3 playsize { get { return new Vector3(Width, 5.5f, 0.1f); } }

	const float minY = 1.5f;
	const float maxY = 7;

	public Node[] Nodes
	{
		get { return GetComponentsInChildren<Node>(); }
	}

	//bool _isSelected;
	//public bool IsSelected
	//{
	//	get { return _isSelected; }
	//	set
	//	{
	//		if (_isSelected != value)
	//		{
	//			_isSelected = value;
	//			if (value)
	//			{
	//				if(Event.current.type == EventType.ContextClick)
	//				UnityEditor.Highlighter.Highlight("Hierarchy", name);
	//			}
	//		}
	//	}
	//}
	public bool _isSelected;
	public bool IsSelected
	{
		get { return _isSelected; }
		set
		{
			_isSelected = value;
			var list = GetComponentsInChildren<Node>(true);
			foreach (var item in list)
			{
				item.gameObject.SetActive(value);
			}
		}
	}
	public int UsedCount;

	public BlockType Kind;
	public int Width = 30;
	public int Number = 0;
	public bool WrongNumber = false;

	void Update()
	{
		if (transform.parent != null) transform.parent = null;
	}
	void OnDrawGizmos()
	{
		if (!IsSelected) return;
		Draw(Vector3.zero);
	}
	public void Draw(Vector3 offset)
	{
		Gizmos.color = new Color(0, 0, 0, .2f);
		Gizmos.DrawCube(offset + center, size);

		Gizmos.color = new Color(1, 1, 1, .2f);
		Gizmos.DrawCube(offset + playcenter, playsize);

		Gizmos.color = Color.green;
		Gizmos.DrawLine(offset + transform.position + new Vector3(0, 0, 0), offset + transform.position + new Vector3(Width, 0, 0));
		Gizmos.color = Color.black;
		Gizmos.DrawLine(offset + transform.position + new Vector3(0, 2.5f, 0), offset + transform.position + new Vector3(Width, 2.5f, 0));
		Gizmos.DrawLine(offset + transform.position + new Vector3(0, 4.25f, 0), offset + transform.position + new Vector3(Width, 4.25f, 0));
		Gizmos.DrawLine(offset + transform.position + new Vector3(0, 6, 0), offset + transform.position + new Vector3(Width, 6, 0));
	}
	public static Block CreateNew(string name)
	{
		var go = new GameObject(name);
		go.tag = "blk";
		var blk = go.AddComponent<Block>();
		blk.Kind = BlockType.free;
		blk.Width = 30;
		return blk;
	}
	public void CopyFrom(Block other)
	{
		Kind = other.Kind;
		Width = other.Width;
		foreach (var item in other.Nodes)
		{
			var node = AddNode((int)item.Kind, item.Variation, item.transform.position, item.Group, item.GroupTag);
			node.Probability = item.Probability;
			node.IsMainTarget = item.IsMainTarget;
			node.Quantity = item.Quantity;
			node.Distance = item.Distance;
			node.SS = item.SS;
			node.SX = item.SX;
			node.SY = item.SY;
		}
	}
	public Node AddNode(int kind, int variation, Vector3 pos, Group group, GroupTag gtag)
	{
		//reset block transform
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;

		var newNode = new GameObject().AddComponent<Node>();
		newNode.Data = new NodeData();
		newNode.gameObject.tag = "dyn";
		newNode.transform.parent = transform;
		newNode.transform.position = pos;
		newNode.Block = this;
		newNode.Group = group;
		newNode.GroupTag = gtag;
		newNode.Kind = (NodeType)kind;
		newNode.Variation = variation;
		newNode.ResetName();

		return newNode;
	}
	public void AutoCorrect()
	{
		var children = GetComponentsInChildren<Transform>(true);
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
				node.Block = this;
				//round position
				if (node.Kind == NodeType.Person)
				{
					node.transform.position = new Vector3(
						Mathf.Max(0, Mathf.Round(4 * node.transform.position.x) / 4),
						0);

				}
				else
				{
					node.transform.position = new Vector3(
						Mathf.Max(0, Mathf.Round(4 * node.transform.position.x) / 4),
						Mathf.Clamp(Mathf.Round(4 * node.transform.position.y) / 4, minY, maxY));

				}
			}
		}

	}

	public Node FirstOverlappedNode()
	{
		foreach (var node in Nodes)
		{
			var overlap = FirstOverlappedNode(node);
			if (overlap != null)
				return overlap;
		}
		return null;
	}
	public Node FirstOverlappedNode(Node other)
	{
		//check for incorrect positioning
		if (other.Kind == NodeType.Person)
		{
			//check other people
			return Nodes.FirstOrDefault(n => n != other && n.Kind == NodeType.Person &&
				Mathf.Abs(other.transform.position.x - n.transform.position.x) < .1f);
		}
		else
		{
			//check all
			return Nodes.FirstOrDefault(n => n != other &&
				Mathf.Abs(other.transform.position.x - n.transform.position.x) < .1f
				&& Mathf.Abs(other.transform.position.y - n.transform.position.y) < .1f);
		}
	}
}