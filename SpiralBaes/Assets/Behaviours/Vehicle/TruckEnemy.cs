using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TruckEnemy : Truck
{
	public bool thrown;
	public TrashBag trashBag;
	public SpriteRenderer binPanel;
	public SpriteRenderer smoke;
	Transform trashEmitter;

	public float wobbleSpeed;
	public float wobbleMax;
	public float wobbleOffset;

	float timer = 0f;

	List<TrashBag> trashBags = new List<TrashBag>();

	// Use this for initialization
	void Start () {

		trashEmitter = transform.GetChild (3);
		StartCoroutine (SpawnTrashBag ());
	}
	
	// Update is called once per frame
	public override void Update () {



		timer += Time.deltaTime;

		if (timer > Random.Range(0.2f, 0.6f))
		{
			StartCoroutine (SpawnTrashBag ());
			timer = 0f;
		}

		smoke.sortingOrder = m_renderer.sortingOrder;
		binPanel.sortingOrder = m_renderer.sortingOrder + 21;

		base.Update ();


		float sin = Mathf.Sin(Time.time * wobbleSpeed) * wobbleMax;

		float y = sin - wobbleOffset;
		transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);

		Mathf.Clamp (sin, 0f, y);
	}

	public override void Move()
	{
		StartCoroutine(LaneDash ());
		base.Move ();
	}

	public IEnumerator LaneDash()
	{
		
		yield return new WaitForSeconds (15f);
		m_zDepth = Mathf.Lerp (m_zDepth, 0.24f, Time.deltaTime);
		yield return new WaitForSeconds (15f);
		m_zDepth = Mathf.Lerp (m_zDepth, 0.64f, Time.deltaTime);

	}

	public IEnumerator SpawnTrashBag()
	{
		yield return new WaitForSeconds(0.4f);
		Actor instance = ObjectManager.Instantiate(trashBag.gameObject, trashEmitter.transform.position, Quaternion.identity);
		
        TrashBag trash = instance.GetComponent<TrashBag>();
            
        trash.isInTruck = true;

		ThrowTrashBag (trash);


	}

	public void ThrowTrashBag(TrashBag trash)
	{
		trash.isInTruck = false;
		trash.isThrown = true;
	
		if (RandomChance (0, 2)) 
		{
			trash.m_zDepth = this.m_zDepth + Random.Range (0.08f, 0.2f);
		} 
		else 
        {
			trash.m_zDepth = this.m_zDepth - Random.Range (0.08f, 0.2f);
		}

		if (thrown) 
        {
			trash.m_anim.SetBool ("InAir", true);
			trash.m_throwVelocity = Vector3.left * Random.Range(5f, 10f) + Vector3.up * Random.Range(3f, 10f);
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
