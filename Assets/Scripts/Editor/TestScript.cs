using System.Collections;
using NUnit.Framework;
using Scripts.TextToSpeech;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestScript
    {
        private static string API_KEY = "REPLACE_YOUR_GOOGLE_API_KEY";
        private static string ACCESS_TOKEN = "REPLACE_YOUR_ACCESS_TOKEN";
        [UnityTest]
        public IEnumerator APIKey()
        {
            bool IsAsserted = false;

            GameObject go1 = new GameObject();
            KeyLoader loader = go1.AddComponent<KeyLoader>();
            loader.FileName = "APIKey.txt";
            loader.runInEditMode = true;
            loader.KeyLoaded = new StringEvent();
            loader.KeyLoaded.AddListener(apikey =>
            {
                Assert.AreNotEqual(apikey, API_KEY);
                API_KEY = apikey;
                IsAsserted = true;
            });

            // Wait for Assert result.
            while (!IsAsserted)
            {
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator AccessToken()
        {
            bool IsAsserted = false;

            GameObject go1 = new GameObject();
            KeyLoader loader = go1.AddComponent<KeyLoader>();
            loader.FileName = "AccessToken.txt";
            loader.runInEditMode = true;
            loader.KeyLoaded = new StringEvent();
            loader.KeyLoaded.AddListener(accessToken =>
            {
                Assert.AreNotEqual(accessToken, ACCESS_TOKEN);
                ACCESS_TOKEN = accessToken;
                IsAsserted = true;
            });

            // Wait for Assert result.
            while (!IsAsserted)
            {
                yield return null;
            }
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TextToSpeech()
        {
            if (API_KEY.Equals("REPLACE_YOUR_GOOGLE_API_KEY"))
            {
                Debug.LogWarning("Dependency of APIKeyTest()");
                yield return null;
            }
            else
            {
                GameObject go2 = new GameObject();
                TextToSpeech tts = go2.AddComponent<TextToSpeech>();
                AudioSource audioSource = go2.GetComponent<AudioSource>();
                tts.runInEditMode = true;
                tts.SourceText = "A UnityTest behaves like a coroutine in Play Mode";
                tts.APIKey = API_KEY;
                tts.StartTextToSpeech();

                // Wait for audio contents set to AudioClip.
                while (audioSource.clip == null)
                {
                    yield return null;
                }

                // You can hear tts.SourceText voice while audioSource.isPlaying
                audioSource.Play();
                while (audioSource.isPlaying)
                {
                    yield return null;
                }

                Assert.AreNotEqual(audioSource.clip, null);
            }
        }
    }
}
