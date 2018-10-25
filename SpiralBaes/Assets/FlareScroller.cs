using UnityEngine;
using System.Collections;

public class FlareScroller : MonoBehaviour {

	public float scrollSpeed = 10f;
	private MeshRenderer flareSprite;

	public void OnEnable() {

		flareSprite = this.GetComponent<MeshRenderer>();

		flareSprite.transform.localPosition = new Vector3(30, 0, 1);

		iTween.MoveTo(this.gameObject, iTween.Hash("x", 0, "islocal", true, "time", 0.5f, "easeType", "easeOutCubic"));

		Invoke("Finalise", 3f);
	}

	// Update is called once per frame
	void Update () {

		if (flareSprite) {
			float offset = Time.time * scrollSpeed;
			flareSprite.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
		}

	}

	public void Finalise() {
		iTween.MoveTo(this.gameObject, iTween.Hash("x", -30, "islocal", true, "time", 0.5f, "easeType", "easeInCubic"));
	}
}
