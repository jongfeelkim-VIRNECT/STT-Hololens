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

    //public StringEvent OnRecordSpeechProcessing;
    //public StringEvent OnRecordSpeechFinished;
    public StringEvent OnRecordSpeechFinished;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartToSpeech();
    }

    public void StartToSpeech()
    {
        //OnRecordSpeechProcessing?.Invoke("OnRecordSpeechProcessing");

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

                //OnRecordSpeechFinished?.Invoke("OnRecordSpeechFinished");

                string wavFilePath = Path.Combine(Application.persistentDataPath, "Recorded.wav");
                string base64decoded = GoogleAPIHelper.GetBase64DecodeFromWavFile(wavFilePath);

                OnRecordSpeechFinished?.Invoke(base64decoded);
            }
        }        
    }
}