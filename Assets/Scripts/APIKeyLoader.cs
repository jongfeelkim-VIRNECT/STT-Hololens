using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class APIKeyLoader : MonoBehaviour
{
    public StringEvent APIKeyLoaded;
    // Start is called before the first frame update
    void Start()
    {
        string apiKeyPath = Path.Combine(Application.persistentDataPath, "APIKey.txt");
        string apiKey;
        if (File.Exists(apiKeyPath))
        {
            apiKey = File.ReadAllText(apiKeyPath);
        }
        else
        {
            apiKey = "REPLACE_YOUR_GOOGLE_API_KEY";
            File.WriteAllText(apiKeyPath, apiKey);
            Debug.LogError("Please open this file and replace your google api key");
            Debug.LogWarning(apiKeyPath);
        }
        APIKeyLoaded?.Invoke(apiKey);
    }
}