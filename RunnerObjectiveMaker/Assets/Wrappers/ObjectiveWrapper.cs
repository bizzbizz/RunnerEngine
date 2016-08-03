using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ObjectiveWrapper : MonoBehaviour
{
	public Objective Data;
	public bool HasError { get; set; }
	public bool HasDuplicate { get; set; }
	public List<ObjectiveWrapper> Requirements { get; set; }

	void Awake()
	{
		HasError = false;
		HasDuplicate = false;
		Requirements = new List<ObjectiveWrapper>();
	}

	void OnDrawGizmos()
	{
		if (Data == null) return;

		//colorize
		if (HasError)
			gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.red;
		else if(HasDuplicate)
			gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.blue;
		else
			gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.black;

		//name
		if (string.IsNullOrEmpty(Data.Name))
		{
			gameObject.name = "[untitled objective]";
			return;
		}
		if (gameObject.name != Data.Name)
			gameObject.name = Data.Name;


		//texts
		Handles.BeginGUI();

		Handles.Label(transform.position + new Vector3(-.9f, .5f), Data.Name);
		Handles.Label(transform.position + new Vector3(-.9f, .3f), Data.Description);

		Handles.DrawLine(transform.position + new Vector3(-.9f, .07f), transform.position + new Vector3(.9f, .07f));
		Handles.Label(transform.position + new Vector3(-.9f, .1f), "Target:");
		Handles.Label(transform.position + new Vector3(0, .1f), Data.TargetValue.ToString());
		Handles.Label(transform.position + new Vector3(-.9f, -.1f), Data.Scope.ToString());
		Handles.Label(transform.position + new Vector3(0, -.1f), Data.Kind.ToString());

		Handles.DrawLine(transform.position + new Vector3(-.9f, -.33f), transform.position + new Vector3(.9f, -.33f));
		Handles.Label(transform.position + new Vector3(-.9f, -.3f), "Rwd:");
		Handles.Label(transform.position + new Vector3(-.3f, -.3f), Data.Reward.Kind.ToString());
		Handles.Label(transform.position + new Vector3(.5f, -.3f), Data.Reward.Value.ToString());

		Handles.EndGUI();
	}
}
