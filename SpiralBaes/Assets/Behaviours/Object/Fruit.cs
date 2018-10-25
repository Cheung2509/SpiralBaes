using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fruit : PickUp {

	public List<Sprite> foodSprites = new List<Sprite>(); 

	public Vector3 m_throwVelocity;
	public bool isInSpawner = false; 
	public bool isThrown = false; 

	void Start () {
		GetComponentInChildren<SpriteRenderer> ().sprite = foodSprites [Random.Range (0, foodSprites.Count)];
		m_fudgeScale = 0.8f;
	}
	
	// Update is called once per frame
	public override void Update () {
		if (m_holder && !inPickupAnim) {
			gameObject.SetActive (false);
			m_holder.m_currentHealth += 10f;
		}
		base.Update ();
	}

	public override void UpdatePosition() {

	}


	public override void LateUpdate() {
		//m_renderer.sortingOrder += 1;

		transform.position += m_throwVelocity * Time.deltaTime;
		if (m_throwVelocity.x > 0f) {
			m_throwVelocity.x -= 4f * Time.deltaTime;
		}
		if (m_throwVelocity.x < 0f) {
			m_throwVelocity.x += 4f * Time.deltaTime;
		}

		if (transform.position.y <= TargetYPos) {
			transform.position = new Vector3 (transform.position.x, TargetYPos, transform.position.z);
			m_throwVelocity = Vector3.zero;
			//m_anim.SetBool ("InAir", false);
		} else {
			
			if (isInSpawner && !isThrown) {

				m_throwVelocity = Vector3.zero;
			} else {
				m_throwVelocity.y -= 45f * Time.deltaTime;
			}
		}
		base.LateUpdate();
	}

}

