using UnityEngine;
using System.Collections;

public class Shutter : MonoBehaviour {

	public Transform phaseText;
	public Transform leftDivider;
	public Transform rightDivider;

	public Transform evilTruckFlourish;

	private float _transitionTime = 0.75f;

	public void OnEnable() {
		DoTransitionIn();
	}

	public void DoTransitionIn() {

		phaseText.gameObject.SetActive(true);

		iTween.MoveTo(leftDivider.gameObject, iTween.Hash("x", 0, "islocal", true, "time", _transitionTime, "easeType", "easeOutBounce"));
		iTween.MoveTo(rightDivider.gameObject, iTween.Hash("x", 0, "islocal", true, "time", _transitionTime, "easeType", "easeOutBounce"));

		evilTruckFlourish.gameObject.SetActive(true);

		Invoke("DoTransitionOut", 3f);
	}

	public void DoTransitionOut() {

		phaseText.gameObject.SetActive(false);

		iTween.MoveTo(leftDivider.gameObject, iTween.Hash("x", -10, "islocal", true, "time", _transitionTime / 1.5f, "easeType", "easeInOutCubic"));
		iTween.MoveTo(rightDivider.gameObject, iTween.Hash("x", 10, "islocal", true, "time", _transitionTime / 1.5f, "easeType", "easeInOutCubic"));

		Invoke("Finalise", 1.5f);
	}

	private void Finalise() {

		this.gameObject.SetActive(false);
		evilTruckFlourish.gameObject.SetActive(false);
	}
}
