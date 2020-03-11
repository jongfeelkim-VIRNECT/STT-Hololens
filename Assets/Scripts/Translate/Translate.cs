using Assets.Scripts.Translate.JsonData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Translate
{
    public class Translate : MonoBehaviour
    {
        public string SourceText { set; get; }
        public string SourceLanguageCode = "ko";
        public string TargetLanguageCode = "en";
        public string AccessToken { set; get; }

        public StringEvent OnTranslatedTextReceived;

        public void StartTranslate()
        {
            StartCoroutine(GoogleTranslate(SourceText, AccessToken, translatedText =>
            {
                OnTranslatedTextReceived?.Invoke(translatedText);
                //string savedWavFilePath = SaveToWavFile(audioContent);
                //StartCoroutine(GetAudioClip(savedWavFilePath));
            }));
        }

        IEnumerator GoogleTranslate(string sourceText, string accessToken, Action<string> callbackText)
        {
            // Create json object
            TranslateRequestBody translate = new TranslateRequestBody()
            {
                contents = new List<string>() { sourceText },
                mimeType = "text/plain",
                sourceLanguageCode = SourceLanguageCode,
                targetLanguageCode = TargetLanguageCode
            };

            string requestBody = JsonUtility.ToJson(translate);
            Debug.Log("[Translate] request body json: " + requestBody);

            string requestUri = "https://translation.googleapis.com/v3/projects/stt-hololens:translateText";
            //Debug.Log("[Translate] request uri: " + requestUri);

            using (UnityWebRequest uwr = UnityWebRequest.Post(requestUri, ""))
            {
                uwr.SetRequestHeader("Authorization", "Bearer " + accessToken);
                uwr.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestBody));
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    string responseBody = uwr.downloadHandler.text;
                    Debug.Log("[Translate] response body json: " + responseBody);

                    TranslateResponseBody ac = JsonUtility.FromJson<TranslateResponseBody>(responseBody);
                    callbackText?.Invoke(ac.translations[0].translatedText);
                }
            }
        }
    }
}