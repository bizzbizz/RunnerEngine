using System.Collections.Generic;

public class BlockNameComparer : IComparer<Block>
{
	public int Compare(Block x, Block y)
	{
		if (x == null || y == null) return 0;
		return x.gameObject.name.CompareTo(y.gameObject.name);
	}
}
public class TGTXYComparer : IComparer<Node>
{
	public int Compare(Node x, Node y)
	{
		if (x.GroupTag < y.GroupTag) return -1;
		else if (x.GroupTag > y.GroupTag) return 1;
		else if (x.Group < y.Group) return -1;
		else if (x.Group > y.Group) return 1;

		if ((int)x.Kind < (int)y.Kind)
			return -1;
		else if ((int)x.Kind > (int)y.Kind)
			return 1;
		else if (x.transform.position.x < y.transform.position.x)
			return -1;
		else if (x.transform.position.x > y.transform.position.x)
			return 1;
		else if (x.transform.position.y > y.transform.position.y)
			return -1;
		else if (x.transform.position.y < y.transform.position.y)
			return 1;
		else
			return 0;
	}
}
public class TXYComparer : IComparer<Node>
{
	public int Compare(Node x, Node y)
	{
		if ((int)x.Kind < (int)y.Kind)
			return -1;
		else if ((int)x.Kind > (int)y.Kind)
			return 1;
		else if (x.transform.position.x < y.transform.position.x)
			return -1;
		else if (x.transform.position.x > y.transform.position.x)
			return 1;
		else if (x.transform.position.y > y.transform.position.y)
			return -1;
		else if (x.transform.position.y < y.transform.position.y)
			return 1;
		else
			return 0;
	}
}

public class XYComparer : IComparer<Node>
{
	public int Compare(Node x, Node y)
	{
		if (x.transform.position.x < y.transform.position.x)
			return -1;
		else if (x.transform.position.x > y.transform.position.x)
			return 1;
		else if (x.transform.position.y > y.transform.position.y)
			return -1;
		else if (x.transform.position.y < y.transform.position.y)
			return 1;
		else
			return 0;
	}
}
public class TVComparer : IComparer<Node>
{
	public int Compare(Node x, Node y)
	{
		if ((int)x.Kind < (int)y.Kind)
			return -1;
		else if ((int)x.Kind > (int)y.Kind)
			return 1;
		else if (x.Variation < y.Variation)
			return -1;
		else if (x.Variation > y.Variation)
			return 1;
		else
			return 0;
	}
}
