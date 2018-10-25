using UnityEngine;
using UnityEditor;
using System.Collections;


public class SuperTrashUtility
{

	[MenuItem("Assets/Create/Level")]
	public static void CreateLevel()
	{
		ScriptableObjectUtility.CreateAsset<LevelAsset>();
	}

	[MenuItem("Assets/Create/Hero")]
	public static void CreateHero()
	{
		ScriptableObjectUtility.CreateAsset<HeroAsset>();
	}



}
