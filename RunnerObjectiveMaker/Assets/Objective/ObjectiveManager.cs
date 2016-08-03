using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
	static ObjectiveManager _instance;
	public static ObjectiveManager instance
	{
		get
		{
			if (!_instance)
			{
				//first try to find one in the scene
				_instance = FindObjectOfType<ObjectiveManager>();

				if (!_instance)
				{
					//if that fails, make a new one
					GameObject go = new GameObject("_ObjectiveManager");
					_instance = go.AddComponent<ObjectiveManager>();

					if (!_instance)
					{
						//if that still fails, we have a big problem;
						Debug.LogError("Fatal Error: could not create ObjectiveManager");
					}

					_instance.Initialize();
				}
			}

			return _instance;
		}
	}

	public Objective[] AllObjectives { get; private set; }
	private string filePath;
	public void Initialize()
	{
		filePath = Application.persistentDataPath + "/objs.dat";
		Load();
	}
	public Objective[] ActiveObjectives { get; private set; }

	void Awake()
	{
		Initialize();
		//check for completed daily objectives here
		//...
	}


	private void ActivateNext()
	{

	}

	//---------------------------------------------------------------------------------------------
	// Serialization
	//---------------------------------------------------------------------------------------------
	public void Save(Objective[] allobjs)
	{
		filePath = Application.persistentDataPath + "/objs.dat";

		Debug.Log(filePath);

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(filePath);
		bf.Serialize(file, allobjs);
		file.Close();
	}

	public void Load()
	{
		if (File.Exists(filePath))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(filePath, FileMode.Open);
			AllObjectives = (Objective[])bf.Deserialize(file);
			file.Close();
		}
	}

	//---------------------------------------------------------------------------------------------
	// Helpers:
	//---------------------------------------------------------------------------------------------

//	private const int MAX_LEVELS = 12;

