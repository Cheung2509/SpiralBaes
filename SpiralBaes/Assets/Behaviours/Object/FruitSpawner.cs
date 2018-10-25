using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FruitSpawner : MonoBehaviour {

	public Fruit fruit;
    public float min = 2;
    public int max = 5;

	public List<Fruit> fruits;

	public void SpawnFruit(TrashBag trash)
	{
		int count = 0;

		for (int i = 0; i < Random.Range (min, max); i++) 
		{
			Actor actor = ObjectManager.Instantiate (fruit.gameObject, transform.position, Quaternion.identity);

			Fruit f = actor.GetComponent<Fruit> ();
			fruits.Add (f);
            
			f.isInSpawner = true;	
			f.m_zDepth = trash.m_zDepth;

			ThrowFruit (f, trash.m_zDepth);

			count++;
		}

		trash.hit = false;

	}

	public void ThrowFruit(Fruit f, float zdepth)
	{

		f.isInSpawner = false;
		f.isThrown = true;

		if (RandomChance(0,2))
		{
			if (RandomChance(0,2))
			{
				f.m_throwVelocity = Vector3.left * Random.Range (2f, 4f) + Vector3.up * Random.Range (10f, 14f);
				f.m_zDepth = zdepth + Random.Range(0.05f, 0.3f);
				}
				else 
				{
					f.m_throwVelocity = Vector3.left * Random.Range (1f, 3f) + Vector3.up * Random.Range (6f, 10f);
					f.m_zDepth = zdepth - Random.Range(0.05f, 0.3f);
				}
			} 
			else 
			{
				if (RandomChance(0,2))
				{
					f.m_throwVelocity = Vector3.right * Random.Range (2f, 4f) + Vector3.up * Random.Range (10f, 14f);
					f.m_zDepth = zdepth + Random.Range(0.05f, 0.3f);
				}
				else 
				{
					f.m_throwVelocity = Vector3.left * Random.Range (1f, 3f) + Vector3.up * Random.Range (6f, 10f);
					f.m_zDepth = zdepth - Random.Range(0.05f, 0.3f);
				}
			} 
		}





	public bool RandomChance(int min, int max)
	{
		int r = Random.Range (min, max);

		if (r % 2 == 0) {
			return true;
		} else {

			return false;
		}
	}

}
