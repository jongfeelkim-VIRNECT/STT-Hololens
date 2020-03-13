using Assets.Scripts.SpeechToText.JsonData;
using Assets.Scripts.TextToSpeech.JsonData;
using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Input = Assets.Scripts.TextToSpeech.JsonData.Input;

namespace Assets.Scripts.SpeechToText
{
    public class SpeechToText : MonoBehaviour
    {
        public string Base64DecodedContent { set; get; }
        public string APIKey { set; get; }

        public StringEvent OnTranscriptReceived;

        public void StartSpeechToText()
        {
            StartCoroutine(GoogleSpeechToText(Base64DecodedContent, transcript =>
            {
                OnTranscriptReceived?.Invoke(transcript);
            }));
        }

        IEnumerator GoogleSpeechToText(string base64DecodedContent, Action<string> callbackContent)
        {
            // Create json object
            STTRequestBody stt = new STTRequestBody()
            {
                 config = new Config() { languageCode = "ko" },
                 audio = new Audio() { content = base64DecodedContent }
            };

            string requestBody = JsonUtility.ToJson(stt);
            Debug.Log("[SpeechToText] request body json: " + requestBody);

            string requestUri = $"https://speech.googleapis.com/v1p1beta1/speech:recognize?key={APIKey}";
            //Debug.Log("[SpeechToText] request uri: " + requestUri);

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
                    Debug.Log("[SpeechToText] response body json: " + responseBody);

                    STTResponseBody sttResponse = JsonUtility.FromJson<STTResponseBody>(responseBody);
                    callbackContent?.Invoke(sttResponse.results[0].alternatives[0].transcript);
                }
            }
        }
    }
}