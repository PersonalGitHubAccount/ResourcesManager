using UnityEngine;
using UnityEditor;
using System.IO;



public class Packager : Editor 
{
	/// <summary>
	/// The source path.
	/// </summary>
	public static string sourcePath = Application.dataPath + "/Resources";

	/// <summary>
	/// The asset bundle out path.
	/// </summary>
	const string AssetBundleOutPath = "Assets/StreamingAssets";

	/// <summary>
	/// Builds the asset bundle.
	/// </summary>
	[MenuItem("Tools/AssetBundle/Build")]
	public static void BuildAssetBundle()
	{
		//清除包名
		ClearAssetBundlesName ();

		//设置包名
		Pack (sourcePath);

		//输出路径
		string outputPath = Path.Combine (AssetBundleOutPath,Platform.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget));

		if(!Directory.Exists(outputPath))
		{
			Directory.CreateDirectory (outputPath);
		}

        Debug.Log("当前打包平台为[" + EditorUserBuildSettings.activeBuildTarget + "]");

		//打包assetbundle
		//BuildPipeline.BuildAssetBundles (outputPath,0,EditorUserBuildSettings.activeBuildTarget);
		BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        //BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

		//刷新资源
        AssetDatabase.Refresh ();
		Debug.Log ("[打包完成]");
	}

	/// <summary>
	/// Clears the name of the asset bundles.
	/// </summary>
	static void ClearAssetBundlesName()
	{
		//获取所有包名数组长度
		int length = AssetDatabase.GetAllAssetBundleNames ().Length;
		Debug.Log ("清除[" + length + "]个assetbundleName");

		//获取旧包的包名数组
		string[] oldAssetBundleNames = new string[length];

		for (int i = 0; i < length; i++)
		{
			oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
		}

		//移除已经存在的包名
		for (int j = 0; j < oldAssetBundleNames.Length; j++) 
		{
			AssetDatabase.RemoveAssetBundleName (oldAssetBundleNames [j], true);
		}

		//获取清除后包名数组长度
		length = AssetDatabase.GetAllAssetBundleNames ().Length;

		Debug.Log ("剩余[" + length + "]个AssetBundleName");
	}

	/// <summary>
	/// Pack the specified source.
	/// </summary>
	/// <param name="source">Source.</param>
	static void Pack(string source)
	{
        Debug.Log("[开始设置新的AssetBundleName]");
		//获取资源路径的文件夹信息
		DirectoryInfo floder = new DirectoryInfo (source);

		//获取该文件夹下的文件信息
		FileSystemInfo[] files = floder.GetFileSystemInfos ();

		//记录该文件夹下的文件个数
		int length = files.Length;

		//遍历该文件夹下的每个文件
		for (int i = 0; i < length; i++) 
		{
			//如果自文件是文件夹，
			if (files [i] is DirectoryInfo) 
			{
				//继续打开，执行此函数
				Pack (files [i].FullName);
			} 
			else
			{
				//后缀名不是meta的重新设置包名
				if(!files[i].Name.EndsWith(".meta"))
				{
					//重新设置包名
					//file (files[i].FullName);
					SetAssetBundleName(files[i].FullName,floder.Name);
				}
			}
		}
        Debug.Log("[新的AssetBundelName设置完成]");
	}

	/// <summary>
	/// File the specified source.
	/// </summary>
	/// <param name="source">Source.</param>
	static void file(string source)
	{
		//转换分隔符的格式
		string _source = Replace(source);
		//资源路径
		string _assetPath = "Assets" + _source.Substring (Application.dataPath.Length);
		//asset目录下的资源路径
		string _assetPath2 = _source.Substring (Application.dataPath.Length + 1);
		//根据资源路径获取assetimporter
		AssetImporter assetImporter = AssetImporter.GetAtPath (_assetPath);
		//asset目录下第一个／分割符后为资源的完整名称
		string assetName = _assetPath2.Substring (_assetPath2.IndexOf("/") + 1);
		//包名，替换后缀名为".unity3d"
		assetName = assetName.Replace (Path.GetExtension(assetName), ".unity3d");
		//设置资源的assetBundle
		assetImporter.assetBundleName = assetName;
	}

    /// <summary>
    /// 根据不同的文件夹名称，设置assetbundle的名称
    /// </summary>
    /// <param name="source">路径</param>
    /// <param name="floderName">文件夹名</param>
	static void SetAssetBundleName(string source, string floderName)
	{
        //替换符号
		string _source = Replace (source);
        //资源路径
		string _assetPath = "Assets" + _source.Substring (Application.dataPath.Length);
        //获取assetimporter
		AssetImporter assetImporter = AssetImporter.GetAtPath (_assetPath);
        //生成assetbundle的名称
		string assetName = string.Format ("{0}.{1}", floderName, PostDix.assetbundle);
        //设置assetbundle的名称
		assetImporter.assetBundleName = assetName;
	}

	/// <summary>
	/// Replace the specified s.
	/// </summary>
	/// <param name="s">S.</param>
	static string Replace(string s)
	{
		//将"\\"替换为"/"
		return s.Replace ("\\","/");
	}

}// End Class


/// <summary>
/// Platform.
/// </summary>
public class Platform
{
	public static string GetPlatformFolder(BuildTarget target)
	{
		switch (target)
		{
		case BuildTarget.Android:
			return "Android";

		case BuildTarget.iOS:
			return "IOS";

		case BuildTarget.StandaloneWindows:
		case BuildTarget.StandaloneWindows64:
			return "Windows";

		case BuildTarget.StandaloneOSXIntel:
		case BuildTarget.StandaloneOSXIntel64:
		case BuildTarget.StandaloneOSXUniversal:
			return "OSX";

		default:
			return null;
		}
	}
}


/// <summary>
/// 后缀名--枚举
/// </summary>
public enum PostDix
{
	unity3d,
	assetbundle,
	bytes
}