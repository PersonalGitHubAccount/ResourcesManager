using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public enum LoadType
{
    Local,
    Internet
}


public class JsonResolver : MonoBehaviour
{

    public static JsonResolver Instance;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public int localVersion;

    public int internetVersion;



     IEnumerator LoadJson(string path,  int version)
    {
        WWW loader = new WWW(path);
        Debug.Log(path);
        yield return loader;

        if(loader.isDone)
        {
            string temp = loader.text;
            JsonData jsondata = JsonMapper.ToObject(temp);
            localVersion = (int)jsondata["version"];
            Debug.Log(localVersion);
            Debug.Log(localVersion + "======0000000=====" + internetVersion);
        }
    }


    IEnumerator LoadJson1(string path, int version)
    {
        WWW loader = new WWW(path);
        Debug.Log(path);
        yield return loader;

        if (loader.isDone)
        {
            string temp = loader.text;
            JsonData jsondata = JsonMapper.ToObject(temp);
            internetVersion = (int)jsondata["version"];
            Debug.Log(internetVersion);
            Debug.Log(localVersion + "======0000000=====" + internetVersion);
        }
    }

    public void CheckVersion(string local, string internet)
    {
        StartCoroutine(LoadJson(local, localVersion));
        StartCoroutine(LoadJson1(internet, internetVersion));
    }

}// End Class