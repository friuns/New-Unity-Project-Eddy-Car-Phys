using System;
using System.Diagnostics;
using UnityEngine;
using MonoBehaviour = Photon.MonoBehaviour;

public class AssetDictTest:MonoBehaviour
{
    public TextAsset AssetDictionary;
    public void Start()
    {
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
            Test();
        
    }
    private void Test()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        var ss = AssetDictionary.text.Split(new[] { "; \r\n", ";\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        print(ss.Length);
        print(sw.Elapsed.TotalMilliseconds);
    }
}