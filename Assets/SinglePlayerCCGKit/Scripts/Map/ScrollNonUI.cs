// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// Utility component that provides a scrollable area without any dependencies on the UI system.
    /// </summary>
    public class ScrollNonUI : MonoBehaviour
    {
        public float TweenBackDuration = 0.3f;
        public Ease TweenBackEase;

        public bool FreezeX;
        public FloatMinMax XConstraints = new FloatMinMax();
        public bool FreezeY;
        public FloatMinMax YConstraints = new FloatMinMax();

        private Vector2 offset;
        private Vector3 pointerDisplacement;
        private float zDisplacement;
        private bool dragging;
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
            zDisplacement = -mainCamera.transform.position.z + transform.position.z;
        }

        private void OnMouseDown()
        {
            pointerDisplacement = -transform.position + MouseInWorldCoords();
            transform.DOKill();
            dragging = true;
        }

        private void OnMouseUp()
        {
            dragging = false;
            TweenBack();
        }

        private void Update()
        {
            if (!dragging)
                return;

            var mousePos = MouseInWorldCoords();
            transform.position = new Vector3(
                FreezeX ? transform.position.x : mousePos.x - pointerDisplacement.x,
                FreezeY ? transform.position.y : mousePos.y - pointerDisplacement.y,
                transform.position.z);
        }

        private Vector3 MouseInWorldCoords()
        {
            var screenMousePos = Input.mousePosition;
            screenMousePos.z = zDisplacement;
            return mainCamera.ScreenToWorldPoint(screenMousePos);
        }

        private void TweenBack()
        {
            if (FreezeY)
            {
                if (transform.localPosition.x >= XConstraints.Min && transform.localPosition.x <= XConstraints.Max)
                    return;

                var targetX = transform.localPosition.x < XConstraints.Min ? XConstraints.Min : XConstraints.Max;
                transform.DOLocalMoveX(targetX, TweenBackDuration).SetEase(TweenBackEase);
            }
            else if (FreezeX)
            {
                if (transform.localPosition.y >= YConstraints.Min && transform.localPosition.y <= YConstraints.Max)
                    return;

                var targetY = transform.localPosition.y < YConstraints.Min ? YConstraints.Min : YConstraints.Max;
                transform.DOLocalMoveY(targetY, TweenBackDuration).SetEase(TweenBackEase);
            }
        }
    }
}