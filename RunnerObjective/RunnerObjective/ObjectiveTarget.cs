using RunnerData;

[System.Serializable]
public struct ObjectiveTarget
{
	public int Value;
	public ObjectiveKind Kind;
	public ObjectiveCondition Condition;
	public ObjectiveScope Scope;

	public PersonVariation DetailGround;
	public EagleVariation DetailAir;
	public CollectibleVariation DetailCollectible;
	public ObjectiveRewardType DetailConsumable;
}
