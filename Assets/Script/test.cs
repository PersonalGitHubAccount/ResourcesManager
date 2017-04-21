using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class test : MonoBehaviour 
{


	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.P))
		{
			Caching.CleanCache ();                                            //清除缓存，即通过www.LoadFromCacheOrDownload记载的资源，目录为Application.persistentDataPath--各个系统的沙盒目录
			Resources.UnloadUnusedAssets ();                         //卸载用不到的资源

            
			string loadPath = "Windows/Windows";
			string asName = "Windows";
			StartCoroutine (AssetBundleLoader.Instance.Load(loadPath, asName,typeof(GameObject),CallBack,false));
		}


        if (Input.GetKeyDown(KeyCode.L))
        {
            Caching.CleanCache();
            Resources.UnloadUnusedAssets();
            Debug.Log("123456");
            string loadpath = "Windows/Windows";
            string asname = "woman";
            string post = ".assetbundle";
			StartCoroutine(AssetLoader.Instance.Load(loadpath, PlatForm.Windows, asname, post, typeof(GameObject), CallBack, false, 0));
        }
	}


	private void CallBack(UnityEngine.Object obj)
	{
        Debug.Log(obj.name);

        GameObject go = GameObject.Instantiate((GameObject)obj);
        go.AddComponent<rotatetest>();
    }
		

}// End Class
