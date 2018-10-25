using UnityEngine;
using System.Collections;

public class Truck : Vehicle {

	public float acceleration = 0.2f;

	// Use this for initialization
	void Start () {
	
		m_horizontalSpeed = 0.005f;
	}
	
	// Update is called once per frame
	public override void Update () {
	

		Move ();
		base.Update ();
	}

	/// <summary>
	/// Move the specified Truck.
	/// </summary>
	public override void Move()
	{
		Vector3 addForce = Vector3.right * m_horizontalSpeed;
		m_velocity += addForce;
	}

	public void SpawnTrash()
	{

	}


	public void ThrowTrash()
	{

	}
		
}
