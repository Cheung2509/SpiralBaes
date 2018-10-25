using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{

	public Actor prefab;

	private List<Actor> poolObjects = new List<Actor>();

	private Actor CreateInstance(Vector3 position, Quaternion rotation)
	{
		var clone = GameObject.Instantiate(prefab);
		clone.transform.position = position;
		clone.transform.parent = transform;
		clone.transform.rotation = rotation;

		poolObjects.Add(clone);

		return clone;
	}

	public Actor NextObject(Vector3 position, Quaternion rotation)
	{
		Actor instance = null;

		foreach (Actor actor in poolObjects)
		{
			if (actor.gameObject.activeSelf != true)
			{
				instance = actor;
				instance.transform.position = position;
			}
		}

		if (instance == null) {
			instance = CreateInstance(position, rotation);
		}

		// instance.Aquire();
		instance.gameObject.SetActive(true);
		return instance;
	}

}
