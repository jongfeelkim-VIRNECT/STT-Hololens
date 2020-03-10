using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneController : MonoBehaviour
{
    private float waitForEndOfSpeech;
    private AudioSource audioSource;
    private string microphoneDevice;

    public StringEvent OnRecognizeSpeechProcessing;
    public StringEvent OnRecognizeSpeechFail;

    public string APIKey { set; get; }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Debug.Log(APIKey);
        StartToSpeech();
    }

    public void StartToSpeech()
    {
        foreach (string microphoneDevice in Microphone.devices)
        {
            Debug.Log($"Detected microphone deivce: {microphoneDevice}");
        }
        microphoneDevice = Microphone.devices.Length > 0 ? Microphone.devices[0] : null;
        Debug.Log($"Selected microphone deivce: {microphoneDevice}");

        audioSource.clip = Microphone.Start(microphoneDevice, true, 20, 16000);
        audioSource.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { }
        Debug.Log("start playing... position is " + Microphone.GetPosition(null));
        audioSource.volume = 0.1f;
        audioSource.Play();
    }

    private float maxSum = 0.0f;
    // Update is called once per frame
    void Update()
    {
        if (audioSource.isPlaying)
        {
            float[] spectrum = new float[256];
            AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
            float sum = spectrum.Sum();
            if (sum > maxSum)
            {
                maxSum = sum;
                Debug.Log(maxSum);
            }

            if (sum > 0.02)
            {
                Debug.Log(sum);
                maxSum = 0;
                waitForEndOfSpeech = 0;
            }
            else
            {
                waitForEndOfSpeech += Time.deltaTime;
            }

            if (waitForEndOfSpeech > 2)
            {
                waitForEndOfSpeech = 0;
                Debug.Log("Microphone.End");
                Microphone.End(microphoneDevice);
                SavWav.Save("Recorded.wav", audioSource.clip);
                audioSource.volume = 1.0f;
                audioSource.Stop();

                string wavFilePath = Path.Combine(Application.persistentDataPath, "Recorded.wav");
                string base64decoded = GetBase64DecodeFromWavFile(wavFilePath);
            }
        }        
    }

    private string GetBase64DecodeFromWavFile(string wavFilePath)
    {
        OnRecognizeSpeechProcessing?.Invoke("Processing Google.Speech.API...");

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