using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager {
	// So does this mean that every actor has their own object pool? That's a little weird, surely we want pools per type, not per actor? -sam 19/01/2017
	private static Dictionary<Actor, ObjectPool> pools = new Dictionary<Actor, ObjectPool>();

	/// <summary>
	/// 
	/// </summary>
	/// <param name="gameObject"></param>
	/// <param name="position"></param>
	/// <returns></returns>
	public static Actor Instantiate(GameObject gameObject, Vector3 position, Quaternion rotation) {
		Actor instance = null;

		Actor actor = gameObject.GetComponent<Actor>();
		if (gameObject != null) {
			ObjectPool pool = GetObjectPool(actor);
			instance = pool.NextObject(position, rotation);
		} else {

			instance = Instantiate(gameObject, position, rotation) as Actor;
			instance.transform.position = position;
		}
		return instance;
	}

	public static void Destroy(GameObject gameObject) { 
		Actor actor = gameObject.GetComponent<Actor>();
		if (actor != null) {
			//actor.Discard();
			actor.gameObject.SetActive(false); // I have literally no idea why this needed an interface class. -sam 19/01/2017
		} else {
			GameObject.Destroy(gameObject);
		}
	}

	private static ObjectPool GetObjectPool(Actor reference) {
		ObjectPool pool = null;
		if (pools.ContainsKey(reference)) {
			pool = pools[reference];
		} else {
			GameObject poolContainer = new GameObject(reference.gameObject.name + "ObjectPool");
			pool = poolContainer.AddComponent<ObjectPool>();
			pool.prefab = reference;
			pools.Add(reference, pool);
		}

		return pool;
	}

}
