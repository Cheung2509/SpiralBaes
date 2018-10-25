using UnityEngine;
using UnityEngine.UI;
using Rewired;
using System.Collections;

public class UIHero : MonoBehaviour {

	public Hero hero;

	public GameObject healthbar;
	public Text[] join;

	private Player player;

	public bool active;

	// Use this for initialization
	void Start () {



		player = ReInput.players.GetPlayer (hero.m_controllerId);

		StartCoroutine (Join ());
	}
	
	// Update is called once per frame
	void Update () {
	
		if (!active)
		{
			healthbar.SetActive (false);
		}

		if (!hero.m_alive && player.GetButtonDown ("Start")) {

			PlayerJoin ();
		}

	}

	void PlayerJoin()
	{
		hero.m_alive = true;
		active = true;

		foreach(Text t in join)
		t.enabled = false;
		
		healthbar.SetActive (true);

		UIManager.Instance.GenerateHealth (hero.m_controllerId);

		hero.gameObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 1));
		hero.m_zDepth = 0.5f;

		hero.transform.position = new Vector3 (hero.transform.position.x, hero.TargetYPos, hero.transform.position.z);
		hero.gameObject.SetActive (hero.m_alive);
	}


	private IEnumerator Join() {

		while (!active) {

			yield return new WaitForSeconds(0.5f);

			foreach(Text t in join)
			t.gameObject.SetActive(!t.gameObject.activeSelf);

		}
	}


}
