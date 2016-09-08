public static class Db
{
	public static Objective[] As = new Objective[]
	{
		new Objective
		{
			Slot = RunnerData.ObjectiveSlot.A,
			Name = "Collector",
			Description = "{0} سکه جمع کن",
			Target = new ObjectiveTarget
			{
				Kind = RunnerData.ObjectiveKind.CollectCoin,
			},
		},
		new Objective
		{
			Slot = RunnerData.ObjectiveSlot.A,
			Name = "Magnetizer",
			Description = "با کمک آهنربا {0} سکه جمع کن",
			Target = new ObjectiveTarget
			{
				Kind = RunnerData.ObjectiveKind.CollectCoin,
				Condition = RunnerData.ObjectiveCondition.WithCollectible,
				DetailCollectible = RunnerData.CollectibleVariation.Magnet,
			},
		},
		new Objective
		{
			Slot = RunnerData.ObjectiveSlot.A,
			Name = "Shopper",
			Description = "{0} سکه خرج کن",
			Target = new ObjectiveTarget
			{
				Kind = RunnerData.ObjectiveKind.SpendCoin,
			},
		},
		new Objective
		{
			Slot = RunnerData.ObjectiveSlot.A,
			Name = "Runner",
			Description = "{0} متر پرواز کن",
			Target = new ObjectiveTarget
			{
				Kind = RunnerData.ObjectiveKind.FlyDistance,
			},
		},
		new Objective
		{
			Slot = RunnerData.ObjectiveSlot.A,
			Name = "Runner",
			Description = "{0} دقیقه پرواز کن",
			Target = new ObjectiveTarget
			{
				Kind = RunnerData.ObjectiveKind.FlyDuration,
			},
		},
		new Objective
		{
			Slot = RunnerData.ObjectiveSlot.A,
			Name = "Tycoon",
			Description = "{0} متر بدون جمع کردن سکه پرواز کن",
			Target = new ObjectiveTarget
			{
				Kind = RunnerData.ObjectiveKind.FlyDistance,
				Condition = RunnerData.ObjectiveCondition.WithoutCoin,
			},
		},
		new Objective
		{
			Slot = RunnerData.ObjectiveSlot.A,
			Name = "Tycoon",
			Description = "{0} دقیقه بدون جمع کردن سکه پرواز کن",
			Target = new ObjectiveTarget
			{
				Kind = RunnerData.ObjectiveKind.FlyDuration,
				Condition = RunnerData.ObjectiveCondition.WithoutCoin,
			},
		},
	};


	public static Objective[] Bs = new Objective[]
	{
		new Objective
		{
			Slot = RunnerData.ObjectiveSlot.B,
			Name = "Marksman",
			Description = "{0} دشمن را کثیف کن",
			Target = new ObjectiveTarget
			{
				Kind = RunnerData.ObjectiveKind.HitAnyEnemy,
			},
		},
		new Objective
		{
			Slot = RunnerData.ObjectiveSlot.B,
			Name = "Marksman",
			Description = "{0} پرنده را کثیف کن",
			Target = new ObjectiveTarget
			{
				Kind = RunnerData.ObjectiveKind.HitAnyAirEnemy,
			},
		},
		new Objective
		{
			Slot = RunnerData.ObjectiveSlot.B,
			Name = "Marksman",
			Description = "{0} آدم را کثیف کن",
			Target = new ObjectiveTarget
			{
				Kind = RunnerData.ObjectiveKind.HitAnyGroundEnemy,
			},
		},
		new Objective
		{
			Slot = RunnerData.ObjectiveSlot.B,
			Name = "Marksman",
			Description = "{0} پرنده را کثیف کن",
			Target = new ObjectiveTarget
			{
				Kind = RunnerData.ObjectiveKind.HitAnyAirEnemy,
			},
		},

	};
}