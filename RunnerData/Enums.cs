namespace RunnerData
{
	public enum NodeType { Collectible, Coin, Food, Person, Eagle }
	public enum BlockType { sp, gift, drive, sparse, normal, crowd, challange, fun }
	public enum GroupTag { Fixed, A, B, C}
	public enum Group { G1, G2, G3, G4, G5 }
	public enum PersonVariation { Normal, Fat, Girl, Driver, Bride }
	public enum EagleVariation { Normal, Fast, Sinus }
	public enum CollectibleVariation { Magnet, Oil, Rocket, Plane, Gem, Heart, }
	public enum FoodVariation { Small, Medium, Large, Drink }
	public enum CoinArrangement { Line, SmallSinus, BigSinus }

	public enum PoolObjectType
	{
		Collectible = 0,
		Coin = 1,
		Food = 2,
		Person = 3,
		Eagle = 4,
		Effect = 5,
		Poop = 6,
		Parallax = 7,
		Count = 8,
	}
}