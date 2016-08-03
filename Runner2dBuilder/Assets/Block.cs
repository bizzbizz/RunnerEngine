using UnityEngine;
using System.Linq;
using RunnerData;
using System.Collections.Generic;

public class Block : MonoBehaviour
{
	Vector3 center { get { return new Vector3(Width / 2, 4, 0); } }
	Vector3 size { get { return new Vector3(Width, 8, 0.1f); } }
	public Node[] Nodes
	{
		get { return GetComponentsInChildren<Node>(); }
	}
	public List<Group> Groups;

	public bool IsSelected = false;
	public BlockType Kind;
	public int Width = 30;

	void OnDrawGizmos()
	{
		if (!IsSelected) return;

		Gizmos.color = new Color(0, 0, 0, .1f);
		Gizmos.DrawCube(center, size);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position + new Vector3(0, 0, 0), transform.position + new Vector3(Width, 0, 0));
		Gizmos.color = Color.black;
		Gizmos.DrawLine(transform.position + new Vector3(0, 2, 0), transform.position + new Vector3(Width, 2, 0));
		Gizmos.DrawLine(transform.position + new Vector3(0, 4, 0), transform.position + new Vector3(Width, 4, 0));
		Gizmos.DrawLine(transform.position + new Vector3(0, 6, 0), transform.position + new Vector3(Width, 6, 0));
	}

	public static Block CreateNew(string name, bool addDefaultGroup = true)
	{
		var go = new GameObject(name);
		go.tag = "blk";
		var blk = go.AddComponent<Block>();
		blk.Kind = BlockType.normal;
		blk.Width = 30;
		return blk;
	}
	//public void CopyFrom(Block other)
	//{
	//	Kind = other.Kind;
	//	Width = other.Width;
	//	Value = other.Value;
	//	Groups = other.Groups.Select(x => new Group(this)).ToList();
	//	foreach (var item in other.Nodes)
	//	{
	//		var node = AddNode((int)item.Kind, item.Variation, item.transform.position, Groups.First(x => x.Id == item.Group.Id));
	//		node.Probability = item.Probability;
	//		node.IsMainTarget = item.IsMainTarget;
	//		node.Quantity = item.Quantity;
	//		node.Distance = item.Distance;
	//		node.Arrangement = item.Arrangement;
	//	}
	//}
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