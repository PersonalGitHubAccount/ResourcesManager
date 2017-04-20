using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetLoaderTest : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		StartCoroutine ("LoadAsset");
	}


	IEnumerator LoadAsset()
	{
		Debug.Log (Application.dataPath + "/StreamingAssets/OSX/cube.unity3d");
		WWW w3 = new WWW ("file://"+Application.dataPath + "/StreamingAssets/OSX/cube");


		yield return w3;


		if(w3.isDone)
		{
			Debug.Log (w3.assetBundle.mainAsset);

			GameObject go = w3.assetBundle.LoadAsset ("Cube") as GameObject;
			yield return Instantiate (go);

		}
	}
	

}
