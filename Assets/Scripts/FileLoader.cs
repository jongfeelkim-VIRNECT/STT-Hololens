using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileLoader : MonoBehaviour
{
    public string FileName;
    public StringEvent ContentsLoaded;
    public string StoredContents { private set; get; }
     
    // Start is called before the first frame update
    private void Start()
    {
        string contentsPath = Path.Combine(Application.persistentDataPath, FileName);
        string contents;
        if (File.Exists(contentsPath))
        {
            contents = File.ReadAllText(contentsPath);
        }
        else
        {
            contents = "REPLACE_YOUR_CONTENTS";
            File.WriteAllText(contentsPath, contents);
            Debug.LogError("Please open this file and replace your appropriate contents");
            Debug.LogWarning(contentsPath);
        }
        StoredContents = contents;
        ContentsLoaded?.Invoke(contents);
    }
}