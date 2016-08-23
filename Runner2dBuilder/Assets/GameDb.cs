using System.Linq;
using System.Collections.Generic;
using UnityEngine;
public class GameDb : MonoBehaviour
{
	public Major[] Majors;
	List<Block> Blocks = new List<Block>();
	public bool ShowGizmos;
	public int SelectedIndex;

	public void DeleteAll()
	{
		foreach (var item in Blocks)
		{
			if (item != null)
				DestroyImmediate(item.gameObject);
		}
		Blocks.Clear();
		Majors = null;
	}
	public void AddBlock(Block b)
	{
		if (!Blocks.Contains(b))
			Blocks.Add(b);
	}
	public void DeleteBlock(Block b)
	{
		if (Blocks.Contains(b))
			Blocks.Remove(b);
	}

	public void UpdateBlocks(List<Block> _blocks, bool _sortBlocks)
	{
		Blocks = Blocks.Where(x => x != null).ToList();

		if (_sortBlocks)
			Blocks.Sort(new BlockNameComparer());

		if (_blocks.Count != Blocks.Count)
		{
			_blocks.Clear();
			foreach (var block in Blocks)
			{
				_blocks.Add(block);
			}
		}
	}


	void OnDrawGizmos()
	{
		if (!ShowGizmos) return;
		if (Majors == null) return;
		if (SelectedIndex < 0 || SelectedIndex >= Majors.Length) return;

		var major = Majors[SelectedIndex];
		if (major == null) return;

		//draw
		Vector3 pos = Vector3.zero;
		foreach (var minor in major.Minors)
		{
			if (minor == null) continue;

			//add space before minor
			pos += minor.Space * Vector3.right;

			if(minor.Block != null)
			{
				//draw minor block
				minor.Block.Draw(pos);
				foreach (var node in minor.Block.Nodes)
				{
					node.Draw(pos);
				}
				//move pointer forward
				pos += minor.Block.Width * Vector3.right;
			}
		}
	}
}