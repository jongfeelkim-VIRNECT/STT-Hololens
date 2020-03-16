using Assets.Scripts.TextToSpeech.JsonData;
using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Input = Assets.Scripts.TextToSpeech.JsonData.Input;

namespace Scripts.TextToSpeech
{
    [RequireComponent(typeof(AudioSource))]
    public class TextToSpeech : MonoBehaviour
    {
        public string SourceText { set; get; }
        public string APIKey { set; get; }

        public AudioSource audioSource { private set; get; }

        // Start is called before the first frame update
        void Start() => audioSource = GetComponent<AudioSource>();

        public void StartTextToSpeech()
        {
            StartCoroutine(GoogleTextToSpeech(SourceText, audioContent =>
            {
                string savedWavFilePath = SaveToWavFile(audioContent);
                StartCoroutine(GetAudioClip(savedWavFilePath, audioSource => 
                {
                    audioSource.Play();
                }));
            }));
        }

        IEnumerator GoogleTextToSpeech(string sourceText, Action<string> callbackContent)
        {
            // Create json object
            TTSRequestBody tts = new TTSRequestBody()
            {
                input = new Input() { text = sourceText },
                voice = new Voice() { languageCode = "en-US", ssmlGender = "FEMALE" },
                audioConfig = new AudioConfig() { audioEncoding = "LINEAR16" }
            };

            string requestBody = JsonUtility.ToJson(tts);
            Debug.Log("[TextToSpeech] request body json: " + requestBody);

            string requestUri = $"https://texttospeech.googleapis.com/v1beta1/text:synthesize?key={APIKey}";
            //Debug.Log("[TextToSpeech] request uri: " + requestUri);

            using (UnityWebRequest uwr = UnityWebRequest.Post(requestUri, ""))
            {
                uwr.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestBody));
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    string responseBody = uwr.downloadHandler.text;
                    Debug.Log("[TextToSpeech] response body json: " + responseBody);

                    TTSResponseBody ac = JsonUtility.FromJson<TTSResponseBody>(responseBody);
                    callbackContent?.Invoke(ac.audioContent);
                }
            }
        }

        private string SaveToWavFile(string contents)
        {
            byte[] contentsByte = Convert.FromBase64String(contents);
            string savedWavFilePath = Path.Combine(Application.persistentDataPath, "TextToSpeech.wav");
            using (FileStream fs = File.Create(savedWavFilePath))
            {
                fs.Write(contentsByte, 0, contentsByte.Length);
            }
            return savedWavFilePath;
        }

        IEnumerator GetAudioClip(string savedWavFilePath, Action<AudioSource> callbackAudioSource)
        {
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(savedWavFilePath, AudioType.WAV))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    audioSource.clip = DownloadHandlerAudioClip.GetContent(uwr);
                    callbackAudioSource?.Invoke(audioSource);
                }
            }
        }
    }
}