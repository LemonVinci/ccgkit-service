// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using UnityEngine;

namespace CCGKit
{
    /// <summary>
    /// The node view is the on-screen representation of a map node.
    /// </summary>
    public class NodeView : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private SpriteRenderer spriteRenderer;
        [SerializeField]
        private SpriteRenderer swirlSpriteRenderer;

        [SerializeField]
        private float selectedScaleFactor = 1.2f;
        [SerializeField]
        private float bossScaleFactor = 2f;

        [SerializeField]
        private Color lockedColor;
        [SerializeField]
        private Color visitedColor;
#pragma warning restore 649

        public Node Node;

        private Vector3 initialScale;
        private float mouseDownTime;

        private const float MaxClickDuration = 0.5f;

        private void Awake()
        {
            swirlSpriteRenderer.enabled = false;
        }

        public void Initialize(Node node, NodeConfig config)
        {
            Node = node;
            spriteRenderer.sprite = config.Sprite;

            if (node.Type == NodeType.Boss)
                transform.localScale *= bossScaleFactor;
            initialScale = spriteRenderer.transform.localScale;
        }

        public void SetState(NodeViewState state)
        {
            switch (state)
            {
                case NodeViewState.Locked:
                    spriteRenderer.DOKill();
                    spriteRenderer.color = lockedColor;
                    break;

                case NodeViewState.Visited:
                    spriteRenderer.DOKill();
                    spriteRenderer.color = visitedColor;
                    break;

                case NodeViewState.Reachable:
                    spriteRenderer.color = lockedColor;
                    spriteRenderer.DOKill();
                    spriteRenderer.DOColor(visitedColor, 0.5f).SetLoops(-1, LoopType.Yoyo);
                    break;
            }
        }

        private void OnMouseEnter()
        {
            spriteRenderer.transform.DOKill();
            spriteRenderer.transform.DOScale(initialScale * selectedScaleFactor, 0.3f);
        }

        private void OnMouseExit()
        {
            spriteRenderer.transform.DOKill();
            spriteRenderer.transform.DOScale(initialScale, 0.3f);
        }

        private void OnMouseUp()
        {
            mouseDownTime = Time.time;
        }

        private void OnMouseDown()
        {
            if (Time.time - mouseDownTime < MaxClickDuration)
                FindFirstObjectByType<MapTracker>().SelectNode(this);
        }

        public void ShowSwirl()
        {
            swirlSpriteRenderer.enabled = true;
            swirlSpriteRenderer.color = visitedColor;
        }
    }
}