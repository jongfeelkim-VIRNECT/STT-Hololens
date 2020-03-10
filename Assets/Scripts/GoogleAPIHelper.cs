using System;
using System.IO;

public class GoogleAPIHelper
{
    public static string GetBase64DecodeFromWavFile(string wavFilePath)
    {
        byte[] wavByteArray = File.ReadAllBytes(wavFilePath);

        // Remove empty space, decoded string "A"
        string contents = Convert.ToBase64String(wavByteArray);
        int lastIndex = 0;
        for (int i = contents.Length - 1; i != 0; i--)
        {
            if (contents[i] != 'A')
            {
                lastIndex = i + 1;    // Prevent remove last index string
                break;
            }
        }
        string removedEmptyContents = contents.Substring(0, lastIndex);
        //File.WriteAllText(Path.Combine(Application.persistentDataPath, "Base64Decoded.txt"), removedEmptyContents);
        return removedEmptyContents;
    }
}
