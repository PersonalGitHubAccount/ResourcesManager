using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public delegate void CallBack(UnityEngine.Object obj);

public class AssetBundleLoader  
{

	public static readonly AssetBundleLoader Instance = new AssetBundleLoader ();

	private string path = Application.streamingAssetsPath;

	private Dictionary<string, AssetBundle> assetBundleRefDic = new Dictionary<string, AssetBundle> ();


	private string GetThePathWithPlatform
	{
		get
		{
			if (Application.isEditor)
			{
				return "file://" + path;
			}
			else 
			{
				return path;
			}
		}
	}


	public IEnumerator Load(string assetPath, string assetName, Type type, CallBack callback, bool isUnload)
	{
		string loadpath = GetThePathWithPlatform;
		string url = Path.Combine (loadpath, assetPath);

		AssetBundle ab = GetASFromUrl (url);

		if (ab != null)
        {
			GetObject (ab, assetName, type, callback, isUnload);
			yield return null;
		}
        else
        {
			WWW www = WWW.LoadFromCacheOrDownload (url, 1);
			yield return www;

			if (!string.IsNullOrEmpty (www.error))
            {
				Debug.Log (www.error);
				yield return null;
			}
            else
            {
				ab = www.assetBundle;

				//---------------------------------------------------------------------------------------------------------------------------
				Debug.Log (www.bytesDownloaded);
			
				AssetBundleManifest mainfest = (AssetBundleManifest)ab.LoadAsset ("AssetBundleManifest"); 
				Debug.Log (mainfest.name);
			
				string[] dps = mainfest.GetAllDependencies ("man.unity3d");
				AssetBundle[] asb = new AssetBundle[dps.Length];

				for (int i = 0; i < dps.Length; i++)
				{
					Debug.Log (dps[i]);
					string url1 = loadpath + "/Windows/" + dps[i];
					WWW loader1 = WWW.LoadFromCacheOrDownload (url1,mainfest.GetAssetBundleHash(dps[i]));
					yield return loader1;
					asb [i] = loader1.assetBundle;
					Debug.Log (asb[i].name);
				}

				string url2 = Path.Combine (loadpath, "Windows/man.unity3d");

				WWW w3 = WWW.LoadFromCacheOrDownload (url2,1);

				yield return w3;


                //根据名称加载目标
                UnityEngine.Object obj = w3.assetBundle.LoadAsset(assetName, type);

                //执行回调函数
                if (callback != null)
                {
                    callback(obj);
                }


                //AssetBundle a3 = w3.assetBundle;

                //GameObject go = a3.LoadAsset ("man") as GameObject;

                //if (go != null) 
                //{
                //	GameObject.Instantiate (go);
                //} 
                //else 
                //{
                //	Debug.LogWarning ("nothing loaded");
                //}

                w3.assetBundle.Unload (false);

			//	string[] dps = mainfest.GetAllDependencies ("sphere.unity3d.manifest");
			//	AssetBundle[] abs = new AssetBundle[dps.Length];
			//	for (int i = 0; i < dps.Length; i++) 
			//	{
			//		string dUrl = assetPath + dps[i];
			//	WWW dwww = WWW.LoadFromCacheOrDownload (dUrl,mainfest.GetAssetBundleHash(dps[i]));
			//	yield return dwww;
			//	abs [i] = dwww.assetBundle;
			//}
			//
			//WWW w3 = WWW.LoadFromCacheOrDownload (url,mainfest.GetAssetBundleHash("sphere.unity3d"),0);
			//
			//yield return w3;
			//
			//AssetBundle abb = w3.assetBundle;
			//
			//GameObject go = abb.LoadAsset ("sphere") as GameObject;
			//if(go != null)
			//{
			//	GameObject.Instantiate (go);
			//}
				//-----------------------------------------------------------------------------------------------------------------------------


				Resources.UnloadUnusedAssets ();

				GetObject (ab, assetName, type, callback, isUnload);

				if(!isUnload)
				{
					if(!assetBundleRefDic.ContainsKey(url))
					{
						Debug.Log ("add");
						assetBundleRefDic.Add (url, ab);
					}
				}
			}
		}
	
	}


	private void GetObject(AssetBundle ab, string assetName, Type type, CallBack callback, bool isUnload)
	{

		UnityEngine.Object obj = ab.LoadAsset (assetName, type);

		if(callback != null)
		{
			callback (obj);
		}

		if(isUnload)
		{
			ab.Unload (true);
		}
	}

	private AssetBundle GetASFromUrl(string url)
	{
		AssetBundle ab = null;
		if(assetBundleRefDic.TryGetValue(url,out ab))
		{
			return ab;
		}
		return null;
	}

}
