using UnityEngine;
using System.Collections;

public class TrashBag : PickUp {

	public Vector3 m_throwVelocity;
	public bool isInTruck = false; 
	public bool isThrown = false; 

	public int count;
	// Use this for initialization
	void Start () {
		m_fudgeScale = 0.85f;
		//m_currentHealth = 30f;

		count = 0;

	}

	// Why override if you're not overriding it with anything!? -sam 19/01/2017
	/*	public override void Update()
		{

			base.Update();
		}*/

	public override void UpdatePosition() {

	}

	public override void LateUpdate() {
		m_renderer.sortingOrder += 1;
		if (m_holder != null && !inPickupAnim) {
			transform.position = m_holder.m_heldItemPos.position;
			m_zDepth = m_holder.m_zDepth;
			m_renderer.sortingOrder = m_holder.m_renderer.sortingOrder + 1;
		} else if (!inPickupAnim) {
			transform.position += m_throwVelocity * Time.deltaTime;
			if (m_throwVelocity.x > 0f) {
				m_throwVelocity.x -= 4f * Time.deltaTime;
			}
			if (m_throwVelocity.x < 0f) {
				m_throwVelocity.x += 4f * Time.deltaTime;
			}

			if (transform.position.y <= TargetYPos) {

				//ObjectManager.Destroy(gameObject);

				transform.position = new Vector3(transform.position.x, TargetYPos, transform.position.z);
				m_throwVelocity = Vector3.zero;
				m_anim.SetBool("InAir", false);
				isThrown = false;
			} else {
				if (isInTruck && !isThrown)  {
					m_throwVelocity = Vector3.zero;
				} else {
					m_throwVelocity.y -= 15f * Time.deltaTime;
				}
			}
		}
		
		base.LateUpdate();
	}

	public override void Pickup(Hero holder) {
		m_holder = holder;
		inPickupAnim = true;
		startPos = transform.position;
		StartCoroutine(PickupAnimation());
	}


	public override IEnumerator PickupAnimation() {
		for (int i = 0; i < 22; i++) {
			yield return new WaitForEndOfFrame();
		}
		t = 0f;
		while (t <= 0.1f) {
			t += Time.deltaTime;
			transform.position = Vector3.Lerp(startPos, m_holder.m_heldItemPos.position, t / 0.1f);
			yield return new WaitForEndOfFrame();
		}
		transform.position = m_holder.m_heldItemPos.position;
		m_anim.SetBool("Carried", true);
		inPickupAnim = false;
	}

	public void Throw(bool right) {
		m_holder = null;
		m_anim.SetBool("Carried", false);
		m_anim.SetBool("InAir", true);
		m_renderer.flipX = !right;
		if (right)
		{
			m_throwVelocity = Vector3.right * 7f + Vector3.up * 4f;
		}
		else
		{
			m_throwVelocity = Vector3.left * 7f + Vector3.up * 4f;
		}
	}
	Vector3 endPos;

	public override void Hit(Actor source, float damage = 40f) {  
		m_anim.SetTrigger("Hit");
		StartCoroutine(Break());

		GetComponentInChildren<FruitSpawner>().SpawnFruit(this);

		count++;
	}

	public IEnumerator Break() {
		yield return new WaitForSeconds (m_anim.GetCurrentAnimatorStateInfo (0).length - 0.2f);
		ObjectManager.Destroy(gameObject);
	}

	public IEnumerator ThrowAfterDelay(bool right) {
		startPos = transform.position;
		if (right) {
			endPos = startPos - new Vector3(0.4f, 0.2f,0f);
		} else {
			endPos = startPos - new Vector3(-0.4f, 0.2f, 0f);
		}
		inPickupAnim = true;
		for (int i = 0; i < 2; i++) {
			yield return new WaitForEndOfFrame();
		}
		t = 0f;
		while (t <= 0.1f) {
			t += Time.deltaTime;
			transform.position = Vector3.Lerp(startPos, endPos, t / 0.1f);
			yield return new WaitForEndOfFrame();
		}

		for (int i = 0; i < 10; i++) {
			yield return new WaitForEndOfFrame();
		}
		inPickupAnim = false;
		transform.position = endPos;
		Throw(right);
	}
}
