// Copyright (C) 2019 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;

using Random = UnityEngine.Random;

namespace CCGKit
{
    /// <summary>
    /// Bootstraps the ComboGame scene: the wizard, the monsters they fight, their HP/status/intent
    /// widgets, the combo deck and the turn loop. Unlike <see cref="GameBootstrap"/> it does not
    /// read the map encounter or the save file, so the scene can be played standalone.
    /// </summary>
    public class ComboBattleBootstrap : MonoBehaviour
    {
#pragma warning disable 649
        [Header("Configuration")]
        [SerializeField]
        private PlayableCharacterConfiguration playerConfig;
        [SerializeField]
        private List<NonPlayableCharacterConfiguration> enemyConfigs = new List<NonPlayableCharacterConfiguration>();
        [SerializeField]
        private AssetReference characterTemplate;
        [Tooltip("Monsters to fight, in slot order. One or two are supported by the character pivots.")]
        [SerializeField]
        private List<EnemyTemplate> enemyTemplates = new List<EnemyTemplate>();
        [Tooltip("Deck used in this scene. Falls back to the hero starting deck when empty.")]
        [SerializeField]
        private CardLibrary comboDeck;

        [Header("Balance overrides")]
        [Tooltip("Player HP for this prototype. Zero uses the hero template value.")]
        [SerializeField]
        private int playerHpOverride;
        [Tooltip("HP for every monster in this prototype. Zero uses the enemy template range.")]
        [SerializeField]
        private int enemyHpOverride;

        [Header("Systems")]
        [SerializeField]
        private DeckDrawingSystem deckDrawingSystem;
        [SerializeField]
        private HandPresentationSystem handPresentationSystem;
        [SerializeField]
        private ManaResetSystem manaResetSystem;
        [SerializeField]
        private TurnManagementSystem turnManagementSystem;
        [SerializeField]
        private CardWithArrowSelectionSystem cardWithArrowSelectionSystem;
        [SerializeField]
        private ComboBattleSystem comboBattleSystem;

        [Header("Character pivots")]
        [SerializeField]
        private Transform playerPivot;
        [SerializeField]
        private List<Transform> enemyPivots1 = new List<Transform>();
        [SerializeField]
        private List<Transform> enemyPivots2 = new List<Transform>();

        [Header("UI")]
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private ManaWidget manaWidget;
        [SerializeField]
        private DeckWidget deckWidget;
        [SerializeField]
        private DiscardPileWidget discardPileWidget;
        [SerializeField]
        private SpriteRenderer background;
        [SerializeField]
        private Sprite backgroundOverride;

        [Header("Pools")]
        [SerializeField]
        private ObjectPool cardPool;
#pragma warning restore 649

        private Camera mainCamera;

        private readonly List<CardTemplate> playerDeck = new List<CardTemplate>();
        private readonly List<GameObject> enemies = new List<GameObject>();

        private GameObject player;

        private void Start()
        {
            mainCamera = Camera.main;
            cardPool.Initialize();

            if (background != null && backgroundOverride != null)
                background.sprite = backgroundOverride;

            Addressables.InitializeAsync().Completed += op => CreatePlayer(characterTemplate);
        }

        private void CreatePlayer(AssetReference templateRef)
        {
            var handle = Addressables.LoadAssetAsync<HeroTemplate>(templateRef);
            handle.Completed += op =>
            {
                var template = op.Result;

                SetPivotActive(playerPivot);
                player = Instantiate(template.Prefab, playerPivot);
                Assert.IsNotNull(player);

                var hp = playerConfig.Hp;
                var mana = playerConfig.Mana;
                var shield = playerConfig.Shield;

                hp.Value = playerHpOverride > 0 ? playerHpOverride : template.Hp;
                mana.Value = template.Mana;
                shield.Value = 0;

                if (manaResetSystem != null)
                    manaResetSystem.SetDefaultMana(template.Mana);
                if (manaWidget != null)
                    manaWidget.Initialize(mana);

                BuildPlayerDeck(template);

                CreateHpWidget(playerConfig.HpWidget, player, hp, hp.Value, shield);
                CreateStatusWidget(playerConfig.StatusWidget, player);

                var characterObject = player.GetComponent<CharacterObject>();
                characterObject.Template = template;
                characterObject.Character = new RuntimeCharacter
                {
                    Hp = hp,
                    Shield = shield,
                    Mana = mana,
                    Status = playerConfig.Status,
                    MaxHp = hp.Value
                };
                characterObject.Character.Status.Value.Clear();

                CreateEnemies();
                InitializeGame();
            };
        }

