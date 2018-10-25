//**********************************/
//* ScriptableObjectUtility.cs
//* Created by Jake Baugh 
//**********************************/
using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Scriptable object utility.
/// </summary>
public class ScriptableObjectUtility {


	/// <summary>
	/// Create new asset from <see cref="ScriptableObject"/> type with unique name at
	/// selected folder in project window. Asset creation can be cancelled by pressing
	/// escape key when asset is initially being named.
	/// </summary>
	/// <typeparam name="T">Type of scriptable object.</typeparam>
	public static void CreateAsset<T>() where T : ScriptableObject 
	{
		ScriptableObject asset = ScriptableObject.CreateInstance<T>();
		AssetDatabase.CreateAsset(asset, "Assets/" + typeof(T).Name + ".asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();

		Selection.activeObject = asset;
	}

}
