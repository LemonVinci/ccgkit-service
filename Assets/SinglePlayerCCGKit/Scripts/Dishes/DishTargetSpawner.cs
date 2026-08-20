// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// Spawns <see cref="DishTarget"/> colliders in the battle scene (Enemy layer) for arrow targeting.
    /// </summary>
    public class DishTargetSpawner : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private Vector2 colliderSize = new Vector2(2.6f, 2.6f);

        [Tooltip("World positions for each dish station (centered on the battle view).")]
        [SerializeField]
        private Vector2[] stationPositions =
        {
            new Vector2(-3f, 0f),
            new Vector2(0f, 0f),
            new Vector2(3f, 0f)
        };
#pragma warning restore 649

        private void Start()
        {
            StartCoroutine(SpawnWhenReady());
        }

        private IEnumerator SpawnWhenReady()
        {
            DishSystem dishSystem = null;
            while (dishSystem == null || !dishSystem.HasStations)
            {
                dishSystem = FindFirstObjectByType<DishSystem>();
                yield return null;
            }

            var count = dishSystem.StationCount;
            var positions = GetPositionsForCount(count);
            var enemyLayer = LayerMask.NameToLayer("Enemy");
            var dishSprite = Resources.Load<Sprite>("BattleDishArt");

            for (var i = 0; i < count; i++)
            {
                var go = new GameObject("DishTarget_" + i);
                go.transform.SetParent(transform, false);
                go.transform.position = new Vector3(positions[i].x, positions[i].y, 0f);
                if (enemyLayer >= 0)
                    go.layer = enemyLayer;

                var collider = go.AddComponent<BoxCollider2D>();
                collider.size = colliderSize;
                collider.isTrigger = false;

                var spriteRenderer = go.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = dishSprite;
                spriteRenderer.sortingOrder = 2;
                spriteRenderer.color = new Color(1f, 1f, 1f, dishSprite != null ? 0.55f : 0.35f);
                if (dishSprite == null)
                    spriteRenderer.color = new Color(0.35f, 0.3f, 0.28f, 0.7f);

                var labelGo = new GameObject("Label");
                labelGo.transform.SetParent(go.transform, false);
                labelGo.transform.localPosition = new Vector3(0f, colliderSize.y * 0.55f, 0f);
                var textMesh = labelGo.AddComponent<TextMesh>();
                textMesh.text = "Plato " + (i + 1);
                textMesh.fontSize = 32;
                textMesh.characterSize = 0.08f;
                textMesh.anchor = TextAnchor.MiddleCenter;
                textMesh.color = Color.white;
                textMesh.alignment = TextAlignment.Center;

                var target = go.AddComponent<DishTarget>();
                target.Configure(i);
            }
        }

        private Vector2[] GetPositionsForCount(int count)
        {
            switch (count)
            {
                case 1:
                    return new[] { new Vector2(0f, 0f) };
                case 2:
                    return new[] { new Vector2(-2.5f, 0f), new Vector2(2.5f, 0f) };
                default:
                    if (stationPositions != null && stationPositions.Length >= 3)
                        return stationPositions;
                    return new[] { new Vector2(-3f, 0f), new Vector2(0f, 0f), new Vector2(3f, 0f) };
            }
        }
    }
}
