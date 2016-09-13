using RunnerData;

[System.Serializable]
public class Objective
{
	//public delegate void ObjectiveUpdatedDelegate(int val);
	//public delegate void ObjectiveCompletedDelegate();

	//public event ObjectiveUpdatedDelegate ObjectiveUpdated;
	//public event ObjectiveCompletedDelegate ObjectiveCompleted;

	public ObjectiveSlot Slot;
	public string Name;
	public string Description;
	public int Index;

	public ObjectiveTarget Target;
	public ObjectiveReward Reward;

	public ObjectiveStatus Status
	{
		get; set;
	}

	public int GetUniqueCode()
	{
		return Index + (int)Slot * 10000;
	}

	//[System.NonSerialized]
	//private int _value;
	//public int CurrentValue
	//{
	//	get { return _value; }
	//	set
	//	{
	//		_value = value;
	//		if (Status == ObjectiveStatus.InProgress)
	//		{
	//			if (ObjectiveUpdated != null)
	//				ObjectiveUpdated(value);

	//			if (value >= Target.Value)
	//			{
	//				if (ObjectiveCompleted != null)
	//					ObjectiveCompleted();
	//				Status = ObjectiveStatus.Completed;
	//			}
	//		}
	//	}
	//}


	public Objective()
	{

	}
	public Objective Clone()
	{
		return new Objective
		{
			Slot = Slot,
			Name = Name,
			Description = Description,
			Index = Index + 1,

			Target = Target,

			Reward = Reward,
		};
	}


	public Objective(JSONObject jo)
	{
		Slot = (ObjectiveSlot)(int)jo.GetField("slot").f;
		Name = jo.GetField("name").str;
		Description = jo.GetField("desc").str;
		Index = (int)jo.GetField("index").f;

		Target = new ObjectiveTarget();
		Target.Kind = (ObjectiveKind)(int)jo.GetField("tkind").f;
		Target.Value = (int)jo.GetField("tval").f;
		Target.Scope = (ObjectiveScope)(int)jo.GetField("tscope").f;
		Target.Condition = (ObjectiveCondition)(int)jo.GetField("tcond").f;
		Target.DetailGround = (PersonVariation)(int)jo.GetField("tdg").f;
		Target.DetailAir = (EagleVariation)(int)jo.GetField("tda").f;
		Target.DetailCollectible = (CollectibleVariation)(int)jo.GetField("tdc").f;
		if (jo.HasField("tdo"))//newly added
			Target.DetailConsumable = (ObjectiveRewardType)(int)jo.GetField("tdo").f;

		Reward = new ObjectiveReward();
		Reward.Kind = (ObjectiveRewardType)(int)jo.GetField("rkind").f;
		Reward.Value = (int)jo.GetField("rval").f;
	}

	public JSONObject ToJSONObject()
	{
		JSONObject jo = new JSONObject();

		jo.AddField("slot", (int)Slot);
		jo.AddField("name", Name);
		jo.AddField("desc", Description);
		jo.AddField("index", Index);

		jo.AddField("tkind", (int)Target.Kind);
		jo.AddField("tval", Target.Value);
		jo.AddField("tscope", (int)Target.Scope);
		jo.AddField("tcond", (int)Target.Condition);
		jo.AddField("tdg", (int)Target.DetailGround);
		jo.AddField("tda", (int)Target.DetailAir);
		jo.AddField("tdc", (int)Target.DetailCollectible);
		jo.AddField("tdo", (int)Target.DetailConsumable);

		jo.AddField("rkind", (int)Reward.Kind);
		jo.AddField("rval", Reward.Value);
		return jo;
	}
}