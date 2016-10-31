using RunnerData;

[System.Serializable]
public struct ObjectiveTarget
{
	public int Value;
	public ObjectiveKind Kind;
	public ObjectiveCondition Condition;
	public ObjectiveScope Scope;

	public GroundVariation DetailGround;
	public AirVariation DetailAir;
	public CollectibleVariation DetailCollectible;
	public ObjectiveRewardType DetailConsumable;
}
