using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using RunnerData;

public class GameDb : MonoBehaviour
{
	public Major[] Majors;
	public bool ShowGizmos;
	public int SelectedIndex;
	public void Init()
	{
		Majors = Major.DefaultMajors;
	}
	public void DeleteAll()
	{
		Majors = null;
	}
	public void CountBlockUsage(Block block)
	{
		block.UsedCount = 0;
		if (Majors == null) return;
		for (int i = 0; i < Majors.Length; i++)
		{
			for (int j = 0; j < Majors[i].Minors.Length; j++)
			{
				if (Majors[i].Minors[j] == null) continue;
				if (Majors[i].Minors[j].Block == block) block.UsedCount++;
			}
		}
	}
	public int GetBlockTypeUsageCount(BlockType type)
	{
		int count = 0;
		if (Majors == null) return 0;
		for (int i = 0; i < Majors.Length; i++)
		{
			for (int j = 0; j < Majors[i].Minors.Length; j++)
			{
				if (Majors[i].Minors[j] == null) continue;
				if (Majors[i].Minors[j].Block != null && Majors[i].Minors[j].Kind == type) count++;
			}
		}
		return count;
	}
	public int GetBlockTypeUsageCount()
	{
		int count = 0;
		if (Majors == null) return 0;
		for (int i = 0; i < Majors.Length; i++)
		{
			for (int j = 0; j < Majors[i].Minors.Length; j++)
			{
				if (Majors[i].Minors[j] == null) continue;
				if (Majors[i].Minors[j].Block == null) count++;
			}
		}
		return count;
	}
	public List<string> GetBlockTypeNumbersWithUsage(RunnerData.BlockType type, int[] numbers)
	{
		List<string> list = new List<string>();
		if (Majors == null) return list;
		foreach (var num in numbers)
		{
		int count = 0;
			for (int i = 0; i < Majors.Length; i++)
			{
				for (int j = 0; j < Majors[i].Minors.Length; j++)
				{
					if (Majors[i].Minors[j] == null) continue;
					if (Majors[i].Minors[j].Kind == type && Majors[i].Minors[j].Number == num) count++;
				}
			}
			list.Add(string.Format("{0}:{1}", num, count));
		}
		return list;
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

			if (minor.Block != null)
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