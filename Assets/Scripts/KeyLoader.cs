using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KeyLoader : MonoBehaviour
{
    public string FileName = "APIKey.txt";
    public StringEvent KeyLoaded;
    // Start is called before the first frame update
    void Start()
    {
        string keyPath = Path.Combine(Application.persistentDataPath, FileName);
        string key;
        if (File.Exists(keyPath))
        {
            key = File.ReadAllText(keyPath);
        }
        else
        {
            key = "REPLACE_YOUR_GOOGLE_API_KEY";
            File.WriteAllText(keyPath, key);
            Debug.LogError("Please open this file and replace your google api key");
            Debug.LogWarning(keyPath);
        }
        KeyLoaded?.Invoke(key);
    }
}