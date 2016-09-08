using UnityEngine;
using UnityEditor;
using RunnerData;

public class ObjectiveWrapper : MonoBehaviour
{
	public Objective Data;
	public bool HasError { get; set; }
	public bool HasDuplicate { get; set; }

	void Awake()
	{
		HasError = false;
		HasDuplicate = false;
	}
}
