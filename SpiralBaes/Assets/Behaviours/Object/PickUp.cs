using UnityEngine;
using System.Collections;

// Potential issue, pickups are actors, even though actors are written to be characters?? What??? -sam 19/01/2017
public class PickUp : Actor {

	public Hero m_holder;
	protected bool inPickupAnim = false;

	private float timer = 0;
	public float distance = 20f;

	public override void Update() {
		// TODO: This is a bad way of doing timers - requires an addition operation and a comparison every frame. Ideally change this to fCurTime >= fTargetTime -sam 19/01/2017
		timer += Time.deltaTime; 

		if(timer > 2f) {
			if(Vector3.Distance(this.transform.position, CameraController.Instance.m_averagePlayerPos) > distance) {
				ObjectManager.Destroy (gameObject);
			}

			timer = 0;
		}

		base.Update();
	}

	public override void UpdatePosition() {

	}

	public override void LateUpdate() {
		m_renderer.sortingOrder += 1; // Constantly increase our sorting order!? -sam 19/01/2017

		if (m_holder != null && !inPickupAnim) {
			transform.position = m_holder.m_heldItemPos.position;
			m_zDepth = m_holder.m_zDepth;
			m_renderer.sortingOrder = m_holder.m_renderer.sortingOrder + 1;
		} else if (!inPickupAnim) {
			if (transform.position.y <= TargetYPos) {
				transform.position = new Vector3 (transform.position.x, TargetYPos, transform.position.z);
			}
		} else {
			m_velocity = Vector3.zero;
		}

		base.LateUpdate();
	}

	public Vector3 startPos;
	public float t = 0f; // What the hell is this? What is "t"? (probably time, but descriptive names please!) -sam 19/01/2017

	public virtual void Pickup(Hero holder) {
		m_holder = holder;
		inPickupAnim = true;
		startPos = transform.position;
		StartCoroutine(PickupAnimation());
	}

	public virtual IEnumerator PickupAnimation() {
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
		inPickupAnim = false;
	}
}
