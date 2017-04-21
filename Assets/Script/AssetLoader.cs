///===================================================================================================================================================
///---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
///--------------------------------------------------ASSETBUNDLE下载器------------------------------------------------------------------------------------------------------------------------------------------------------------------------
///--------------------------------------------------lcx--2017--04--20---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
///------------------------------------------------???将检查版本与下载分开，一开始检查并下载所有资源到本地，直接从本地读取-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
///===================================================================================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


/// <summary>
/// Call back func.回调函数
/// </summary>
public delegate void CallBackFunc(UnityEngine.Object obj);


public class AssetLoader 
{
    /// <summary>
    /// 单例
    /// </summary>
    public static readonly AssetLoader Instance = new AssetLoader();

    /// <summary>
    /// 路径
    /// </summary>
    private string pathlocal = Application.streamingAssetsPath; //"D:/Soft/Apache/WWW/StreamingAssets";// Application.streamingAssetsPath;

	/// <summary>
	/// The path internet.
	/// </summary>
    private string pathInternet = "D:/Soft/Apache/WWW/StreamingAssets";


	/// <summary>
	/// The asset bundle reference dic.
	/// </summary>
    private Dictionary<string, AssetBundle> assetBundleRefDic = new Dictionary<string, AssetBundle>();



    /// <summary>
    /// 根据平台获取路径
    /// </summary>
    private string GetThePathWithPlatformLocal
    {
        get
        {
            if (Application.isEditor)
            {
				#if UNITY_EDITOR_OSX
                return "file:///" + pathlocal;
				#elif UNITY_EDITOR
				return "file://" + pathlocal;
				#endif
            }
            else
            {
                return pathlocal;
            }
        }
    }


	/// <summary>
	/// Gets the get the path with platform internet.
	/// </summary>
	/// <value>The get the path with platform internet.</value>
    private string GetThePathWithPlatformInternet
    {
        get
        {
            if(Application.isEditor)
            {
                return "file:///" + pathInternet;
            }
            else
            {
                return pathInternet;
            }
        }
    }


    private string PathUsed;

    /// <summary>
    /// 下载器
    /// </summary>
    /// <param name="assetPath">总的依赖资源路径</param>
    /// <param name="platform">当前平台</param>
    /// <param name="assetName">目标资源名称</param>
    /// <param name="assetPostfix">目标资源后缀</param>
    /// <param name="type">目标类型</param>
    /// <param name="callbackfunc">回调函数</param>
    /// <param name="isUnload">是否完全卸载（慎用true）</param>
    /// <param name="version">资源版本</param>
    /// <returns></returns>
    public IEnumerator Load(string assetPath, PlatForm platform, string assetName, string assetPostfix, Type type, CallBackFunc callbackfunc, bool isUnload, int version)
    {
      // while(!download)
      // {
      //     yield return null;
      // }

        yield return new WaitForEndOfFrame();

        string loadPath = GetThePathWithPlatformLocal;

        //string loadPath = PathUsed;

        string loadUrl = Path.Combine(loadPath, assetPath);
		Debug.LogWarning(loadUrl);
        string targetAssetName = string.Format("{0}{1}", assetName, assetPostfix);

        Debug.LogWarning(targetAssetName);

        AssetBundle loadedBundle = GetBundleFromDic(assetName);
        if (loadedBundle != null)
        {
            ExcuteCallBackFunc(loadedBundle, assetName, type, callbackfunc, isUnload);
            yield return null;
        }
        else
        {
            WWW mainfestLoader = WWW.LoadFromCacheOrDownload(loadUrl, version);

            yield return mainfestLoader;

            if (!string.IsNullOrEmpty(mainfestLoader.error))
            {
                Debug.LogWarning(mainfestLoader.error);
                yield return null;
            }
            else
            {
                AssetBundle mainfestAsset = mainfestLoader.assetBundle;

                AssetBundleManifest mainfest = (AssetBundleManifest)mainfestAsset.LoadAsset("AssetBundleManifest");

                Debug.LogWarning(mainfest.name);

                //获取总的依赖
                string[] dependency = mainfest.GetAllDependencies(targetAssetName);

                AssetBundle[] bundles = new AssetBundle[dependency.Length];

                for (int i = 0; i < dependency.Length; i++)
                {
                    //加载依赖资源
                    string singleUrl = string.Format("{0}/{1}/{2}", loadPath, GetPlatformStr(platform), dependency[i]);
                    WWW singleLoader = WWW.LoadFromCacheOrDownload(singleUrl, mainfest.GetAssetBundleHash(dependency[i]));
                    yield return singleLoader;

                    bundles[i] = singleLoader.assetBundle;
                }

                string targetUrl = Path.Combine(loadPath, string.Format("{0}/{1}", platform, targetAssetName));

                //加载目标资源
                WWW targetLoader = WWW.LoadFromCacheOrDownload(targetUrl, version);
                yield return targetLoader;

                //根据名称加载目标
                UnityEngine.Object obj = targetLoader.assetBundle.LoadAsset(assetName, type);


                //执行回调函数
                if (callbackfunc != null)
                {
                    callbackfunc(obj);
                }


                if (!isUnload)
                {
                    if (!assetBundleRefDic.ContainsKey(assetName))
                    {
                        Debug.Log("add");
                        assetBundleRefDic.Add(assetName, targetLoader.assetBundle);
                    }
                }
                else
                {
                    //从内存中卸载用不到的资源文件，释放内存=======================================================
                    Resources.UnloadUnusedAssets();
                    mainfestAsset.Unload(false);
                    for (int i = 0; i < bundles.Length; i++)
                    {
                        bundles[i].Unload(false);
                    }

                    targetLoader.assetBundle.Unload(false);
                }
            }
        }
    }