//	private static Objective CreateObjective(int level, ObjectiveType kind, ObjectiveScope scope)
//	{
//		return new Objective
//		{
//			Name = ObjectiveTypeNames[(int)kind],
//			Description = string.Format(
//				ObjectiveTypeDescriptions[(int)kind], //string format
//				TargetValues[(int)kind, level],     //{0}
//				ObjectiveScopeNames[(int)scope]),   //{1}
//			Kind = kind,
//			Scope = scope,
//			CurrentValue = 0,
//			Reward = RewardValues[(int)kind, level],
//			Status = ObjectiveStatus.NotActive,
//			TargetValue = TargetValues[(int)kind, level],
//		};
//	}
//	private static string[] ObjectiveScopeNames = new string[(int)ObjectiveScope.Count]
//    {
//		"(total).",
//		"in a single run."
//	};
//	private static string[] ObjectiveTypeNames = new string[(int)ObjectiveType.Count]
//	{
//		"Coins",
//		"Distance",
//		"Enemies",
//		"People",
//	};
//	private static string[] ObjectiveTypeDescriptions = new string[(int)ObjectiveType.Count]
//	{
//		"Gather {0} coins {1}",
//		"Travel {0} meters {1}",
//		"Defeat {0} Enemies {1}",
//		"Shoot {0} People {1}",
//	};
//	private static int[,] TargetValues = new int[(int)ObjectiveType.Count, MAX_LEVELS]
//	{
//		/*Money*/		{ 100, 250, 500, 1000, 2000, 3000, 5000, 10000, 20000, 100000, 1000000, 10000000 },
//		/*Distance*/	{ 100, 250, 500, 1000, 2000, 3000, 5000, 10000, 20000, 100000, 1000000, 10000000 },
//		/*GroundEnemy*/ { 3, 10, 25, 40, 100, 160, 250, 1000, 2000, 10000, 100000, 1000000 },
//		/*AirEnemy*/	{ 3, 10, 25, 40, 100, 160, 250, 1000, 2000, 10000, 100000, 1000000 },
//	};
//	private static ObjectiveReward[,] RewardValues = new ObjectiveReward[(int)ObjectiveType.Count, MAX_LEVELS]
//	{
//		/*Money*/		
//		{
//			new ObjectiveReward(ObjectiveRewardType.Money,100),
//			new ObjectiveReward(ObjectiveRewardType.Money, 250),
//			new ObjectiveReward(ObjectiveRewardType.Money, 500),
//			new ObjectiveReward(ObjectiveRewardType.Money, 1000),
//			new ObjectiveReward(ObjectiveRewardType.Money, 2000),
//			new ObjectiveReward(ObjectiveRewardType.Money, 3000),
//			new ObjectiveReward(ObjectiveRewardType.Money, 5000),
//			new ObjectiveReward(ObjectiveRewardType.Money, 10000),
//			new ObjectiveReward(ObjectiveRewardType.Money, 20000),
//			new ObjectiveReward(ObjectiveRewardType.Money, 100000),
//			new ObjectiveReward(ObjectiveRewardType.Money, 1000000),
//			new ObjectiveReward(ObjectiveRewardType.Money, 10000000)
//		},
//		/*Distance*/	
//		{
//			new ObjectiveReward(ObjectiveRewardType.Gem,1),
//			new ObjectiveReward(ObjectiveRewardType.Gem, 2),
//			new ObjectiveReward(ObjectiveRewardType.Gem, 4),
//			new ObjectiveReward(ObjectiveRewardType.Gem, 6),
//			new ObjectiveReward(ObjectiveRewardType.Gem, 8),
//			new ObjectiveReward(ObjectiveRewardType.Gem, 10),
//			new ObjectiveReward(ObjectiveRewardType.Gem, 15),
//			new ObjectiveReward(ObjectiveRewardType.Gem, 20),
//			new ObjectiveReward(ObjectiveRewardType.Gem, 25),
//			new ObjectiveReward(ObjectiveRewardType.Gem, 30),
//			new ObjectiveReward(ObjectiveRewardType.Gem, 35),
//			new ObjectiveReward(ObjectiveRewardType.Gem, 50)
//		},
//		/*GroundEnemy*/ 
//		{
//			new ObjectiveReward(ObjectiveRewardType.Money,100),
//			new ObjectiveReward(ObjectiveRewardType.Money, 250),
//			new ObjectiveReward(ObjectiveRewardType.Money, 500),
//			new ObjectiveReward(ObjectiveRewardType.Money, 1000),
//			new ObjectiveReward(ObjectiveRewardType.Money, 2000),
//			new ObjectiveReward(ObjectiveRewardType.Money, 3000),
//			new ObjectiveReward(ObjectiveRewardType.Money, 5000),
//			new ObjectiveReward(ObjectiveRewardType.Money, 10000),
//			new ObjectiveReward(ObjectiveRewardType.Money, 20000),
//			new ObjectiveReward(ObjectiveRewardType.Money, 100000),
//			new ObjectiveReward(ObjectiveRewardType.Money, 1000000),
//			new ObjectiveReward(ObjectiveRewardType.Money, 10000000)
//		},
//		/*AirEnemy*/	
//		{
//			new ObjectiveReward(ObjectiveRewardType.Experience, 50),
//			new ObjectiveReward(ObjectiveRewardType.Experience, 100),
//			new ObjectiveReward(ObjectiveRewardType.Experience, 150),
//			new ObjectiveReward(ObjectiveRewardType.Experience, 200),
//			new ObjectiveReward(ObjectiveRewardType.Experience, 250),
//			new ObjectiveReward(ObjectiveRewardType.Experience, 300),
//			new ObjectiveReward(ObjectiveRewardType.Experience, 400),
//			new ObjectiveReward(ObjectiveRewardType.Experience, 500),
//			new ObjectiveReward(ObjectiveRewardType.Experience, 750),
//			new ObjectiveReward(ObjectiveRewardType.Experience, 1000),
//			new ObjectiveReward(ObjectiveRewardType.Experience, 2000),
//			new ObjectiveReward(ObjectiveRewardType.Experience, 3000)
//		},
//	};
}