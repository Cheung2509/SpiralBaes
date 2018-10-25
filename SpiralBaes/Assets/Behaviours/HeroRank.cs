using UnityEngine;
using System.Collections;

// Part of the ranking system which we're removing -sam 19/01/2017
public class HeroRank : MonoBehaviour {
	public uint xp = 0;
	public uint level = 0;
	
	public void AddXP(uint _xp) {
		xp += _xp;
	}

	uint xpToLevelUp {
		get {
			return (uint)(level * Mathf.Log(level * 10)) * 100;
		}
	}

	float damageModifier {
		get {
			return 1f;
		}
	}
}
