using RunnerData;

[System.Serializable]
public class Major
{
	public string Name;
	public int StartX;
	public int EndX;
	public float Speed;
	public Minor[] Minors;
	public Major(int startX, int endX, int size, float speed)
	{
		StartX = startX;
		EndX = endX;
		Name = string.Format("{0}m-{1}m", startX, endX);
		Speed = speed;
		Minors = new Minor[size];
	}

	public Major(JSONObject j)
	{
		StartX = (int)j.GetField("startx").f;
		EndX = (int)j.GetField("endx").f;
		Name = string.Format("{0}m-{1}m", StartX, EndX);
		Speed = j.GetField("speed").f;

		var jMinors = j.GetField("minors").list;
		Minors = new Minor[jMinors.Count];
		for (int i = 0; i < jMinors.Count; i++)
		{
			Minors[i] = new Minor(jMinors[i]);
		}
	}

	public JSONObject ToJson()
	{
		var j = new JSONObject();
		j.AddField("speed", Speed);
		j.AddField("startx", StartX);
		j.AddField("endx", EndX);

		var jm = new JSONObject();
		for (int i = 0; i < Minors.Length; i++)
		{
			if (Minors[i] == null) Minors[i] = new Minor();
			jm.Add(Minors[i].ToJson());
		}

		j.AddField("minors", jm);
		return j;
	}


	public static Major[] DefaultMajors = new Major[]
	{
				new Major(0,90, 3, 3),
				new Major(90,300, 7,3.5f),
				new Major(300,660, 12,4),
				new Major(660,1350, 23,4.5f),
				new Major(1350,2250, 30,5),
				new Major(2250,3420, 39,5.5f),
				new Major(3420,5040, 54,6),
				new Major(5040,6960, 64,6.5f),
				new Major(6960,9270, 77,7),
				new Major(9270,12210, 98,7.5f),
	};

}