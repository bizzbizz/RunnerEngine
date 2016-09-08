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
				}
			}

			return _instance;
		}
	}

}