    /// <summary>
    /// 获取平台名称
    /// </summary>
    /// <param name="platform">平台类型</param>
    /// <returns>返回字符串</returns>
    private string GetPlatformStr(PlatForm platform)
    {
        string result = null;
        switch (platform)
        {
            case PlatForm.Windows:
                result = "Windows";
                break;
            case PlatForm.OSX:
                result = "OSX";
                break;
            case PlatForm.IOS:
                result = "IOS";
                break;
            case PlatForm.Android:
                result = "Android";
                break;
            default:
                break;
        }
        return result;
    }

    /// <summary>
    /// 从字典中获取资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    private AssetBundle GetBundleFromDic(string assetName)
    {
        AssetBundle bundle = null;
        if(assetBundleRefDic.TryGetValue(assetName, out bundle))
        {
            return bundle;
        }
        return null;
    }


    /// <summary>
    /// 执行回调函数
    /// </summary>
    /// <param name="bundle">AssetBundle</param>
    /// <param name="assetName">资源名称</param>
    /// <param name="type">资源类型</param>
    /// <param name="callbackfunc">回调函数</param>
    /// <param name="isUnload">是否卸载资源</param>
    private void ExcuteCallBackFunc(AssetBundle bundle, string assetName, Type type, CallBackFunc callbackfunc, bool isUnload)
    {
        UnityEngine.Object obj = bundle.LoadAsset(assetName, type);

        if(callbackfunc != null)
        {
            Debug.Log("从字典中加载");
            callbackfunc(obj);
        }

        if(isUnload)
        {
            bundle.Unload(true);
        }
    }

    bool download;
	/// <summary>
	/// Checks the version.检查资源版本
	/// </summary>
	/// <returns>The version.</returns>
    public IEnumerator CheckVersion()
    {
        JsonResolver.Instance.CheckVersion(GetThePathWithPlatformLocal + "/resourcesInfo.json", GetThePathWithPlatformInternet + "/resourcesInfo.json");
        while(JsonResolver.Instance.localVersion == 0 &&JsonResolver.Instance.internetVersion == 0)
        {
            download = false;
            yield return null;
        }

        if(JsonResolver.Instance.localVersion == JsonResolver.Instance.internetVersion)
        {
            PathUsed = GetThePathWithPlatformLocal;
        }
        else
        {
            PathUsed = GetThePathWithPlatformInternet;
        }
        download = true;
    }


}// End Class

/// <summary>
/// 平台枚举
/// </summary>
public enum PlatForm
{
     Windows,
     OSX,
     IOS,
     Android
}
