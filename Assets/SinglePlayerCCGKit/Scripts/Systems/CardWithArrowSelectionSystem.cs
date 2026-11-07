// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace CCGKit
{
    public enum ArrowTargetKind
    {
        DishStation,
        Enemy
    }

    /// <summary>
    /// Creates a targeting arrow when the player selects a card, then plays it on the aimed target:
    /// a dish station in the kitchen modes, or a monster in ComboGame.
    /// </summary>
    public class CardWithArrowSelectionSystem : CardSelectionSystem
    {
#pragma warning disable 649
        [SerializeField]
        private Canvas cardPileCanvas;

        [Tooltip("Disable in game modes where cards are dropped on a play zone instead of aimed at a target.")]
        [SerializeField]
        private bool arrowEnabled = true;

        [Tooltip("What the arrow can be dropped on. Enemy is used by ComboGame.")]
        [SerializeField]
        private ArrowTargetKind targetKind = ArrowTargetKind.DishStation;
#pragma warning restore 649

        private TargetingArrow targetingArrow;
        private DishTarget highlightedDishTarget;
        private GameObject selectedEnemy;
        private Vector3 prevClickPos;
        private bool isArrowCreated;

        private const float CardSelectionDetectionOffset = 2.2f;
        private const float CardSelectionAnimationTime = 0.2f;
        private const float CardSelectionCanceledAnimationTime = 0.2f;
        private const float SelectedCardYOffset = -4.0f;

        protected override void Start()
        {
            base.Start();
            TargetLayer = 1 << LayerMask.NameToLayer("Enemy");
            targetingArrow = Object.FindFirstObjectByType<TargetingArrow>();
        }

        private void Update()
        {
            if (!arrowEnabled)
                return;

            if (cardPileCanvas.isActiveAndEnabled)
                return;

            if (TurnManagementSystem.IsEndOfGame())
                return;

            if (HandPresentationSystem.IsAnimating())
                return;

            if (Input.GetMouseButtonDown(0))
            {
                DetectCardSelection();
                DetectTargetSelection();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                DetectTargetSelection();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                DetectCardUnselection();
            }

            if (SelectedCard != null)
                UpdateTargetingArrow();
        }

        private void DetectCardSelection()
        {
            var mousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hitInfo = Physics2D.Raycast(mousePos, Vector3.forward, Mathf.Infinity, CardLayer);
            if (hitInfo.collider == null)
                return;

            var card = hitInfo.collider.GetComponent<CardObject>();
            if (card == null)
                return;

            var cardTemplate = card.Template;
            if (!CardUtils.CardCanBePlayed(cardTemplate, PlayerMana) || !CardNeedsTarget(cardTemplate))
                return;

            SelectedCard = hitInfo.collider.gameObject;
            SelectedCard.GetComponent<SortingGroup>().sortingOrder += 10;
            prevClickPos = mousePos;
        }

        private void DetectCardUnselection()
        {
            if (SelectedCard == null)
                return;

            var card = SelectedCard.GetComponent<CardObject>();
            SelectedCard.transform.DOKill();
            card.Reset(() =>
            {
                SelectedCard = null;
                isArrowCreated = false;
            });

            if (targetingArrow != null)
                targetingArrow.EnableArrow(false);
        }

        private void DetectTargetSelection()
        {
            if (SelectedCard == null)
                return;

            var mousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hitInfo = Physics2D.Raycast(mousePos, Vector3.forward, Mathf.Infinity, TargetLayer);
            if (hitInfo.collider == null)
                return;

            if (targetKind == ArrowTargetKind.Enemy)
            {
                DetectEnemySelection(hitInfo.collider.gameObject);
                return;
            }

            var dishTarget = hitInfo.collider.GetComponent<DishTarget>();
            if (dishTarget == null)
                return;

            highlightedDishTarget = dishTarget;
            if (!TryPlayIngredientOnDishTarget())
            {
                CancelIngredientTargeting();
                return;
            }

            PlaySelectedCard();
            ClearSelection();
        }

        private void DetectEnemySelection(GameObject hitObject)
        {
            var enemy = hitObject.GetComponent<CharacterObject>();
            if (enemy == null || enemy.IsDead)
                return;

            selectedEnemy = hitObject;
            PlaySelectedCard();
            ClearSelection();
        }

        private void ClearSelection()
        {
            SelectedCard = null;
            isArrowCreated = false;
            highlightedDishTarget = null;
            selectedEnemy = null;
            if (targetingArrow != null)
                targetingArrow.EnableArrow(false);
        }

        private bool TryPlayIngredientOnDishTarget()
        {
            if (SelectedCard == null || highlightedDishTarget == null)
                return false;

            var card = SelectedCard.GetComponent<CardObject>().RuntimeCard;
            var dishSystem = Object.FindFirstObjectByType<DishSystem>();
            if (dishSystem == null)
                return false;

            return dishSystem.TryRegisterIngredientPlayed(card.Template, highlightedDishTarget.StationIndex);
        }

        private void CancelIngredientTargeting()
        {
            if (SelectedCard == null)
                return;

            var card = SelectedCard.GetComponent<CardObject>();
            SelectedCard.transform.DOKill();
            card.Reset(() =>
            {
                SelectedCard = null;
                isArrowCreated = false;
            });

            highlightedDishTarget = null;
            if (targetingArrow != null)
                targetingArrow.EnableArrow(false);
        }

        private void UpdateTargetingArrow()
        {
            if (targetingArrow == null)
                return;

            var mousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            var diffY = mousePos.y - prevClickPos.y;
            if (isArrowCreated || diffY <= CardSelectionDetectionOffset)
                return;

            isArrowCreated = true;
            var pos = SelectedCard.transform.position;

            SelectedCard.transform.DOKill();
            var seq = DOTween.Sequence();
            seq.AppendCallback(() =>
            {
                SelectedCard.transform.DOMove(new Vector3(0, SelectedCardYOffset, pos.z), CardSelectionAnimationTime);
                SelectedCard.transform.DORotate(Vector3.zero, CardSelectionAnimationTime);
            });
            seq.AppendInterval(0.05f);
            seq.OnComplete(() => targetingArrow.EnableArrow(true));
        }

        protected override void PlaySelectedCard()
        {
            var template = SelectedCard.GetComponent<CardObject>().Template;

            base.PlaySelectedCard();

            if (ComboBattleSystem != null)
                ComboBattleSystem.ResolvePlayedCard(template, selectedEnemy);
        }

        public bool IsArrowActive()
        {
            return isArrowCreated;
        }

        public GameObject GetSelectedEnemy()
        {
            return selectedEnemy;
        }
    }
}
