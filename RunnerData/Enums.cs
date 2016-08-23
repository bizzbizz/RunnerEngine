namespace RunnerData
{
	public enum NodeType
	{
		Collectible = 0,
		Coin = 1,
		Food = 2,
		Person = 3,
		Eagle = 4,
	}
	public enum BlockType { free, person1, person2, person3, funCoin, funShit, funDrive, challange, gift, other }
	public enum GroupTag { Fixed, T1, T2, T3, T4, T5, T6, T7, T8, T9 }
	public enum Group { G1, G2, G3, G4, G5, G6 }
	public enum PersonVariation { Normal, Fat, Girl, Driver, Bride }
	public enum EagleVariation { Normal, Fast, Sinus }
	public enum CollectibleVariation { Magnet, Oil, Rocket, Plane, Gem, Heart, }
	public enum FoodVariation { Small, Medium, Large, Drink }
	public enum CoinArrangement { Line, SmallSinus, BigSinus }

	public enum PoolObjectType
	{
		Collectible = 0,//match NodeType
		Coin = 1,//match NodeType
		Food = 2,//match NodeType
		Person = 3,//match NodeType
		Eagle = 4,//match NodeType
		Effect = 5,
		Poop = 6,
		Parallax = 7,
		Count = 8,
	}
}