using UnityEngine;
using System.Collections;

public class Vehicle : Actor {

	public float m_maxHorizontalSpeed;
	public float m_targetHorizontalSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public override void  Update () {
	
		base.Update ();
	}

	/// <summary>
	/// Move the specified Vehicle.
	/// </summary>
	/// <param name="movement">Movement.</param>
	public override void Move()
	{

	}
}
