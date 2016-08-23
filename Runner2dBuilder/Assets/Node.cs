using UnityEngine;
using UnityEditor;
using RunnerData;
public class Node : MonoBehaviour
{
	public NodeData Data;

	public NodeType Kind { get { return Data.Kind; } set { Data.Kind = value; } }
	public int Variation { get { return Data.Variation; } set { Data.Variation = value; } }
	public float Probability { get { return Data.Probability; } set { Data.Probability = value; } }

	//person
	public bool IsMainTarget { get { return Data.IsMainTarget; } set { Data.IsMainTarget = value; } }
	//coin
	public int Quantity { get { return Data.Quantity; } set { Data.Quantity = value; } }
	public float Distance { get { return Data.Distance; } set { Data.Distance = value; } }
	public float SS { get { return Data.SS; } set { Data.SS = value; } }
	public float SX { get { return Data.SX; } set { Data.SX = value; } }
	public float SY { get { return Data.SY; } set { Data.SY = value; } }

	//in editor
	public Block Block;
	public GroupTag GroupTag { get { return Data.GroupTag; } set { Data.GroupTag = value; } }
	public Group Group { get { return Data.Group; } set { Data.Group = value; } }
	public bool IsFocused = false;
	public bool CanDraw = true;

	public override string ToString()
	{
		return Data.ToString();
	}
	public void ResetName()
	{
		if (Data != null)
		{
			if (Kind != NodeType.Person) IsMainTarget = false;
			gameObject.name = Data.ToString() + " " + Probability + GroupText();
		}
	}
	public string GroupText()
	{
		return (GroupTag == GroupTag.Fixed ? "" : string.Format("[{0}-{1}]", GroupTag, Group));
	}
	public string GroupShortText()
	{
		return (GroupTag == GroupTag.Fixed ? "" : string.Format("[{0}-{1}]", GroupTag.ToString()[1], Group.ToString()[1]));
	}
	//helper
	static Color[] groupColors = new[]
	{
		Color.magenta,
		new Color(0,0,0,0),
		Color.green,
		Color.gray,
		Color.blue,
	};


	void OnDrawGizmos()
	{
		if (Block == null || !Block.IsSelected || !CanDraw) return;

		ResetName();

		Draw(Vector3.zero);

		//write prob
		Handles.color = Color.red;
		if (GroupTag == GroupTag.Fixed)
			Handles.Label(transform.position + Vector3.up, Probability.ToString());
		else
			Handles.Label(transform.position + new Vector3(-.25f, 1, 0), Probability.ToString() + GroupShortText());
	}
	public void Draw(Vector3 offset)
	{
		if (Kind == NodeType.Coin)//coin bundle
		{
			for (int i = 0; i < Quantity; i++)
				drawIcon(
					offset +
					transform.position//initial position
					+ Vector3.right * i * Distance//inner distance
					+ ((SX == 0 || SY == 0) //check for sinus
					? Vector3.zero //line
					: Vector3.up * Mathf.Sin(SS + i * SX) * SY));//sinus
		}
		else if (Kind == NodeType.Eagle && Variation == 2)
		{
			Gizmos.DrawWireCube(offset + transform.position, new Vector3(.25f, 2.25f, 0));
			drawIcon(offset + transform.position);
		}
		else
		{
			drawIcon(offset + transform.position);
		}
	}
	void drawIcon(Vector3 pos)
	{
		Gizmos.color = IsMainTarget ? Color.red : groupColors[(int)Kind];
		Gizmos.DrawCube(pos, Vector3.one);
		Gizmos.DrawIcon(pos, string.Format("{0}.png", this));
	}
	void OnDrawGizmosSelected()
	{
		if (Block == null || !Block.IsSelected) return;

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(transform.position, Vector3.one * 1.1f);
	}

}
