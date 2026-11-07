// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Random = System.Random;

namespace CCGKit
{
    /// <summary>
    /// This component brings together the map generator, map view and map tracker in a concerted
    /// effort to provide the map functionality to the current Unity scene.
    /// </summary>
    public class MapScreen : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private MapGenerator mapGenerator;
        [SerializeField]
        private MapView mapView;
        [SerializeField]
        private MapTracker mapTracker;

        [Header("Session")]
        [SerializeField]
        private string contextId = PlayContext.Kitchen;
        [SerializeField]
        private string mapPrefKey = PlayContext.KitchenMapPref;
        [SerializeField]
        private string saveDataPrefKey = PlayContext.KitchenSavePref;
        [SerializeField]
        private string mapSceneName = "Map";
        [SerializeField]
        private bool showResetContextButton = false;
#pragma warning restore 649

        private Random rng;

        private void Awake()
        {
            SceneManager.sceneUnloaded += scene => {
                DOTween.KillAll();
            };

            EnsureSession();
        }

        private void Start()
        {
            rng = new Random();

            if (showResetContextButton)
                BuildResetContextButton();

            LoadAndPresentMap();
        }

        public GameInfo EnsureSession()
        {
            return GameInfo.EnsureSession(contextId, mapPrefKey, saveDataPrefKey, mapSceneName);
        }

        public void ResetContext()
        {
            PlayerPrefs.DeleteKey(mapPrefKey);
            PlayerPrefs.DeleteKey(saveDataPrefKey);
            PlayerPrefs.Save();

            var gameInfo = FindFirstObjectByType<GameInfo>();
            if (gameInfo != null)
                Destroy(gameInfo.gameObject);

            EnsureSession();
            rng = new Random();
            LoadAndPresentMap();
        }

        private void LoadAndPresentMap()
        {
            var map = LoadMap();
            mapView.ShowMap(map);
            mapTracker.TrackMap(map, rng);
            SaveMap(map);

            var gameInfo = EnsureSession();
            if (gameInfo.PlayerWonEncounter)
            {
                var mapNode = map.GetNode(gameInfo.PlayerCoordinate);
                map.Path.Add(mapNode.Coordinate);

                mapView.SetReachableNodes();
                mapView.SetLineColors();
                var mapNodeView = mapView.GetNode(gameInfo.PlayerCoordinate);
                mapNodeView.ShowSwirl();
                SaveMap(map);

                var camPos = Camera.main.transform.position;
                camPos.y = mapNodeView.transform.position.y;
                Camera.main.transform.position = camPos;

                SavePlayerData(gameInfo.SaveData);
            }
            else
            {
                LoadPlayerData();

                foreach (var coordinate in map.Path)
                {
                    var mapNode = mapView.GetNode(coordinate);
                    mapNode.ShowSwirl();

                    var camPos = Camera.main.transform.position;
                    camPos.y = mapNode.transform.position.y;
                    Camera.main.transform.position = camPos;
                }
            }
        }

        private Map LoadMap()
        {
            if (PlayerPrefs.HasKey(mapPrefKey))
            {
                var json = PlayerPrefs.GetString(mapPrefKey);
                return JsonUtility.FromJson<Map>(json);
            }

            var map = mapGenerator.GenerateMap(rng);
            return map;
        }

        private void SaveMap(Map map)
        {
            var json = JsonUtility.ToJson(map, true);
            PlayerPrefs.SetString(mapPrefKey, json);
            PlayerPrefs.Save();
        }

        private void LoadPlayerData()
        {
            if (PlayerPrefs.HasKey(saveDataPrefKey))
            {
                var json = PlayerPrefs.GetString(saveDataPrefKey);
                var saveData = JsonUtility.FromJson<SaveData>(json);
                var gameInfo = EnsureSession();
                gameInfo.SaveData = saveData;
            }
        }

        private void SavePlayerData(SaveData data)
        {
            var json = JsonUtility.ToJson(data, true);
            PlayerPrefs.SetString(saveDataPrefKey, json);
            PlayerPrefs.Save();
        }

        private void BuildResetContextButton()
        {
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
                return;

            var canvasRect = canvas.GetComponent<RectTransform>();
            if (canvasRect.localScale == Vector3.zero)
                canvasRect.localScale = Vector3.one;

            var btnGo = new GameObject("ResetContextButton", typeof(RectTransform), typeof(Image), typeof(Button));
            btnGo.transform.SetParent(canvas.transform, false);

            var btnRect = btnGo.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0f, 0.5f);
            btnRect.anchorMax = new Vector2(0f, 0.5f);
            btnRect.pivot = new Vector2(0.5f, 0.5f);
            btnRect.sizeDelta = new Vector2(308.61f, 100f);
            btnRect.anchoredPosition = new Vector2(154.3f, -416f);

            var image = btnGo.GetComponent<Image>();
            image.color = new Color(0.45f, 0.22f, 0.22f, 1f);

            var button = btnGo.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(ResetContext);

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(btnGo.transform, false);
            var labelRect = labelGo.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var label = labelGo.AddComponent<TextMeshProUGUI>();
            label.text = "Reiniciar contexto";
            label.fontSize = 28;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
            if (TMP_Settings.defaultFontAsset != null)
                label.font = TMP_Settings.defaultFontAsset;
        }
    }
}
