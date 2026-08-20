// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// Keeps a UI <see cref="RectTransform"/> aligned to a world-space point on the battle canvas.
    /// </summary>
    public class WorldCanvasAnchor : MonoBehaviour
    {
        private Transform worldTarget;
        private Vector3 worldOffset;
        private Canvas canvas;
        private RectTransform rectTransform;

        public void Configure(Transform target, Vector3 offset, Canvas battleCanvas)
        {
            worldTarget = target;
            worldOffset = offset;
            canvas = battleCanvas;
            rectTransform = GetComponent<RectTransform>();
            UpdatePosition();
        }

        private void LateUpdate()
        {
            if (worldTarget == null || canvas == null || rectTransform == null)
                return;

            UpdatePosition();
        }

        private void UpdatePosition()
        {
            var cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
            if (cam == null || worldTarget == null)
                return;

            var worldPoint = worldTarget.position + worldOffset;
            var screenPoint = cam.WorldToScreenPoint(worldPoint);
            var canvasRect = canvas.transform as RectTransform;
            if (canvasRect == null)
                return;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, screenPoint, cam, out var localPoint))
                rectTransform.localPosition = localPoint;
        }
    }
}
