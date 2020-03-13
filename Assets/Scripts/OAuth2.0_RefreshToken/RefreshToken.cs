using Assets.Scripts.OAuth2._0_RefreshToken;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.OAuth2RefreshToken
{
    [RequireComponent(typeof(FileLoader))]
    public class RefreshToken : MonoBehaviour
    {
        public StringEvent OnNewAccessTokenReceived;

        private RefreshTokenResponseBody lastReceivedBody;
        private FileLoader keyLoader;

        public void StartRefereshToken()
        {
            keyLoader = GetComponent<FileLoader>();

            // check first request or expire date
            if (lastReceivedBody?.ExpiresDate > DateTime.Now)
            {
                OnNewAccessTokenReceived?.Invoke(lastReceivedBody?.access_token);
            }
            else
            {
                RefreshTokenParameters rtp = JsonUtility.FromJson<RefreshTokenParameters>(keyLoader.StoredContents);
                IDictionary<string, string> parameters = new Dictionary<string, string>
                {
                    ["client_id"] = rtp.client_id,
                    ["client_secret"] = rtp.client_secret,
                    ["grant_type"] = rtp.grant_type,
                    ["refresh_token"] = rtp.refresh_token
                };

                StartCoroutine(NewAccessToken(parameters, newAccessToken => OnNewAccessTokenReceived?.Invoke(newAccessToken)));
            }
        }

        IEnumerator NewAccessToken(IDictionary<string, string> parameters, Action<string> callbackNewAccessToken)
        {
            string parameterValue = string.Join("&", parameters.Select(item => $"{item.Key}={item.Value}").ToArray());
            string requestUri = $"https://oauth2.googleapis.com/token?{parameterValue}";
            Debug.Log("[SpeechToText] request uri: " + requestUri);

            using (UnityWebRequest uwr = UnityWebRequest.Post(requestUri, ""))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    string responseBody = uwr.downloadHandler.text;
                    Debug.Log("[SpeechToText] response body json: " + responseBody);

                    lastReceivedBody = JsonUtility.FromJson<RefreshTokenResponseBody>(responseBody);
                    callbackNewAccessToken?.Invoke(lastReceivedBody.access_token);
                }
            }
        }
    }
}