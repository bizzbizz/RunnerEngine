﻿namespace RunnerData
{
	public enum NodeType
	{
		Collectible = 0,
		Coin = 1,
		Food = 2,
		GroundEnemy = 3,
		AirEnemy = 4,
	}
	public enum BlockType { free, person1, person2, person3, funCoin, funShit, funDrive, challange, tutorial, other }
	public enum GroupTag { Fixed, T1, T2, T3, T4, T5, T6, T7, T8, T9 }
	public enum Group { G1, G2, G3, G4, G5, G6 }
	public enum GroundVariation { Normal = 0, Fat = 1, Girl = 2, Kid = 3, Ghelyuni = 4, Elderly = 5}
	public enum AirVariation
	{
		Tuka = 0, Vulture = 1, Pelican = 2,
		Owl = 3, Bat = 4,
	}
	public enum AirGroupVariation { Normal = 0, Fast = 1, Sinus = 2}
	public static class AirVariationConverter
	{
		public static AirVariation Convert(AirGroupVariation agv, bool isNight)
		{
			if (isNight)
			{
				switch (agv)
				{
					default:
					case AirGroupVariation.Normal:
						return AirVariation.Tuka;
					case AirGroupVariation.Fast:
						return AirVariation.Owl;
					case AirGroupVariation.Sinus:
						return AirVariation.Bat;
				}
			}
			else
			{
				switch (agv)
				{
					default:
					case AirGroupVariation.Normal:
						return AirVariation.Tuka;
					case AirGroupVariation.Fast:
						return AirVariation.Vulture;
					case AirGroupVariation.Sinus:
						return AirVariation.Pelican;
				}
			}
		}
	}

	public enum CollectibleVariation { Magnet = 0, Oil = 1, Rocket = 2, Plane = 3, Gem = 4, Heart = 5, }
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

	public enum ObjectiveSlot
	{
		A = 0,
		B = 1,
		C = 2
	}
	public enum ObjectiveKind
	{
		//A
		CollectCoin = 1,
		FlyDistance = 2,
		FlyDuration = 3,

		//B
		HitAnyEnemy = 11,
		HitAnyGroundEnemy = 12,
		HitSpecificGroundEnemy = 13,
		HitAnyAirEnemy = 14,
		HitSpecificAirEnemy = 15,

		//C
		CollectCollectible = 21,
		UseConsumable = 22,
		ActivateSuperpower = 23,
		CompleteDailyChallenge = 25,
		PassBetweenEnemies = 26,
	}
	public enum ObjectiveCondition
	{
		None = 0,
		WithCollectible = 1,
		WithSuperpower = 2,
		WithoutCoin = 3,
		WithoutShit = 4,
	}

	public enum ObjectiveScope
	{
		Total = 0,
		OneRun = 1,
	}
	public enum ObjectiveRewardType
	{
		Coin = 0,
		Gem = 1,
		Heart = 2,
		Booster = 3,
	}
	public enum ObjectiveStatus
	{
		NotActive = 0,
		InProgress = 1,
		Completed = 2,
	}

}