[System.Serializable]
public class Objective
{
	public delegate void ObjectiveUpdatedDelegate(int val);
	public delegate void ObjectiveCompletedDelegate();

	public event ObjectiveUpdatedDelegate ObjectiveUpdated;
	public event ObjectiveCompletedDelegate ObjectiveCompleted;

	public int Order;
	public string Name;
	public string Description;

	public int TargetValue;
	public ObjectiveType Kind;
	public ObjectiveScope Scope;

	public ObjectiveReward Reward;

	[System.NonSerialized]
	private int _value;
	public int CurrentValue
	{
		get { return _value; }
		set
		{
			_value = value;
			if(Status == ObjectiveStatus.InProgress)
			{
				if (ObjectiveUpdated != null)
					ObjectiveUpdated(value);

				if (value >= TargetValue)
				{
					if (ObjectiveCompleted != null)
						ObjectiveCompleted();
					Status = ObjectiveStatus.Completed;
				}
			}
		}
	}

	public ObjectiveStatus Status
	{
		get; set;
	}

	public int[] RequirementCodes
	{
		get; set;
	}

	[System.NonSerialized]
	private Objective[] Requirements;

	public int GetUniqueCode()
	{
		return Order * 20 + (int)Scope * 10 + (int)Kind;
	}

	public bool RequirementsMet()
	{
		//pass if no requirements
		if (Requirements == null) return true;

		//else check all requirements
		foreach (var req in Requirements)
		{
			if (req.Status != ObjectiveStatus.Completed)
			{
				//not met
				return false;
			}
		}
		//all met
		return true;
	}
}