        private void BuildPlayerDeck(HeroTemplate template)
        {
            playerDeck.Clear();

            var library = comboDeck != null ? comboDeck : template.StartingDeck;
            if (library == null)
                return;

            foreach (var entry in library.Entries)
            {
                if (entry == null || entry.Card == null)
                    continue;
                for (var i = 0; i < entry.NumCopies; i++)
                    playerDeck.Add(entry.Card);
            }
        }

        private void CreateEnemies()
        {
            enemies.Clear();

            var pivots = enemyTemplates.Count >= 2 ? enemyPivots2 : enemyPivots1;

            for (var index = 0; index < enemyTemplates.Count; index++)
            {
                var template = enemyTemplates[index];
                if (template == null || index >= pivots.Count || index >= enemyConfigs.Count)
                    continue;

                var pivot = pivots[index];
                SetPivotActive(pivot);

                var enemy = Instantiate(template.Prefab, pivot);
                Assert.IsNotNull(enemy);

                var config = enemyConfigs[index];
                var initialHp = enemyHpOverride > 0
                    ? enemyHpOverride
                    : Random.Range(template.HpLow, template.HpHigh + 1);

                var hp = config.Hp;
                var shield = config.Shield;
                hp.Value = initialHp;
                shield.Value = 0;

                CreateHpWidget(config.HpWidget, enemy, hp, initialHp, shield);
                CreateStatusWidget(config.StatusWidget, enemy);
                CreateIntentWidget(config.IntentWidget, enemy);

                var characterObject = enemy.GetComponent<CharacterObject>();
                characterObject.Template = template;
                characterObject.IsDead = false;
                characterObject.Character = new RuntimeCharacter
                {
                    Hp = hp,
                    Shield = shield,
                    Status = config.Status,
                    MaxHp = initialHp
                };
                characterObject.Character.Status.Value.Clear();

                enemies.Add(enemy);
            }
        }

        /// <summary>Character pivots ship disabled in the scene, so the whole branch has to be enabled.</summary>
        private static void SetPivotActive(Transform pivot)
        {
            while (pivot != null)
            {
                pivot.gameObject.SetActive(true);
                pivot = pivot.parent;
            }
        }

        private void InitializeGame()
        {
            deckDrawingSystem.Initialize(deckWidget, discardPileWidget);
            deckDrawingSystem.LoadDeck(playerDeck);
            deckDrawingSystem.ShuffleDeck();

            handPresentationSystem.Initialize(cardPool, deckWidget, discardPileWidget);

            var playerCharacter = player.GetComponent<CharacterObject>();
            var enemyCharacters = new List<CharacterObject>(enemies.Count);
            foreach (var enemy in enemies)
                enemyCharacters.Add(enemy.GetComponent<CharacterObject>());

            cardWithArrowSelectionSystem.Initialize(playerCharacter, enemyCharacters);
            if (comboBattleSystem != null)
                comboBattleSystem.Initialize(playerCharacter, enemyCharacters);

            turnManagementSystem.BeginGame();
        }

        private void CreateHpWidget(GameObject prefab, GameObject character, IntVariable hp, int maxHp, IntVariable shield)
        {
            if (prefab == null || canvas == null)
                return;

            var widget = Instantiate(prefab, canvas.transform, false);
            AnchorToCharacter(widget, character, new Vector3(0.0f, -0.5f, 0.0f));
            widget.GetComponent<HpWidget>().Initialize(hp, maxHp, shield);
        }

        private void CreateStatusWidget(GameObject prefab, GameObject character)
        {
            if (prefab == null || canvas == null)
                return;

            var widget = Instantiate(prefab, canvas.transform, false);
            AnchorToCharacter(widget, character, new Vector3(0.0f, -1.0f, 0.0f));
        }

        private void CreateIntentWidget(GameObject prefab, GameObject character)
        {
            if (prefab == null || canvas == null)
                return;

            var collider = character.GetComponent<BoxCollider2D>();
            var height = collider != null ? collider.bounds.size.y : 1.0f;

            var widget = Instantiate(prefab, canvas.transform, false);
            AnchorToCharacter(widget, character, new Vector3(-0.5f, height + 0.7f, 0.0f));
        }

        private void AnchorToCharacter(GameObject widget, GameObject character, Vector3 offset)
        {
            var canvasPos = mainCamera.WorldToViewportPoint(character.transform.position + offset);
            var rectTransform = widget.GetComponent<RectTransform>();
            rectTransform.anchorMin = canvasPos;
            rectTransform.anchorMax = canvasPos;
        }
    }
}
