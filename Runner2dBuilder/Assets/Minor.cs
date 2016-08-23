using RunnerData;

[System.Serializable]
public class Minor
{
	public int Space;
	public BlockType Kind;
	public int Number;
	[System.NonSerialized]
	public Block Block;
	public Minor()
	{
		Space = 0;
		Kind = BlockType.free;
		Number = -1;
	}
	public Minor(JSONObject j)
	{
		Space = (int)j.GetField("space").f;
		Kind = (BlockType)(int)j.GetField("kind").f;
		Number = (int)j.GetField("number").f;
	}
	public JSONObject ToJson()
	{
		var j = new JSONObject();
		j.AddField("space", Space);
		j.AddField("kind", (int)Kind);
		j.AddField("number", Number);
		return j;
	}
}