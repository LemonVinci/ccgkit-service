// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CCGKit
{
    /// <summary>
    /// This class is responsible for managing the transitions between scenes
    /// that are performed in the game via a classic fade to/from black.
    /// </summary>
    public class Transition : MonoBehaviour
    {
        private static GameObject canvasObject;

        private GameObject overlay;
        
        private readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        private void Awake()
        {
            canvasObject = new GameObject("TransitionCanvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            DontDestroyOnLoad(canvasObject);
        }

        public static void LoadLevel(string level, float duration, Color fadeColor)
        {
            var fade = new GameObject("Transition");
            fade.AddComponent<Transition>();
            fade.GetComponent<Transition>().StartFade(level, duration, fadeColor);
            fade.transform.SetParent(canvasObject.transform, false);
            fade.transform.SetAsLastSibling();
        }

        private void StartFade(string level, float duration, Color fadeColor)
        {
            StartCoroutine(RunFade(level, duration, fadeColor));
        }

        private IEnumerator RunFade(string level, float duration, Color fadeColor)
        {
            var bgTex = new Texture2D(1, 1);
            bgTex.SetPixel(0, 0, fadeColor);
            bgTex.Apply();

            overlay = new GameObject();
            var image = overlay.AddComponent<Image>();
            var rect = new Rect(0, 0, bgTex.width, bgTex.height);
            var sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);
            image.material.mainTexture = bgTex;
            image.sprite = sprite;
            var newColor = image.color;
            image.color = newColor;
            image.canvasRenderer.SetAlpha(0.0f);

            overlay.transform.localScale = new Vector3(1, 1, 1);
            overlay.GetComponent<RectTransform>().sizeDelta = canvasObject.GetComponent<RectTransform>().sizeDelta;
            overlay.transform.SetParent(canvasObject.transform, false);
            overlay.transform.SetAsFirstSibling();

            var time = 0.0f;
            var halfDuration = duration / 2.0f;
            while (time < halfDuration)
            {
                time += Time.deltaTime;
                image.canvasRenderer.SetAlpha(Mathf.InverseLerp(0, 1, time / halfDuration));
                yield return waitForEndOfFrame;
            }

            image.canvasRenderer.SetAlpha(1.0f);
            yield return waitForEndOfFrame;

            SceneManager.LoadScene(level);

            time = 0.0f;
            while (time < halfDuration)
            {
                time += Time.deltaTime;
                image.canvasRenderer.SetAlpha(Mathf.InverseLerp(1, 0, time / halfDuration));
                yield return waitForEndOfFrame;
            }

            image.canvasRenderer.SetAlpha(0.0f);
            yield return waitForEndOfFrame;

            Destroy(canvasObject);
        }
    }
}