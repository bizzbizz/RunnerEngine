[System.Serializable]
public struct ObjectiveReward
{
	public int Value;
	public ObjectiveRewardType Kind;
	public ObjectiveReward(ObjectiveRewardType kind, int value)
	{
		Kind = kind;
		Value = value;
	}
}