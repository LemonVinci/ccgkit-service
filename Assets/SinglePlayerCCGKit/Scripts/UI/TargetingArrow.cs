// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// Targeting arrow from the raised card to the cursor; highlights the dish station under the pointer.
    /// </summary>
    public class TargetingArrow : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private GameObject bodyPrefab;
        [SerializeField]
        private GameObject headPrefab;

        [SerializeField]
        private GameObject topLeftPrefab;
        [SerializeField]
        private GameObject topRightPrefab;
        [SerializeField]
        private GameObject bottomLeftPrefab;
        [SerializeField]
        private GameObject bottomRightPrefab;
#pragma warning restore 649

        private const int NumPartsTargetingArrow = 17;
        private readonly List<GameObject> arrow = new List<GameObject>(NumPartsTargetingArrow);

        private DishTarget selectedDishTarget;
        private GameObject topLeftVertex;
        private GameObject topRightVertex;
        private GameObject bottomLeftVertex;
        private GameObject bottomRightVertex;

        private Camera mainCamera;
        private LayerMask targetLayer;
        private DishSystem dishSystem;
        private bool isArrowEnabled;

        private void Start()
        {
            for (var i = 0; i < NumPartsTargetingArrow - 1; i++)
            {
                var body = Instantiate(bodyPrefab, gameObject.transform);
                arrow.Add(body);
            }

            var head = Instantiate(headPrefab, gameObject.transform);
            arrow.Add(head);

            foreach (var part in arrow)
                part.SetActive(false);

            topLeftVertex = Instantiate(topLeftPrefab, gameObject.transform);
            topRightVertex = Instantiate(topRightPrefab, gameObject.transform);
            bottomLeftVertex = Instantiate(bottomLeftPrefab, gameObject.transform);
            bottomRightVertex = Instantiate(bottomRightPrefab, gameObject.transform);

            DisableSelectionBox();

            mainCamera = Camera.main;
            targetLayer = 1 << LayerMask.NameToLayer("Enemy");
            dishSystem = FindFirstObjectByType<DishSystem>();
        }

        public void EnableArrow(bool arrowEnabled)
        {
            isArrowEnabled = arrowEnabled;
            foreach (var part in arrow)
                part.SetActive(arrowEnabled);

            if (!arrowEnabled)
                UnselectTarget();
        }

        private void LateUpdate()
        {
            if (!isArrowEnabled)
                return;

            var mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var mouseX = mousePos.x;
            var mouseY = mousePos.y;

            var hitInfo = Physics2D.Raycast(mousePos, Vector3.forward, Mathf.Infinity, targetLayer);
            if (hitInfo.collider != null)
            {
                var dishTarget = hitInfo.collider.GetComponent<DishTarget>();
                if (dishTarget != null &&
                    (dishTarget != selectedDishTarget || selectedDishTarget == null))
                    SelectTarget(dishTarget);
            }
            else
            {
                UnselectTarget();
            }

            const float centerX = 0.0f;
            const float centerY = -4.0f;

            var controlAx = centerX - (mouseX - centerX) * 0.3f;
            var controlAy = centerY + (mouseY - centerY) * 0.8f;
            var controlBx = centerX + (mouseX - centerX) * 0.1f;
            var controlBy = centerY + (mouseY - centerY) * 1.4f;

            for (var i = 0; i < arrow.Count; i++)
            {
                var part = arrow[i];

                var t = (i + 1) * 1.0f / arrow.Count;
                var tt = t * t;
                var ttt = tt * t;
                var u = 1.0f - t;
                var uu = u * u;
                var uuu = uu * u;

                var arrowX = uuu * centerX +
                             3 * uu * t * controlAx +
                             3 * u * tt * controlBx +
                             ttt * mouseX;
                var arrowY = uuu * centerY +
                             3 * uu * t * controlAy +
                             3 * u * tt * controlBy +
                             ttt * mouseY;

                arrow[i].transform.position = new Vector3(arrowX, arrowY, 0.0f);

                float lenX;
                float lenY;
                if (i > 0)
                {
                    lenX = arrow[i].transform.position.x - arrow[i - 1].transform.position.x;
                    lenY = arrow[i].transform.position.y - arrow[i - 1].transform.position.y;
                }
                else
                {
                    lenX = arrow[i + 1].transform.position.x - arrow[i].transform.position.x;
                    lenY = arrow[i + 1].transform.position.y - arrow[i].transform.position.y;
                }

                part.transform.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(lenX, lenY) * Mathf.Rad2Deg);

                part.transform.localScale = new Vector3(
                    1.0f - 0.03f * (arrow.Count - 1 - i),
                    1.0f - 0.03f * (arrow.Count - 1 - i),
                    0);
            }
        }

        private void SelectTarget(DishTarget dishTarget)
        {
            selectedDishTarget = dishTarget;

            var boxCollider = dishTarget.GetComponent<BoxCollider2D>();
            var size = boxCollider.size;
            var offset = boxCollider.offset;

            var topLeftLocal = offset + new Vector2(-size.x * 0.5f, size.y * 0.5f);
            var topLeftWorld = dishTarget.transform.TransformPoint(topLeftLocal);
            var topRightLocal = offset + new Vector2(size.x * 0.5f, size.y * 0.5f);
            var topRightWorld = dishTarget.transform.TransformPoint(topRightLocal);
            var bottomLeftLocal = offset + new Vector2(-size.x * 0.5f, -size.y * 0.5f);
            var bottomLeftWorld = dishTarget.transform.TransformPoint(bottomLeftLocal);
            var bottomRightLocal = offset + new Vector2(size.x * 0.5f, -size.y * 0.5f);
            var bottomRightWorld = dishTarget.transform.TransformPoint(bottomRightLocal);

            EnableSelectionBox();

            topLeftVertex.transform.position = topLeftWorld;
            topRightVertex.transform.position = topRightWorld;
            bottomLeftVertex.transform.position = bottomLeftWorld;
            bottomRightVertex.transform.position = bottomRightWorld;

            if (dishSystem != null)
                dishSystem.SetFocusedStation(dishTarget.StationIndex);
        }

        private void UnselectTarget()
        {
            selectedDishTarget = null;
            DisableSelectionBox();
        }

        private void EnableSelectionBox()
        {
            topLeftVertex.SetActive(true);
            topRightVertex.SetActive(true);
            bottomLeftVertex.SetActive(true);
            bottomRightVertex.SetActive(true);
        }

        private void DisableSelectionBox()
        {
            topLeftVertex.SetActive(false);
            topRightVertex.SetActive(false);
            bottomLeftVertex.SetActive(false);
            bottomRightVertex.SetActive(false);
        }
    }
}
