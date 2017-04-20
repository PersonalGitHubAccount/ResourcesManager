using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Packager : Editor 
{
	public static string sourcePath = Application.dataPath + "/Resources";
	const string AssetBundleOutPath = "Assets/StreamingAssets";

	[MenuItem("Tools/AssetBundle/Build")]
	public static void BuildAssetBundle()
	{
		ClearAssetBundlesName ();
		Pack (sourcePath);
		string outputPath = Path.Combine (AssetBundleOutPath,Platform.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget));

		if(!Directory.Exists(outputPath))
		{
			Directory.CreateDirectory (outputPath);
		}

		//BuildPipeline.BuildAssetBundles (outputPath,0,EditorUserBuildSettings.activeBuildTarget);
		//BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneOSXIntel64);
        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);


        AssetDatabase.Refresh ();
		Debug.Log ("打包完成");
	}

	static void ClearAssetBundlesName()
	{
		int length = AssetDatabase.GetAllAssetBundleNames ().Length;
		Debug.Log (length);
		string[] oldAssetBundleNames = new string[length];

		for (int i = 0; i < length; i++)
		{
			oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
		}

		for (int j = 0; j < oldAssetBundleNames.Length; j++) 
		{
			AssetDatabase.RemoveAssetBundleName (oldAssetBundleNames [j], true);
		}

		length = AssetDatabase.GetAllAssetBundleNames ().Length;
		Debug.Log (length);
	}

	static void Pack(string source)
	{
		DirectoryInfo floder = new DirectoryInfo (source);
		FileSystemInfo[] files = floder.GetFileSystemInfos ();

		int length = files.Length;

		for (int i = 0; i < length; i++) 
		{
			if (files [i] is DirectoryInfo) 
			{
				Pack (files [i].FullName);
			} 
			else
			{
				if(!files[i].Name.EndsWith(".meta"))
				{
					file (files[i].FullName);
				}
			}
		}
	}

	static void file(string source)
	{
		string _source = Replace(source);
		string _assetPath = "Assets" + _source.Substring (Application.dataPath.Length);
		string _assetPath2 = _source.Substring (Application.dataPath.Length + 1);

		AssetImporter assetImporter = AssetImporter.GetAtPath (_assetPath);
		string assetName = _assetPath2.Substring (_assetPath2.IndexOf("/") + 1);

		assetName = assetName.Replace (Path.GetExtension(assetName), ".unity3d");
		assetImporter.assetBundleName = assetName;
	}

	static string Replace(string s)
	{
		return s.Replace ("\\","/");
	}

}

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