﻿namespace RunnerData
{
	[System.Serializable]
	public class NodeData
	{
		//general
		public NodeType Kind = NodeType.Coin;
		public int Variation = 0;
		public float Probability = 1;

		//person
		public bool IsMainTarget = false;

		//coin
		public int Quantity = 1;
		public float Distance = 0.5f;//inner distance
		public float SS = 1;//sinus start
		public float SX = 0;//sinus compactness x
		public float SY = 0;//sinus compactness y

		public Group Group = Group.G1;
		public GroupTag GroupTag = GroupTag.Fixed;

		public string GetVariationString()
		{
			switch (Kind)
			{
				case NodeType.Collectible:
					return ((CollectibleVariation)Variation).ToString();
				case NodeType.Food:
					return ((FoodVariation)Variation).ToString();
				case NodeType.Person:
					return ((PersonVariation)Variation).ToString();
				case NodeType.Eagle:
					return ((EagleVariation)Variation).ToString();
				case NodeType.Coin:
				default:
					return "";
			}
		}
		public override string ToString()
		{
			return Kind.ToString() + "-" + GetVariationString();
		}
	}
}